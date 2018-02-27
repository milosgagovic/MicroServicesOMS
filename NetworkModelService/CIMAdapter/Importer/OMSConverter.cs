namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
    using FTN.Common;
    using System.Collections.Generic;

    /// <summary>
    /// PowerTransformerConverter has methods for populating
    /// ResourceDescription objects using PowerTransformerCIMProfile_Labs objects.
    /// </summary>
    public static class OMSConverter
    {

        #region Populate ResourceDescription
        public static void PopulateIdentifiedObjectProperties(FTN.IdentifiedObject cimIdentifiedObject, ResourceDescription rd)
        {
            if ((cimIdentifiedObject != null) && (rd != null))
            {
                if (cimIdentifiedObject.MRIDHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimIdentifiedObject.MRID));
                }
                if (cimIdentifiedObject.NameHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimIdentifiedObject.Name));
                }
                //if (cimIdentifiedObject.DescriptionHasValue)
                //{
                //	rd.AddProperty(new Property(ModelCode.IDOBJ_DESCRIPTION, cimIdentifiedObject.Description));
                //}
            }
        }


        public static void PopulatePowerSystemResourceProperties(FTN.PowerSystemResource cimPowerSystemResource, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimPowerSystemResource != null) && (rd != null))
            {
                OMSConverter.PopulateIdentifiedObjectProperties(cimPowerSystemResource, rd);

            }
        }

        public static void PopulateEquipmentProperties(FTN.Equipment cimEquipment, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimEquipment != null) && (rd != null))
            {
                OMSConverter.PopulatePowerSystemResourceProperties(cimEquipment, rd, importHelper, report);

                if (cimEquipment.NormallyInServiceHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.EQUIPMENT_NORMINSERV, cimEquipment.NormallyInService));
                }
            }
        }

        public static void PopulateConductingEquipmentProperties(FTN.ConductingEquipment cimConductingEquipment, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimConductingEquipment != null) && (rd != null))
            {
                OMSConverter.PopulateEquipmentProperties(cimConductingEquipment, rd, importHelper, report);
            }
        }

        public static void PopulateConnectivityNodeContainerProperties(FTN.ConnectivityNodeContainer cimConnNCon, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimConnNCon != null) && (rd != null))
            {
                OMSConverter.PopulatePowerSystemResourceProperties(cimConnNCon, rd, importHelper, report);

            }
        }

        public static void PopulateConnecttivityNodeProperties(FTN.ConnectivityNode cimConnecNode, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimConnecNode != null) && (rd != null))
            {
                OMSConverter.PopulateIdentifiedObjectProperties(cimConnecNode, rd);

                if (cimConnecNode.ConnectivityNodeContainerHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimConnecNode.ConnectivityNodeContainer.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimConnecNode.GetType().ToString()).Append(" rdfID = \"").Append(cimConnecNode.ID);
                        report.Report.Append("\" - Failed to set reference to PowerTransformer: rdfID \"").Append(cimConnecNode.ConnectivityNodeContainer.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.CONNECTNODE_CONNECTNODECONT, gid));
                }
            }
        }

		public static void PopulateTerminalProperties(FTN.Terminal cimTerminal, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimTerminal != null) && (rd != null))
			{
				OMSConverter.PopulateIdentifiedObjectProperties(cimTerminal, rd);

				if (cimTerminal.ConnectivityNodeHasValue)
				{
					long gid = importHelper.GetMappedGID(cimTerminal.ConnectivityNode.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cimTerminal.GetType().ToString()).Append(" rdfID = \"").Append(cimTerminal.ID);
						report.Report.Append("\" - Failed to set reference to TransformerWinding: rdfID \"").Append(cimTerminal.ConnectivityNode.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.TERMINAL_CONNECTNODE, gid));
				}
                if (cimTerminal.ConductingEquipmentHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimTerminal.ConductingEquipment.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimTerminal.GetType().ToString()).Append(" rdfID = \"").Append(cimTerminal.ID);
                        report.Report.Append("\" - Failed to set reference to TransformerWinding: rdfID \"").Append(cimTerminal.ConductingEquipment.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.TERMINAL_CONDEQUIP, gid));
                }
            }
        }

        public static void PopulateSwitchProperties(FTN.Switch cimSwitch, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimSwitch != null) && (rd != null))
            {
                OMSConverter.PopulateConductingEquipmentProperties(cimSwitch, rd, importHelper, report);

                if (cimSwitch.NormalOpenHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SWITCH_NORMOPEN, cimSwitch.NormalOpen));
                }

                if (cimSwitch.SwitchOnCountHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SWITCH_ONCOUNT, cimSwitch.SwitchOnCount));
                }

                if (cimSwitch.SwitchOnDateHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SWITCH_ONDATE, cimSwitch.SwitchOnDate));
                }
            }
        }

        public static void PopulateProtectedSwitchProperties(FTN.ProtectedSwitch cimProtSwitch, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimProtSwitch != null) && (rd != null))
            {
                OMSConverter.PopulateSwitchProperties(cimProtSwitch, rd, importHelper, report);
            }
        }

        public static void PopulateBreakerProperties(FTN.Breaker cimBreaker, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimBreaker != null) && (rd != null))
            {
                OMSConverter.PopulateProtectedSwitchProperties(cimBreaker, rd, importHelper, report);
            }
        }

        public static void PopulateConductorProperties(FTN.Conductor cimConductor, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimConductor != null) && (rd != null))
            {
                OMSConverter.PopulateConductingEquipmentProperties(cimConductor, rd, importHelper, report);

                if (cimConductor.LengthHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.CONDUCTOR_LEN, cimConductor.Length));
                }

            }
        }

        public static void PopulateACLineSegmentProperties(FTN.ACLineSegment cimACLineSeg, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimACLineSeg != null) && (rd != null))
            {
                OMSConverter.PopulateConductorProperties(cimACLineSeg, rd, importHelper, report);
            }
        }

        public static void PopulateEnergySourceProperties(FTN.EnergySource cimEnergySource, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimEnergySource != null) && (rd != null))
            {
                OMSConverter.PopulateConductingEquipmentProperties(cimEnergySource, rd, importHelper, report);

                if (cimEnergySource.ActivePowerHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ENERGSOURCE_ACTPOW, cimEnergySource.ActivePower));
                }
                if (cimEnergySource.NominalVoltageHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ENERGSOURCE_NOMVOLT, cimEnergySource.NominalVoltage));
                }

            }
        }

        public static void PopulateEnergyConsumerProperties(FTN.EnergyConsumer cimEnergyConsumer, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimEnergyConsumer != null) && (rd != null))
            {
                OMSConverter.PopulateConductingEquipmentProperties(cimEnergyConsumer, rd, importHelper, report);

                if (cimEnergyConsumer.PfixedHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ENERGCONSUMER_PFIXED, cimEnergyConsumer.Pfixed));
                }
                if (cimEnergyConsumer.QfixedHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ENERGCONSUMER_QFIXED, cimEnergyConsumer.Qfixed));
                }

            }
        }

        public static void PopulateMeasurmentProperties(FTN.Measurement cimMeasurment, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimMeasurment != null) && (rd != null))
            {
                OMSConverter.PopulateIdentifiedObjectProperties(cimMeasurment, rd);

                if (cimMeasurment.DirectionHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.MEASUREMENT_DIRECTION, (short)cimMeasurment.Direction));
                }
                if (cimMeasurment.MeasurementTypeHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.MEASUREMENT_TYPE, cimMeasurment.MeasurementType));
                }
                if (cimMeasurment.UnitSymbolHasValue)
                {
                    var unitSym = GetDMSUnitSymbol(cimMeasurment.UnitSymbol);
                    rd.AddProperty(new Property(ModelCode.MEASUREMENT_UNITSYMB, (short)unitSym));
                    //rd.AddProperty(new Property(ModelCode.MEASUREMENT_UNITSYMB, (long)cimMeasurment.UnitSymbol));
                    //rd.AddProperty(new Property(ModelCode.MEASUREMENT_UNITSYMB, (short)cimMeasurment.UnitSymbol));
                    //rd.AddProperty(new Property(ModelCode.MEASUREMENT_UNITSYMB, cimMeasurment.UnitSymbol.ToString()));
                }
                if (cimMeasurment.PowerSystemResourceHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimMeasurment.PowerSystemResource.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimMeasurment.GetType().ToString()).Append(" rdfID = \"").Append(cimMeasurment.ID);
                        report.Report.Append("\" - Failed to set reference to TransformerWinding: rdfID \"").Append(cimMeasurment.PowerSystemResource.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.MEASUREMENT_PSR, gid));
                }

            }
        }

        public static void PopulateAnalogProperties(FTN.Analog cimAnalog, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimAnalog != null) && (rd != null))
            {
                OMSConverter.PopulateMeasurmentProperties(cimAnalog, rd, importHelper, report);

                if (cimAnalog.MinValueHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ANALOG_MINVAL, cimAnalog.MinValue));
                }
                if (cimAnalog.MaxValueHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ANALOG_MAXVAL, cimAnalog.MaxValue));
                }
                if (cimAnalog.NormalValueHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ANALOG_NORMVAL, cimAnalog.NormalValue));
                }

            }
        }

        public static void PopulateDiscreteProperties(FTN.Discrete cimDiscrete, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimDiscrete != null) && (rd != null))
            {
                OMSConverter.PopulateMeasurmentProperties(cimDiscrete, rd, importHelper, report);

                if (cimDiscrete.MinValueHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.DISCRETE_MINVAL, cimDiscrete.MinValue));
                }
                if (cimDiscrete.MaxValueHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.DISCRETE_MAXVAL, cimDiscrete.MaxValue));
                }
                if (cimDiscrete.NormalValueHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.DISCRETE_NORMVAL, cimDiscrete.NormalValue));
                }
                if (cimDiscrete.ValidCommandsHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.DISCRETE_VALIDCOMMANDS,GetDMSCommand(cimDiscrete.ValidCommands)));
                }

                if (cimDiscrete.ValidStatesHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.DISCRETE_VALIDSTATES, GetDMSStates(cimDiscrete.ValidStates)));
                }

            }
        }
        #endregion Populate ResourceDescription

        #region Enums convert
        public static UnitSymbol GetDMSUnitSymbol(FTN.UnitSymbol symbol)
        {
            switch (symbol)
            {
                case FTN.UnitSymbol.A:
                    return UnitSymbol.A;
                case FTN.UnitSymbol.deg:
                    return UnitSymbol.deg;
                case FTN.UnitSymbol.degC:
                    return UnitSymbol.degC;
                case FTN.UnitSymbol.F:
                    return UnitSymbol.F;
                case FTN.UnitSymbol.g:
                    return UnitSymbol.g;
                case FTN.UnitSymbol.h:
                    return UnitSymbol.h;
                case FTN.UnitSymbol.H:
                    return UnitSymbol.H;
                case FTN.UnitSymbol.Hz:
                    return UnitSymbol.Hz;
                case FTN.UnitSymbol.J:
                    return UnitSymbol.J;
                case FTN.UnitSymbol.m:
                    return UnitSymbol.m;
                case FTN.UnitSymbol.m2:
                    return UnitSymbol.m2;
                case FTN.UnitSymbol.m3:
                    return UnitSymbol.m3;
                case FTN.UnitSymbol.min:
                    return UnitSymbol.min;
                case FTN.UnitSymbol.N:
                    return UnitSymbol.N;
                case FTN.UnitSymbol.none:
                    return UnitSymbol.none;
                case FTN.UnitSymbol.ohm:
                    return UnitSymbol.ohm;
                case FTN.UnitSymbol.Pa:
                    return UnitSymbol.Pa;
                case FTN.UnitSymbol.rad:
                    return UnitSymbol.rad;
                case FTN.UnitSymbol.s:
                    return UnitSymbol.s;
                case FTN.UnitSymbol.S:
                    return UnitSymbol.S;
                case FTN.UnitSymbol.V:
                    return UnitSymbol.V;
                case FTN.UnitSymbol.VA:
                    return UnitSymbol.VA;
                case FTN.UnitSymbol.VAh:
                    return UnitSymbol.VAh;
                case FTN.UnitSymbol.VAr:
                    return UnitSymbol.VAr;
                case FTN.UnitSymbol.VArh:
                    return UnitSymbol.VArh;
                case FTN.UnitSymbol.W:
                    return UnitSymbol.W;
                case FTN.UnitSymbol.Wh:
                    return UnitSymbol.Wh;
                default: return UnitSymbol.none;
            }
        }
        public static DirectionType GetDMSDirectionType(FTN.DirectionType dir)
        {
            switch (dir)
            {
                case FTN.DirectionType.Read:
                    return DirectionType.Read;
                case FTN.DirectionType.Write:
                    return DirectionType.Write;
                case FTN.DirectionType.ReadWrite:
                    return DirectionType.ReadWrite;
                default:
                    return DirectionType.ReadWrite;
            }
        }
        public static List<short> GetDMSCommand(List<FTN.Commands> commands)
        {
            List<short> pom = new List<short>();
            foreach (var item in commands)
            {
                switch (item)
                {
                    case FTN.Commands.Close:
                        pom.Add((short)Commands.CLOSE);
                        break;
                    case FTN.Commands.Open:
                        pom.Add((short)Commands.OPEN);
                        break; 
                    default:
                        break;
                }
            }
            return pom;
        }
        public static List<short> GetDMSStates (List<FTN.States> state)
        {
            List<short> pom = new List<short>();
            foreach (var item in state)
            {
                switch (item)
                {
                    case FTN.States.Closed:
                        pom.Add((short)States.CLOSED);
                        break;
                    case FTN.States.Opened:
                        pom.Add((short)States.OPENED);
                        break;
                    default:
                        break;
                }
            }
            return pom;
        }
        #endregion Enums convert
    }
}
