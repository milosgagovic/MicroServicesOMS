using DMSCommon.Model;
using FTN.Common;
using IMSContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TransactionManagerContract
{
    [ServiceContract]
    public interface IOMSClient
    {
        /*CIMAdapter methods*/

        /// <summary>
        /// Update system Static Data. Called by ModelLabs (CIMAdapter) when Static data changes
        /// </summary>
        /// <param name="d">Delta</param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateSystem(Delta d);

        /*DispatcherApp methods*/
        
        [OperationContract]
        TMSAnswerToClient GetNetwork();

        [OperationContract]
        void SendCommandToSCADA(TypeOfSCADACommand command, string mrid, OMSSCADACommon.CommandTypes commandtype, float value);

        [OperationContract]
        void SendCrew(IncidentReport report);

        [OperationContract]
        bool IsNetworkAvailable();

        /*unused methods :)*/

        [OperationContract]
        void GetNetworkWithOutParam(out List<Element> DMSElements, out List<ResourceDescription> resourceDescriptions, out int GraphDeep);
     
		//[OperationContract]
		//void AddReport(string mrID, DateTime time, string state);

        [OperationContract]
        void AddReport(IncidentReport report);

        [OperationContract]
		List<IncidentReport> GetAllReports();

		[OperationContract]
		List<List<IncidentReport>> GetReportsForMrID(string mrID);

		[OperationContract]
		List<IncidentReport> GetReportsForSpecificTimeInterval(DateTime startTime, DateTime endTime);

		[OperationContract]
		List<IncidentReport> GetReportsForSpecificMrIDAndSpecificTimeInterval(string mrID, DateTime startTime, DateTime endTime);

        [OperationContract]
        List<ElementStateReport> GetAllElementStateReports();

        [OperationContract]
        List<List<ElementStateReport>> GetElementStateReportsForMrID(string mrID);

        [OperationContract]
        List<ElementStateReport> GetElementStateReportsForSpecificTimeInterval(DateTime startTime, DateTime endTime);

        [OperationContract]
        List<List<IncidentReport>> GetReportsForSpecificDateSortByBreaker(List<string> mrids, DateTime date);

        [OperationContract]
        List<ElementStateReport> GetElementStateReportsForSpecificMrIDAndSpecificTimeInterval(string mrID, DateTime startTime, DateTime endTime);

        [OperationContract]
        List<List<IncidentReport>> GetAllReportsSortByBreaker(List<string> mrids);

        //[OperationContract]
        //void SendCrew(string mrid);

        [OperationContract]
        List<Crew> GetCrews();

		[OperationContract]
		bool AddCrew(Crew crew);

        [OperationContract]
        void ClearNMSDB();
    }
}
