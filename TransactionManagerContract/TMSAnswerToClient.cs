using DMSCommon.Model;
using FTN.Common;
using IMSContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransactionManagerContract
{
    [DataContract]
    public class TMSAnswerToClient
    {
        private List<ResourceDescription> resourceDescriptions;
        private List<ResourceDescription> resourceDescriptionsOfMeasurment;
        private List<Element> elements;
        private List<Crew> crews;
        private List<IncidentReport> incidentReports;
        private int graphDeep;

        [DataMember]
        public int GraphDeep
        {
            get { return graphDeep; }
            set { graphDeep = value; }
        }
        [DataMember]
        public List<Element> Elements
        {
            get { return elements; }
            set { elements = value; }
        }
        [DataMember]
        public List<Crew> Crews
        {
            get { return crews; }
            set { crews = value; }
        }
        [DataMember]
        public List<IncidentReport> IncidentReports
        {
            get { return incidentReports; }
            set { incidentReports = value; }
        }
        [DataMember]
        public List<ResourceDescription> ResourceDescriptions
        {
            get { return resourceDescriptions; }
            set { resourceDescriptions = value; }
        }
        [DataMember]
        public List<ResourceDescription> ResourceDescriptionsOfMeasurment
        {
            get { return resourceDescriptionsOfMeasurment; }
            set { resourceDescriptionsOfMeasurment = value; }
        }

        public TMSAnswerToClient()
        {
            Elements = new List<Element>();
            ResourceDescriptions = new List<ResourceDescription>();
            resourceDescriptionsOfMeasurment = new List<ResourceDescription>();
        }

        public TMSAnswerToClient(List<ResourceDescription> rd, List<Element> ele, int deep, List<ResourceDescription> meas, List<Crew> crews, List<IncidentReport> incidents)
        {
            GraphDeep = deep;
            ResourceDescriptions = rd;
            Elements = ele;
            ResourceDescriptionsOfMeasurment = meas;
            Crews = crews;
            IncidentReports = incidents;
        }
    }
}
