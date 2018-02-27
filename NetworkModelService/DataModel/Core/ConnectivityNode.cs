using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class ConnectivityNode : IdentifiedObject
    {

        private List<long> terminals = new List<long>();

        private long connectivityNodeContainer = 0;


        public ConnectivityNode(long globalId) :
                base(globalId)
        {
        }

        public List<long> Terminals
        {
            get
            {
                return this.terminals;
            }
            set
            {
                this.terminals = value;
            }
        }

        public long Connectivitynodecontainer
        {
            get
            {
                return this.connectivityNodeContainer;
            }
            set
            {
                this.connectivityNodeContainer = value;
            }
        }


        public override bool IsReferenced
        {
            get
            {
                return
                     (Terminals.Count > 0) ||
                base.IsReferenced;
            }
        }

        public override bool Equals(object obj)
        {
            if ((true && base.Equals(obj)))
            {
                ConnectivityNode x = (ConnectivityNode)obj;
                return (
                (CompareHelper.CompareLists(x.Terminals , this.Terminals, true)) &&
                (x.connectivityNodeContainer == this.connectivityNodeContainer));
                
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.CONNECTNODE_TERMINALS:
                    property.SetValue(Terminals);
                    break;
                case ModelCode.CONNECTNODE_CONNECTNODECONT:
                    property.SetValue(connectivityNodeContainer);
                    break;
                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.CONNECTNODE_TERMINALS:
                case ModelCode.CONNECTNODE_CONNECTNODECONT:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.CONNECTNODE_CONNECTNODECONT:
                    Connectivitynodecontainer = property.AsReference();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (Terminals != null && Terminals.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONNECTNODE_TERMINALS] = Terminals.GetRange(0, Terminals.Count);
            }
            if (Connectivitynodecontainer != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONNECTNODE_CONNECTNODECONT] = new List<long>();
                references[ModelCode.CONNECTNODE_CONNECTNODECONT].Add(Connectivitynodecontainer);
            }
            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_CONNECTNODE:
                    Terminals.Add(globalId);
                    break;
                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_CONNECTNODE:
                    if (Terminals.Contains(globalId))
                    {
                        Terminals.Remove(globalId);
                    }
                    else
                    {
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity(GID = 0x{ 0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
                    }
                    break;
                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }
    }
}