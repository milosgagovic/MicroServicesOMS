using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IMSContract
{
    public class IMSClient : ClientBase<IIMSContract>, IIMSContract
    {
        // izbrisati iz App.config fajlova client tag...
        //public IMSClient() : base("IMSEndpoint")
        //{
        //}

        public IMSClient(string endpointName) : base(new NetTcpBinding(), new EndpointAddress(endpointName))
        {
            
        }

        public IMSClient(EndpointAddress address) : base(new NetTcpBinding(), address)
        {

        }

        public bool AddCrew(Crew crew)
        {
            return Channel.AddCrew(crew);
        }

        public void AddElementStateReport(ElementStateReport report)
        {
            Channel.AddElementStateReport(report);
        }

        public void AddReport(IncidentReport report)
        {
            Channel.AddReport(report);
        }

        public List<ElementStateReport> GetAllElementStateReports()
        {
            return Channel.GetAllElementStateReports();
        }

        public List<IncidentReport> GetAllReports()
        {
            return Channel.GetAllReports();
        }

        public List<Crew> GetCrews()
        {
            return Channel.GetCrews();
        }

        public List<List<ElementStateReport>> GetElementStateReportsForMrID(string mrID)
        {
            return Channel.GetElementStateReportsForMrID(mrID);
        }

        public List<ElementStateReport> GetElementStateReportsForSpecificMrIDAndSpecificTimeInterval(string mrID, DateTime startTime, DateTime endTime)
        {
            return Channel.GetElementStateReportsForSpecificMrIDAndSpecificTimeInterval(mrID, startTime, endTime);
        }

        public List<ElementStateReport> GetElementStateReportsForSpecificTimeInterval(DateTime startTime, DateTime endTime)
        {
            return Channel.GetElementStateReportsForSpecificTimeInterval(startTime, endTime);
        }

        public IncidentReport GetReport(DateTime id)
        {
            return Channel.GetReport(id);
        }

        public List<List<IncidentReport>> GetReportsForMrID(string mrID)
        {
            return Channel.GetReportsForMrID(mrID);
        }

        public List<IncidentReport> GetReportsForSpecificMrIDAndSpecificTimeInterval(string mrID, DateTime startTime, DateTime endTime)
        {
            return Channel.GetReportsForSpecificMrIDAndSpecificTimeInterval(mrID, startTime, endTime);
        }

        public List<IncidentReport> GetReportsForSpecificTimeInterval(DateTime startTime, DateTime endTime)
        {
            return Channel.GetReportsForSpecificTimeInterval(startTime, endTime);
        }

        public List<List<IncidentReport>> GetReportsForSpecificDateSortByBreaker(List<string> mrids, DateTime date)
        {
            return Channel.GetReportsForSpecificDateSortByBreaker(mrids, date);
        }

        public List<List<IncidentReport>> GetAllReportsSortByBreaker(List<string> mrids)
        {
            return Channel.GetAllReportsSortByBreaker(mrids);
        }

        public bool Ping()
        {
            return Channel.Ping();
        }

        public void UpdateReport(IncidentReport report)
        {
            Channel.UpdateReport(report);
        }
    }
}
