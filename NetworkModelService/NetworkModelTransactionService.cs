using FTN.Common;
using FTN.ServiceContracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using TransactionManagerContract;

namespace FTN.Services.NetworkModelService
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class NetworkModelTransactionService : ITransaction
    {
        //   private static GenericDataAccess gda; // = new GenericDataAccess();
        private NetworkGDAServiceFabric networkGDAServiceFabric;

        public NetworkGDAServiceFabric NetworkGDAServiceFabric
        {
            get
            {
                if(networkGDAServiceFabric == null)
                {
                    IServicePartitionResolver partitionResolverToNMS = ServicePartitionResolver.GetDefault();
                    var wcfClientFactoryToNMS = new WcfCommunicationClientFactory<INetworkModelGDAContract>
                        (clientBinding: BindingForTCP.CreateCustomNetTcp(), servicePartitionResolver: partitionResolverToNMS);
                    networkGDAServiceFabric = new NetworkGDAServiceFabric(
                                    wcfClientFactoryToNMS,
                                    new Uri("fabric:/ServiceFabricOMS/NMStatelessService"),
                                    ServicePartitionKey.Singleton,
                                    listenerName: "NMServiceEndpoint");
                }
                return networkGDAServiceFabric;
            }
            set => networkGDAServiceFabric = value;
        }

        public NetworkModelTransactionService()
        {
          //  gda = new GenericDataAccess();
        }

        public void Enlist()
        {
            ITransactionCallback callback = OperationContext.Current.GetCallbackChannel<ITransactionCallback>();
            Console.WriteLine("Pozvan je enlist na NMS-u");
            try
            {
                networkGDAServiceFabric.InvokeWithRetry(client => client.Channel.GetCopyOfNetworkModel());
               // gda.GetCopyOfNetworkModel();
                callback.CallbackEnlist(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                callback.CallbackEnlist(false);
            }
        }

        public void Prepare(Delta delta)
        {
            Console.WriteLine("Pozvan je prepare na NMS-u");
            ITransactionCallback callback = OperationContext.Current.GetCallbackChannel<ITransactionCallback>();

            try
            {
                UpdateResult updateResult = NetworkGDAServiceFabric.InvokeWithRetry(client => client.Channel.ApplyUpdate(delta));
                //UpdateResult updateResult = gda.ApplyUpdate(delta);
                if (updateResult.Result == ResultType.Succeeded)
                {
                    callback.CallbackPrepare(true);
                    PushDataToDatabase(delta);
                }
                else
                {
                    Rollback();
                    callback.CallbackPrepare(false);
                }
            }
            catch (Exception ex)
            {
                Rollback();
                callback.CallbackPrepare(false);
                Console.WriteLine(ex.Message);
            }
        }

        public void Commit()
        {
            Console.WriteLine("Pozvan je Commit na NMS-u");

            if (GenericDataAccess.NewNetworkModel != null)
            {
                GenericDataAccess.NetworkModel = GenericDataAccess.NewNetworkModel;
                ResourceIterator.NetworkModel = GenericDataAccess.NewNetworkModel;
            }


            ITransactionCallback callback = OperationContext.Current.GetCallbackChannel<ITransactionCallback>();
            callback.CallbackCommit("Uspjesno je prosao commit na NMS-u");
        }
            
        public void Rollback()
        {
            Console.WriteLine("Pozvan je RollBack na NMSu");
            GenericDataAccess.NewNetworkModel = null;
            GenericDataAccess.NetworkModel = GenericDataAccess.OldNetworkModel;
            ResourceIterator.NetworkModel = GenericDataAccess.OldNetworkModel;
            ITransactionCallback callback = OperationContext.Current.GetCallbackChannel<ITransactionCallback>();
            callback.CallbackRollback("Something went wrong on NMS");
        }

        private void PushDataToDatabase(Delta delta)
        {
            using (NMSAdoNet ctx = new NMSAdoNet())
            {
                foreach (ResourceDescription rd in delta.InsertOperations)
                {
                    ctx.ResourceDescription.Add(rd);
                    foreach (Property item in rd.Properties)
                    {
                        ctx.PropertyValue.Add(item.PropertyValue);
                        ctx.Property.Add(item);
                    }
                    ctx.SaveChanges();
                }
                foreach (ResourceDescription rd in delta.UpdateOperations)
                {
                    List<Property> propForRD = ctx.Property.Where(x => x.ResourceDescription_Id == rd.IdDb).ToList();
                    propForRD.ForEach(x => x.PropertyValue = ctx.PropertyValue.Where(y => y.Id == x.IdDB).FirstOrDefault());
                    ctx.SaveChanges();
                }

                foreach (ResourceDescription rd in delta.DeleteOperations)
                {
                }
            }
        }
    }
}
