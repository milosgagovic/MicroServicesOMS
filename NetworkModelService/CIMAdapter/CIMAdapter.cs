using System;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using CIM.Model;
using CIMParser;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Importer;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;
using FTN.ServiceContracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using TransactionManagerContract;

namespace FTN.ESI.SIMES.CIM.CIMAdapter
{
	public class CIMAdapter
	{
        private NetworkModelGDAProxy gdaQueryProxy = null;
		private IOMSClient proxyToTransactionManager;
		private ChannelFactory<IOMSClient> factoryToTMS = null;
        private DispatcherClient dispatcherClient;

        public CIMAdapter()
		{
            NetTcpBinding binding = new NetTcpBinding();
            // Create a partition resolver
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            // create a  WcfCommunicationClientFactory object.
            var wcfClientFactory = new WcfCommunicationClientFactory<IOMSClient>
                (clientBinding: binding, servicePartitionResolver: partitionResolver);

            //
            // Create a client for communicating with the ICalculator service that has been created with the
            // Singleton partition scheme.
            //
            dispatcherClient = new DispatcherClient(
                           wcfClientFactory,
                           new Uri("fabric:/ServiceFabricOMS/TMServiceEndpoint"),
                           ServicePartitionKey.Singleton);

        }

        private IOMSClient ProxyToTransactionManager
		{
			get
			{
				if (proxyToTransactionManager == null /*&& factoryToTMS == null*/)
				{
                    var binding = new NetTcpBinding();
                    binding.CloseTimeout = TimeSpan.FromDays(10);
                    binding.OpenTimeout = TimeSpan.FromDays(10);
                    binding.ReceiveTimeout = TimeSpan.FromDays(10);
                    binding.SendTimeout = TimeSpan.FromDays(10);
                    binding.TransactionFlow = true;

                    factoryToTMS = new ChannelFactory<IOMSClient>(BindingForTCP.CreateCustomNetTcp(), new EndpointAddress("net.tcp://localhost:7090/TMServiceEndpoint"));
                    proxyToTransactionManager = factoryToTMS.CreateChannel();
                }

				return proxyToTransactionManager;
			}
		}

		private NetworkModelGDAProxy GdaQueryProxy
        {
            get
            {
                if (gdaQueryProxy != null)
                {
                    gdaQueryProxy.Abort();
                    gdaQueryProxy = null;
                }

                gdaQueryProxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
                gdaQueryProxy.Open();

                return gdaQueryProxy;
            }
        }

		public Delta CreateDelta(Stream extract, SupportedProfiles extractType, out string log)
		{
			Delta nmsDelta = null;
			ConcreteModel concreteModel = null;
			Assembly assembly = null;
			string loadLog = string.Empty;
			string transformLog = string.Empty;

			if (LoadModelFromExtractFile(extract, extractType, ref concreteModel, ref assembly, out loadLog))
			{
				DoTransformAndLoad(assembly, concreteModel, extractType, out nmsDelta, out transformLog);
			}
			log = string.Concat("Load report:\r\n", loadLog, "\r\nTransform report:\r\n", transformLog);

			return nmsDelta;
		}

		public string ApplyUpdates(Delta delta)
		{
			string updateResult = "Apply Updates Report:\r\n";
			System.Globalization.CultureInfo culture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

			if ((delta != null) && (delta.NumberOfOperations != 0))
			{
                //// NetworkModelService->ApplyUpdates
                //updateResult = ProxyToTransactionManager.UpdateSystem(delta).ToString();
                //call Transaction manager
                //updateResult = GdaQueryProxy.ApplyUpdate(delta).ToString();
                //  updateResult = dispatcherClient.InvokeWithRetry(client => client.Channel.UpdateSystem(delta)).ToString();
                ProxyToTransactionManager.UpdateSystem(delta).ToString();
            }

			Thread.CurrentThread.CurrentCulture = culture;
			return updateResult;
		}


		private bool LoadModelFromExtractFile(Stream extract, SupportedProfiles extractType, ref ConcreteModel concreteModelResult, ref Assembly assembly, out string log)
		{
			bool valid = false;
			log = string.Empty;

			System.Globalization.CultureInfo culture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
			try
			{
				ProfileManager.LoadAssembly(extractType, out assembly);
				if (assembly != null)
				{
					CIMModel cimModel = new CIMModel();
					CIMModelLoaderResult modelLoadResult = CIMModelLoader.LoadCIMXMLModel(extract, ProfileManager.Namespace, out cimModel);
					if (modelLoadResult.Success)
					{
						concreteModelResult = new ConcreteModel();
						ConcreteModelBuilder builder = new ConcreteModelBuilder();
						ConcreteModelBuildingResult modelBuildResult = builder.GenerateModel(cimModel, assembly, ProfileManager.Namespace, ref concreteModelResult);

						if (modelBuildResult.Success)
						{
							valid = true;
						}
						log = modelBuildResult.Report.ToString();
					}
					else
					{
						log = modelLoadResult.Report.ToString();
					}
				}
			}
			catch (Exception e)
			{
				log = e.Message;
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = culture;
			}
			return valid;
		}

		private bool DoTransformAndLoad(Assembly assembly, ConcreteModel concreteModel, SupportedProfiles extractType, out Delta nmsDelta, out string log)
		{
			nmsDelta = null;
			log = string.Empty;
			bool success = false;
			try
			{
				LogManager.Log(string.Format("Importing {0} data...", extractType), LogLevel.Info);

				switch (extractType)
				{
					case SupportedProfiles.OMS:
						{
							// transformation to DMS delta					
							TransformAndLoadReport report = OMSImporter.Instance.CreateNMSDelta(concreteModel);

							if (report.Success)
							{
								nmsDelta = OMSImporter.Instance.NMSDelta;
								success = true;
							}
							else
							{
								success = false;
							}
							log = report.Report.ToString();
							OMSImporter.Instance.Reset();

							break;
						}
					default:
						{
							LogManager.Log(string.Format("Import of {0} data is NOT SUPPORTED.", extractType), LogLevel.Warning);
							break;
						}
				}

				return success;
			}
			catch (Exception ex)
			{
				LogManager.Log(string.Format("Import unsuccessful: {0}", ex.StackTrace), LogLevel.Error);
				return false;
			}
		}
        public void ClearDataBaseOnNMS()
        {
            NetTcpBinding binding = new NetTcpBinding();
            ChannelFactory<IOMSClient>  factoryToTMS = new ChannelFactory<IOMSClient>(binding, new EndpointAddress("net.tcp://localhost:6080/TransactionManagerService"));
            proxyToTransactionManager = factoryToTMS.CreateChannel();
            proxyToTransactionManager.ClearNMSDB();
        }

	}
}
