using IMSContract;
using IncidentManagementSystem.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncidentManagementSystem.Service
{
    public class IMSService : IIMSContract
    {
        public bool Ping()
        {
            return true;
        }

        public bool AddCrew(Crew crew)
        {
            using (var ctx = new IncidentCloudContext())
            {
                try
                {
                    ctx.Crews.Add(crew);

                    foreach (Crew c in ctx.Crews)
                    {
                        Console.WriteLine("Added crew: " + c.CrewName + ", crew id: " + c.Id);
                    }

                    ctx.SaveChanges();

                    using (var ctxCloud = new IncidentCloudContext())
                    {
                        try
                        {
                            ctxCloud.Crews.Add(crew);
                            foreach (Crew c in ctx.Crews)
                            {
                                Console.WriteLine("Added crew: " + c.CrewName + ", crew id: " + c.Id);
                            }
                            ctxCloud.SaveChanges();
                            return true;
                        }
                        catch (Exception e)
                        {
                            return false;
                        }
                    }

                }
                catch (Exception e)
                {
                    return false;
                }
            }

        }

        public void AddElementStateReport(ElementStateReport report)
        {
            using (var ctxCloud = new IncidentCloudContext())
            {
                ctxCloud.ElementStateReports.Add(report);
                ctxCloud.SaveChanges();
                Console.WriteLine("Upisano:\n MRID: " + report.MrID + ", Date Time: " + report.Time + ", State: " + report.State);
            }
        }

        public void AddReport(IncidentReport report)
        {
            using (var ctxCloud = new IncidentCloudContext())
            {
                ctxCloud.IncidentReports.Add(report);
                ctxCloud.SaveChanges();
            }
        }

        public List<ElementStateReport> GetAllElementStateReports()
        {
            List<ElementStateReport> retVal = new List<ElementStateReport>();
            using (var ctxCloud = new IncidentCloudContext())
            {
                foreach (ElementStateReport ir in ctxCloud.ElementStateReports)
                {
                    retVal.Add(ir);
                }
            }
            return retVal;
        }

        public List<IncidentReport> GetAllReports()
        {
            List<IncidentReport> retVal = new List<IncidentReport>();
            using (var ctxCloud = new IncidentCloudContext())
            {
                foreach (IncidentReport ir in ctxCloud.IncidentReports.Include("InvestigationCrew").Include("RepairCrew"))
                {
                    retVal.Add(ir);
                }
            }
            return retVal;
        }

        public List<Crew> GetCrews()
        {
            List<Crew> retVal = new List<Crew>();
            using (var ctxCloud = new IncidentCloudContext())
            {
                ctxCloud.Crews.ToList().ForEach(u => retVal.Add(u));
            }
            return retVal;
        }

        public List<List<ElementStateReport>> GetElementStateReportsForMrID(string mrID)
        {
            List<ElementStateReport> temp = new List<ElementStateReport>();
            Dictionary<string, List<ElementStateReport>> reportsByBreaker = new Dictionary<string, List<ElementStateReport>>();
            List<List<ElementStateReport>> retVal = new List<List<ElementStateReport>>();

            using (var ctxCloud = new IncidentCloudContext())
            {
                ctxCloud.IncidentReports.ToList();

                foreach (ElementStateReport report in ctxCloud.ElementStateReports.ToList())
                {
                    if (report.MrID == mrID)
                    {
                        temp.Add(report);
                    }
                }
            }

            foreach (ElementStateReport report in temp)
            {
                string key = report.Time.ToString();

                if (!reportsByBreaker.ContainsKey(key))
                {
                    reportsByBreaker.Add(key, new List<ElementStateReport>());
                }

                reportsByBreaker[key].Add(report);
            }

            int i = 0;
            foreach (List<ElementStateReport> reports in reportsByBreaker.Values)
            {
                retVal.Add(new List<ElementStateReport>());
                retVal[i++] = reports;
            }

            return retVal;
        }

        public List<ElementStateReport> GetElementStateReportsForSpecificMrIDAndSpecificTimeInterval(string mrID, DateTime startTime, DateTime endTime)
        {
            List<ElementStateReport> retVal = new List<ElementStateReport>();
            using (var ctxClud = new IncidentCloudContext())
            {
                ctxClud.ElementStateReports.Where(u => u.MrID == mrID && u.Time > startTime && u.Time < endTime).ToList().ForEach(x => retVal.Add(x));
            }
            return retVal;
        }

        public List<ElementStateReport> GetElementStateReportsForSpecificTimeInterval(DateTime startTime, DateTime endTime)
        {

            List<ElementStateReport> retVal = new List<ElementStateReport>();
            using (var ctxCloud = new IncidentCloudContext())
            {
                ctxCloud.ElementStateReports.Where(u => u.Time > startTime && u.Time < endTime).ToList().ForEach(x => retVal.Add(x));
            }
            return retVal;
        }

        public IncidentReport GetReport(DateTime id)
        {
            List<IncidentReport> retVal = new List<IncidentReport>();

            using (var ctxCloud = new IncidentCloudContext())
            {
                foreach (IncidentReport ir in ctxCloud.IncidentReports)
                {
                    retVal.Add(ir);
                }
            }

            IncidentReport res = null;
            foreach (IncidentReport report in retVal)
            {
                if (DateTime.Compare(report.Time, id) == 0)
                {
                    res = report;
                    break;
                }
            }

            using (var ctxCloud = new IncidentCloudContext())
            {
                res = ctxCloud.IncidentReports.Where(ir => ir.Id == res.Id).Include("InvestigationCrew").FirstOrDefault();
            }
            return res;
        }

        public List<List<IncidentReport>> GetReportsForMrID(string mrID)
        {
            List<IncidentReport> temp = new List<IncidentReport>();
            Dictionary<string, List<IncidentReport>> reportsByBreaker = new Dictionary<string, List<IncidentReport>>();
            List<List<IncidentReport>> retVal = new List<List<IncidentReport>>();

            using (var ctxCloud = new IncidentCloudContext())
            {
                ctxCloud.IncidentReports.ToList();

                foreach (IncidentReport report in ctxCloud.IncidentReports.ToList())
                {
                    if (report.MrID == mrID)
                    {
                        temp.Add(report);
                    }
                }
            }

            foreach (IncidentReport report in temp)
            {
                string key = report.Time.Day + "/" + report.Time.Month + "/" + report.Time.Year;

                if (!reportsByBreaker.ContainsKey(key))
                {
                    reportsByBreaker.Add(key, new List<IncidentReport>());
                }

                reportsByBreaker[key].Add(report);
            }

            int i = 0;
            foreach (List<IncidentReport> reports in reportsByBreaker.Values)
            {
                retVal.Add(new List<IncidentReport>());
                retVal[i++] = reports;
            }

            return retVal;
        }

        public List<IncidentReport> GetReportsForSpecificMrIDAndSpecificTimeInterval(string mrID, DateTime startTime, DateTime endTime)
        {
            List<IncidentReport> retVal = new List<IncidentReport>();
            using (var ctxCloud = new IncidentCloudContext())
            {
                ctxCloud.IncidentReports.Where(u => u.MrID == mrID && u.Time > startTime && u.Time < endTime).ToList().ForEach(x => retVal.Add(x));
            }
            return retVal;
        }

        public List<IncidentReport> GetReportsForSpecificTimeInterval(DateTime startTime, DateTime endTime)
        {
            List<IncidentReport> retVal = new List<IncidentReport>();
            using (var ctxCloud = new IncidentCloudContext())
            {
                ctxCloud.IncidentReports.Where(u => u.Time > startTime && u.Time < endTime).ToList().ForEach(x => retVal.Add(x));
            }
            return retVal;
        }

        public void UpdateReport(IncidentReport report)
        {
            List<IncidentReport> list = new List<IncidentReport>();
            using (var ctx = new IncidentCloudContext())
            {
                foreach (IncidentReport ir in ctx.IncidentReports)
                {
                    list.Add(ir);
                }

                int i = 0;
                for (i = 0; i < list.Count; i++)
                {
                    if (DateTime.Compare(list[i].Time, report.Time) == 0)
                    {
                        i = list[i].Id;
                        break;
                    }
                }

                var res = ctx.IncidentReports.Where(r => r.Id == i).FirstOrDefault();
                res.Reason = report.Reason;
                res.RepairTime = report.RepairTime;
                res.CrewSent = report.CrewSent;
                res.Crewtype = report.Crewtype;
                res.IncidentState = report.IncidentState;
                res.LostPower = report.LostPower;
                try { res.InvestigationCrew = ctx.Crews.Where(c => c.Id == report.InvestigationCrew.Id).FirstOrDefault(); } catch { }
                try { res.RepairCrew = ctx.Crews.Where(c => c.Id == report.RepairCrew.Id).FirstOrDefault(); } catch { }

                ctx.SaveChanges();
            }
        }

        public List<List<IncidentReport>> GetReportsForSpecificDateSortByBreaker(List<string> mrids, DateTime date)
        {
            List<IncidentReport> temp = new List<IncidentReport>();
            Dictionary<string, List<IncidentReport>> reportsByBreaker = new Dictionary<string, List<IncidentReport>>();
            List<List<IncidentReport>> retVal = new List<List<IncidentReport>>();

            foreach (string mrid in mrids)
            {
                reportsByBreaker.Add(mrid, new List<IncidentReport>());
            }

            using (var ctxCloud = new IncidentCloudContext())
            {
                ctxCloud.IncidentReports.ToList();

                foreach (IncidentReport report in ctxCloud.IncidentReports.ToList())
                {
                    if (report.Time.Date == date)
                    {
                        temp.Add(report);
                    }
                }
            }

            foreach (IncidentReport report in temp)
            {
                if (reportsByBreaker.ContainsKey(report.MrID))
                {
                    reportsByBreaker[report.MrID].Add(report);
                }
            }

            int i = 0;
            foreach (List<IncidentReport> reports in reportsByBreaker.Values)
            {
                retVal.Add(new List<IncidentReport>());
                retVal[i++] = reports;
            }

            return retVal;
        }

        public List<List<IncidentReport>> GetAllReportsSortByBreaker(List<string> mrids)
        {
            List<IncidentReport> temp = new List<IncidentReport>();
            Dictionary<string, List<IncidentReport>> reportsByBreaker = new Dictionary<string, List<IncidentReport>>();
            List<List<IncidentReport>> retVal = new List<List<IncidentReport>>();

            foreach (string mrid in mrids)
            {
                reportsByBreaker.Add(mrid, new List<IncidentReport>());
            }

            using (var ctxCloud = new IncidentCloudContext())
            {
                temp = ctxCloud.IncidentReports.ToList();
            }

            foreach (IncidentReport report in temp)
            {
                if (reportsByBreaker.ContainsKey(report.MrID))
                {
                    reportsByBreaker[report.MrID].Add(report);
                }
            }

            int i = 0;
            foreach (List<IncidentReport> reports in reportsByBreaker.Values)
            {
                retVal.Add(new List<IncidentReport>());
                retVal[i++] = reports;
            }

            return retVal;
        }
    }
}