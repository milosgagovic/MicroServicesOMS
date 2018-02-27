using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class ConnectivityNodeContainer : PowerSystemResource
    {

        private List<long> connectivityNodes = new List<long>();
       

        public ConnectivityNodeContainer(long globalId):base(globalId)
        {
        }
       

        public List<long> ConnectivityNodes
        {
            get
            {
                return this.connectivityNodes;
            }
            set
            {
                this.connectivityNodes = value;
            }
        }


        public override bool IsReferenced
        {
            get
            {
                return
                     (ConnectivityNodes.Count > 0) ||
                base.IsReferenced;
            }
        }

        public override bool Equals(object obj)
        {
            if ((true && base.Equals(obj)))
            {
                ConnectivityNodeContainer x = (ConnectivityNodeContainer)obj;
                return ((CompareHelper.CompareLists(x.ConnectivityNodes, this.connectivityNodes, true)));
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
                case ModelCode.CONNECTNODECONT_CONNECTNODES:
                    property.SetValue(ConnectivityNodes);
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
                case ModelCode.CONNECTNODECONT_CONNECTNODES:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (ConnectivityNodes != null && ConnectivityNodes.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONNECTNODECONT_CONNECTNODES] = ConnectivityNodes.GetRange(0, ConnectivityNodes.Count);
            }
            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.CONNECTNODE_CONNECTNODECONT:
                    ConnectivityNodes.Add(globalId);
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
                case ModelCode.CONNECTNODE_CONNECTNODECONT:
                    if (ConnectivityNodes.Contains(globalId))
                    {
                        ConnectivityNodes.Remove(globalId);
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