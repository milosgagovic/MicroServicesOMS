using FTN.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherApp.Model.Measurements
{
    public class Measurement : INotifyPropertyChanged
    {
        private long gID;
        private string mRID;
        private string name;
        private long psr;

        public void ReadFromResourceDescription(ResourceDescription rd)
        {
            try { this.GID = rd.GetProperty(ModelCode.IDOBJ_GID).AsLong(); } catch { }
            try { this.MRID = rd.GetProperty(ModelCode.IDOBJ_MRID).AsString(); } catch { }
            try { this.Name = rd.GetProperty(ModelCode.IDOBJ_NAME).AsString(); } catch { }
            try { this.Psr = rd.GetProperty(ModelCode.MEASUREMENT_PSR).AsLong(); } catch { }
        }

        public long GID
        {
            get
            {
                return this.gID;
            }
            set
            {
                this.gID = value;
                RaisePropertyChanged("GID");
            }
        }

        public string MRID
        {
            get
            {
                return this.mRID;
            }
            set
            {
                this.mRID = value;
                RaisePropertyChanged("MRID");
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                RaisePropertyChanged("Name");
            }
        }

        public long Psr
        {
            get
            {
                return this.psr;
            }
            set
            {
                this.psr = value;
                RaisePropertyChanged("Psr");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
