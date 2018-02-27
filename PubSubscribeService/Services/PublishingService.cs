using DMSCommon.Model;
using IMSContract;
using PubSubContract;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PubSubscribeService.Services
{
    public class PublishingService : IPublishing
    {
        public PublishingService()
        {
        }

        public void Publish(List<SCADAUpdateModel> deltaUpdate)
        {
            foreach (IPublishing subscriber in PubSubscribeDB.Subscribers)
            {
                PublishThreadData threadObj = new PublishThreadData(subscriber, deltaUpdate);

                Thread thread = new Thread(threadObj.PublishDelta);
                thread.Start();
            }
        }
       
        public void PublishCrewUpdate(SCADAUpdateModel update)
        {
            foreach (IPublishing subscriber in PubSubscribeDB.Subscribers)
            {
                PublishThreadData threadObj = new PublishThreadData(subscriber, update);

                Thread thread = new Thread(threadObj.PublishCrewDelta);
                thread.Start();
            }
        }

        public void PublishIncident(IncidentReport report)
        {
            foreach (IPublishing subscriber in PubSubscribeDB.Subscribers)
            {
                PublishThreadData threadObj = new PublishThreadData(subscriber, report);

                Thread thread = new Thread(threadObj.PublishIncidentDelta);
                thread.Start();
            }
        }
        public void PublishCallIncident(SCADAUpdateModel call)
        {
            foreach (IPublishing subscriber in PubSubscribeDB.Subscribers)
            {
                PublishThreadData threadObj = new PublishThreadData(subscriber, call,true);

                Thread thread = new Thread(threadObj.PublishCallIncidentDelta);
                thread.Start();
            }
        }

        public void PublishUIBreakers(bool IsIncident, long incidentBreaker)
        {
            foreach (IPublishing subscriber in PubSubscribeDB.Subscribers)
            {
                PublishThreadData threadObj = new PublishThreadData(subscriber, IsIncident, incidentBreaker);

                Thread thread = new Thread(threadObj.PublishUIBreakersDelta);
                thread.Start();
            }
        }
    }

    internal class PublishThreadData
    {
        private IPublishing subscriber;

        private List<SCADAUpdateModel> deltaUpdate;
        private SCADAUpdateModel crewUpdate;
        private IncidentReport report;
        private SCADAUpdateModel call;
        private bool Isincident;
        private long incidentBreaker;

        public PublishThreadData(IPublishing subscriber, List<SCADAUpdateModel> deltaUpdate)
        {
            this.subscriber = subscriber;
            this.deltaUpdate = deltaUpdate;
        }
        public PublishThreadData(IPublishing subscriber, SCADAUpdateModel update)
        {
            this.subscriber = subscriber;
            this.crewUpdate = update;
        }
        public PublishThreadData(IPublishing subscriber, IncidentReport report)
        {
            this.subscriber = subscriber;
            this.report = report;
        }
        public PublishThreadData(IPublishing subscriber, SCADAUpdateModel call,bool isCall)
        {
            this.subscriber = subscriber;
            this.call = call;
        }
        public PublishThreadData(IPublishing subscriber, bool IsIncident, long incidentBreaker)
        {
            this.subscriber = subscriber;
            this.Isincident = IsIncident;
            this.incidentBreaker = incidentBreaker;
        }

        public IPublishing Subscriber
        {
            get
            {
                return subscriber;
            }

            set
            {
                subscriber = value;
            }
        }

        public List<SCADAUpdateModel> DeltaUpdate
        {
            get
            {
                return deltaUpdate;
            }

            set
            {
                deltaUpdate = value;
            }
        }     
        public SCADAUpdateModel CrewUpdate { get => crewUpdate; set => crewUpdate = value; }
        public IncidentReport Report { get => report; set => report = value; }
        public SCADAUpdateModel Call { get => call; set => call = value; }
        public bool IsIncident { get => Isincident; set => Isincident = value; }
        public long IncidentBreaker { get => incidentBreaker; set => incidentBreaker = value; }

        public void PublishDelta()
        {
            try
            {
                subscriber.Publish(DeltaUpdate);
            }
            catch (Exception e)
            {
                PubSubscribeDB.RemoveSubsriber(subscriber);
            }
        }
        public void PublishCrewDelta()
        {
            try
            {
                subscriber.PublishCrewUpdate(CrewUpdate);
            }
            catch (Exception e)
            {
                PubSubscribeDB.RemoveSubsriber(subscriber);
            }
        }
        public void PublishIncidentDelta()
        {
            try
            {
                subscriber.PublishIncident(Report);
            }
            catch (Exception e)
            {
                PubSubscribeDB.RemoveSubsriber(subscriber);
            }
        }
        public void PublishCallIncidentDelta()
        {
            try
            {
                subscriber.PublishCallIncident(Call);
            }
            catch (Exception e)
            {
                PubSubscribeDB.RemoveSubsriber(subscriber);
            }
        }

        public void PublishUIBreakersDelta()
        {
            try
            {
                subscriber.PublishUIBreakers(IsIncident,IncidentBreaker);
            }
            catch (Exception e)
            {
                PubSubscribeDB.RemoveSubsriber(subscriber);
            }
        }
    }
}
