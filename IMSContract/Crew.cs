using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSContract
{
    public class Crew : INotifyPropertyChanged
    {
        private string id;
        private string crewName;
        private CrewType type;
        private bool working;

        public event PropertyChangedEventHandler PropertyChanged;

        [Key]
        public string Id { get => id; set => id = value; }
        public string CrewName { get => crewName; set => crewName = value; }
        public CrewType Type { get => type; set => type = value; }
        public bool Working { get => working; set { working = value; RaisePropertyChanged("Working"); } }

        public Crew()
        {
        }

        public Crew(string id, string name, ICollection<int> levels)
        {
            this.Id = id;
            this.CrewName = name;
        }

        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
