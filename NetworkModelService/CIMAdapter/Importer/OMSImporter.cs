using System;
using System.Collections.Generic;
using CIM.Model;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
	/// <summary>
	/// PowerTransformerImporter
	/// </summary>
	public class OMSImporter
	{
		/// <summary> Singleton </summary>
		private static OMSImporter ptImporter = null;
		private static object singletoneLock = new object();

		private ConcreteModel concreteModel;
		private Delta delta;
		private ImportHelper importHelper;
		private TransformAndLoadReport report;


		#region Properties
		public static OMSImporter Instance
		{
			get
			{
				if (ptImporter == null)
				{
					lock (singletoneLock)
					{
						if (ptImporter == null)
						{
							ptImporter = new OMSImporter();
							ptImporter.Reset();
						}
					}
				}
				return ptImporter;
			}
		}

		public Delta NMSDelta
		{
			get 
			{
				return delta;
			}
		}
		#endregion Properties


		public void Reset()
		{
			concreteModel = null;
			delta = new Delta();
			importHelper = new ImportHelper();
			report = null;
		}

		public TransformAndLoadReport CreateNMSDelta(ConcreteModel cimConcreteModel)
		{
			LogManager.Log("Importing PowerTransformer Elements...", LogLevel.Info);
			report = new TransformAndLoadReport();
			concreteModel = cimConcreteModel;
			delta.ClearDeltaOperations();

			if ((concreteModel != null) && (concreteModel.ModelMap != null))
			{
				try
				{
					// convert into DMS elements
					ConvertModelAndPopulateDelta();
				}
				catch (Exception ex)
				{
					string message = string.Format("{0} - ERROR in data import - {1}", DateTime.Now, ex.Message);
					LogManager.Log(message);
					report.Report.AppendLine(ex.Message);
					report.Success = false;
				}
			}
			LogManager.Log("Importing PowerTransformer Elements - END.", LogLevel.Info);
			return report;
		}

		/// <summary>
		/// Method performs conversion of network elements from CIM based concrete model into DMS model.
		/// </summary>
		private void ConvertModelAndPopulateDelta()
		{
			LogManager.Log("Loading elements and creating delta...", LogLevel.Info);

            //// import all concrete model types (DMSType enum)
            ImportConnectNodeCount();
            ImportConnectNode();
            ImportEnergySource();
            ImportACLineSegment();
            ImportBreaker();
            ImportEnergyConsumer();
            ImportTerminal();
            ImportDiscrete();
            ImportAnalog();

			LogManager.Log("Loading elements and creating delta completed.", LogLevel.Info);
		}

		#region Import
		private void ImportConnectNodeCount()
		{
			SortedDictionary<string, object> cimConnNodeCont = concreteModel.GetAllObjectsOfType("FTN.ConnectivityNodeContainer");
			if (cimConnNodeCont != null)
			{
				foreach (KeyValuePair<string, object> cimConnNodeContPair in cimConnNodeCont)
				{
					FTN.ConnectivityNodeContainer cimCNC = cimConnNodeContPair.Value as FTN.ConnectivityNodeContainer;

					ResourceDescription rd = CreateConnNodeContResourceDescription(cimCNC);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("ConnectivityNodeContainer ID = ").Append(cimCNC.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("ConnectivityNodeContainer ID = ").Append(cimCNC.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateConnNodeContResourceDescription(FTN.ConnectivityNodeContainer cimCNC)
		{
			ResourceDescription rd = null;
			if (cimCNC != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.CONNECTNODECONT, importHelper.CheckOutIndexForDMSType(DMSType.CONNECTNODECONT));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimCNC.ID, gid);

				////populate ResourceDescription
				OMSConverter.PopulateConnectivityNodeContainerProperties(cimCNC, rd,importHelper,report);
			}
			return rd;
		}
		
		private void ImportConnectNode()
		{
			SortedDictionary<string, object> cimCN = concreteModel.GetAllObjectsOfType("FTN.ConnectivityNode");
			if (cimCN != null)
			{
				foreach (KeyValuePair<string, object> cimCNPair in cimCN)
				{
					FTN.ConnectivityNode cimConnNode = cimCNPair.Value as FTN.ConnectivityNode;

                    ResourceDescription rd = CreateConnectNodeResourceDescriptionn(cimConnNode);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("ConnectivityNode ID = ").Append(cimConnNode.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("ConnectivityNode ID = ").Append(cimConnNode.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateConnectNodeResourceDescriptionn(FTN.ConnectivityNode cimConnNode)
		{
			ResourceDescription rd = null;
			if (cimConnNode != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.CONNECTNODE, importHelper.CheckOutIndexForDMSType(DMSType.CONNECTNODE));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimConnNode.ID, gid);

				////populate ResourceDescription
				OMSConverter.PopulateConnecttivityNodeProperties(cimConnNode, rd,importHelper,report);
			}
			return rd;
		}

		private void ImportEnergySource()
		{
			SortedDictionary<string, object> cimEnergySource = concreteModel.GetAllObjectsOfType("FTN.EnergySource");
			if (cimEnergySource != null)
			{
				foreach (KeyValuePair<string, object> cimEnergySourcePair in cimEnergySource)
				{
					FTN.EnergySource cimES = cimEnergySourcePair.Value as FTN.EnergySource;

					ResourceDescription rd = CreateEnergySourceResourceDescription(cimES);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("EnergySource ID = ").Append(cimES.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("EnergySource ID = ").Append(cimES.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateEnergySourceResourceDescription(FTN.EnergySource cimEnergSource)
		{
			ResourceDescription rd = null;
			if (cimEnergSource != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ENERGSOURCE, importHelper.CheckOutIndexForDMSType(DMSType.ENERGSOURCE));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimEnergSource.ID, gid);

				////populate ResourceDescription
				OMSConverter.PopulateEnergySourceProperties(cimEnergSource, rd, importHelper, report);
			}
			return rd;
		}

        private void ImportEnergyConsumer()
        {
            SortedDictionary<string, object> cimEnergyConsumer = concreteModel.GetAllObjectsOfType("FTN.EnergyConsumer");
            if (cimEnergyConsumer != null)
            {
                foreach (KeyValuePair<string, object> cimEnergyConsumerPair in cimEnergyConsumer)
                {
                    FTN.EnergyConsumer cimEC = cimEnergyConsumerPair.Value as FTN.EnergyConsumer;

                    ResourceDescription rd = CreateEnergyConsumerResourceDescription(cimEC);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("EnergyConsumer ID = ").Append(cimEC.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("EnergyConsumer ID = ").Append(cimEC.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateEnergyConsumerResourceDescription(FTN.EnergyConsumer cimEnergyConsumer)
        {
            ResourceDescription rd = null;
            if (cimEnergyConsumer != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ENERGCONSUMER, importHelper.CheckOutIndexForDMSType(DMSType.ENERGCONSUMER));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimEnergyConsumer.ID, gid);

                ////populate ResourceDescription
                OMSConverter.PopulateEnergyConsumerProperties(cimEnergyConsumer, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportACLineSegment()
        {
            SortedDictionary<string, object> cimAcLineSeg = concreteModel.GetAllObjectsOfType("FTN.ACLineSegment");
            if (cimAcLineSeg != null)
            {
                foreach (KeyValuePair<string, object> cimAcLineSegPair in cimAcLineSeg)
                {
                    FTN.ACLineSegment cimACLineSeg = cimAcLineSegPair.Value as FTN.ACLineSegment;

                    ResourceDescription rd = CreateACLineSegResourceDescription(cimACLineSeg);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("ACLineSegment ID = ").Append(cimACLineSeg.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("ACLineSegment ID = ").Append(cimACLineSeg.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateACLineSegResourceDescription(FTN.ACLineSegment cimACLineSegment)
        {
            ResourceDescription rd = null;
            if (cimACLineSegment != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ACLINESEGMENT, importHelper.CheckOutIndexForDMSType(DMSType.ACLINESEGMENT));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimACLineSegment.ID, gid);

                ////populate ResourceDescription
                OMSConverter.PopulateACLineSegmentProperties(cimACLineSegment, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportBreaker()
        {
            SortedDictionary<string, object> cimBreaker = concreteModel.GetAllObjectsOfType("FTN.Breaker");
            if (cimBreaker != null)
            {
                foreach (KeyValuePair<string, object> cimBreakerPair in cimBreaker)
                {
                    FTN.Breaker cimBreak = cimBreakerPair.Value as FTN.Breaker;

                    ResourceDescription rd = CreateBreakerResourceDescription(cimBreak);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Breaker ID = ").Append(cimBreak.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Breaker ID = ").Append(cimBreak.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateBreakerResourceDescription(FTN.Breaker cimBreakerSegment)
        {
            ResourceDescription rd = null;
            if (cimBreakerSegment != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.BREAKER, importHelper.CheckOutIndexForDMSType(DMSType.BREAKER));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimBreakerSegment.ID, gid);

                ////populate ResourceDescription
                OMSConverter.PopulateBreakerProperties(cimBreakerSegment, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportTerminal()
        {
            SortedDictionary<string, object> cimTerminal = concreteModel.GetAllObjectsOfType("FTN.Terminal");
            if (cimTerminal != null)
            {
                foreach (KeyValuePair<string, object> cimTerminalPair in cimTerminal)
                {
                    FTN.Terminal cimTermi = cimTerminalPair.Value as FTN.Terminal;

                    ResourceDescription rd = CreateTerminalResourceDescription(cimTermi);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Terminal ID = ").Append(cimTermi.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Terminal ID = ").Append(cimTermi.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateTerminalResourceDescription(FTN.Terminal cimTerminal)
        {
            ResourceDescription rd = null;
            if (cimTerminal != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.TERMINAL, importHelper.CheckOutIndexForDMSType(DMSType.TERMINAL));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimTerminal.ID, gid);

                ////populate ResourceDescription
                OMSConverter.PopulateTerminalProperties(cimTerminal, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportDiscrete()
        {
            SortedDictionary<string, object> cimDiscrete = concreteModel.GetAllObjectsOfType("FTN.Discrete");
            if (cimDiscrete != null)
            {
                foreach (KeyValuePair<string, object> cimDiscretePair in cimDiscrete)
                {
                    FTN.Discrete cimDisc = cimDiscretePair.Value as FTN.Discrete;

                    ResourceDescription rd = CreateDiscreteResourceDescription(cimDisc);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Discrete ID = ").Append(cimDisc.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Discrete ID = ").Append(cimDisc.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateDiscreteResourceDescription(FTN.Discrete cimDiscrete)
        {
            ResourceDescription rd = null;
            if (cimDiscrete != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.DISCRETE, importHelper.CheckOutIndexForDMSType(DMSType.DISCRETE));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimDiscrete.ID, gid);

                ////populate ResourceDescription
                OMSConverter.PopulateDiscreteProperties(cimDiscrete, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportAnalog()
        {
            SortedDictionary<string, object> cimAnalog = concreteModel.GetAllObjectsOfType("FTN.Analog");
            if (cimAnalog != null)
            {
                foreach (KeyValuePair<string, object> cimAnalogPair in cimAnalog)
                {
                    FTN.Analog cimAnalo = cimAnalogPair.Value as FTN.Analog;

                    ResourceDescription rd = CreatecimAnalogResourceDescription(cimAnalo);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Analog ID = ").Append(cimAnalo.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Analog ID = ").Append(cimAnalo.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreatecimAnalogResourceDescription(FTN.Analog cimAnalog)
        {
            ResourceDescription rd = null;
            if (cimAnalog != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ANALOG, importHelper.CheckOutIndexForDMSType(DMSType.ANALOG));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimAnalog.ID, gid);

                ////populate ResourceDescription
                OMSConverter.PopulateAnalogProperties(cimAnalog, rd, importHelper, report);
            }
            return rd;
        }

		
		#endregion Import
	}
}

