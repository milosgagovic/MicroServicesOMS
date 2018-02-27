using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSContract
{
    public class IncidentReport : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int id;
        private DateTime time;
        private string mrID;
        private float lostPower;
        private IncidentState incidentState;
        private bool crewSent;
        private TimeSpan repairTime;
        private ReasonForIncident reason;
        private CrewType type;
        private Crew investigationCrew;
        private Crew repairCrew;
        private double maxValue;
        private double currentValue;

        public IncidentReport()
        {
            Time = DateTime.UtcNow;
            Time = Time.AddTicks(-(Time.Ticks % TimeSpan.TicksPerSecond));
            CrewSent = false;
            RepairTime = new TimeSpan();
            LostPower = 0;
        }

        [Key]
        public int Id { get => id; set => id = value; }
        public DateTime Time { get => time; set { time = value; } }
        public string MrID { get => mrID; set => mrID = value; }
        public float LostPower { get => lostPower; set => lostPower = value; }
        public IncidentState IncidentState { get => incidentState; set { incidentState = value; RaisePropertyChanged("IncidentState"); } }
        public bool CrewSent { get => crewSent; set { crewSent = value; RaisePropertyChanged("CrewSent"); } }
        public TimeSpan RepairTime { get => repairTime; set { repairTime = value; RaisePropertyChanged("RepairTime"); } }
        public ReasonForIncident Reason { get => reason; set { reason = value; RaisePropertyChanged("Reason"); } }
        public CrewType Crewtype { get => type; set { type = value; RaisePropertyChanged("Crewtype"); } }
        public Crew InvestigationCrew { get => investigationCrew; set { investigationCrew = value; RaisePropertyChanged("InvestigationCrew"); } }
        public Crew RepairCrew { get => repairCrew; set { repairCrew = value; RaisePropertyChanged("RepairCrew"); } }
        public double MaxValue { get => maxValue; set { maxValue = value; RaisePropertyChanged("MaxValue"); } }
        public double CurrentValue { get => currentValue; set { currentValue = value; RaisePropertyChanged("CurrentValue"); } }

        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
