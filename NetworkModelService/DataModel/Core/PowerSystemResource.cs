using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FTN.Common;



namespace FTN.Services.NetworkModelService.DataModel.Core
{
	public class PowerSystemResource : IdentifiedObject
	{
        private List<long> measurments = new List<long>();

        public List<long> Measurments
        {
            get
            {
                return this.measurments;
            }
            set
            {
                this.measurments = value;
            }
        }

        public PowerSystemResource(long globalId)
			: base(globalId)
		{
		}	

		

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				PowerSystemResource x = (PowerSystemResource)obj;
                return ((CompareHelper.CompareLists(x.Measurments, this.Measurments, true)));
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

		#region IAccess implementation

		public override bool HasProperty(ModelCode property)
		{
			switch (property)
			{
                case ModelCode.PSR_MEASUREMENTS:
                    return true;
                default:
					return base.HasProperty(property);
			}
		}

		public override void GetProperty(Property property)
		{
			switch (property.Id)
			{
                case ModelCode.PSR_MEASUREMENTS:
                    property.SetValue(Measurments);
                    break;
                default:
					base.GetProperty(property);
					break;
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

        #endregion IAccess implementation

        #region IReference implementation
        public override bool IsReferenced
        {
            get
            {
                return
                     (Measurments.Count > 0) ||
                base.IsReferenced;
            }
        }
        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (Measurments != null && Measurments.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.PSR_MEASUREMENTS] = Measurments.GetRange(0, Measurments.Count);
            }
            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.MEASUREMENT_PSR:
                    Measurments.Add(globalId);
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
                case ModelCode.MEASUREMENT_PSR:
                    if (Measurments.Contains(globalId))
                    {
                        Measurments.Remove(globalId);
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

        #endregion IReference implementation		
    }
}
