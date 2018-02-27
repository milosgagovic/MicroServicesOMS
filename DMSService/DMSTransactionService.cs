using FTN.Common;
using PubSubscribe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransactionManagerContract;
using DMSCommon.Model;
using DMSCommon.TreeGraph;

namespace DMSService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class DMSTransactionService : ITransaction
    {
        private static Tree<Element> newTree;
        private static Tree<Element> oldTree;

        public void Enlist()
        {
            Console.WriteLine("Pozvan je enlist na DMS-u");
            oldTree = DMSService.Instance.Tree;
            ITransactionCallback callback = OperationContext.Current.GetCallbackChannel<ITransactionCallback>();
            callback.CallbackEnlist(true);
        }

        public void Commit()
        {
            Console.WriteLine("Pozvan je Commit na DMS-u");
            DMSService.Instance.Tree = newTree;
            if (DMSService.updatesCount >= 2)
            {
                Publisher publisher = new Publisher();
                List<SCADAUpdateModel> update = new List<SCADAUpdateModel>();
                Source s = (Source)DMSService.Instance.Tree.Data[DMSService.Instance.Tree.Roots[0]];
                update.Add(new SCADAUpdateModel(true, s.ElementGID));

                publisher.PublishUpdate(update);
            }


            ITransactionCallback callback = OperationContext.Current.GetCallbackChannel<ITransactionCallback>();
            callback.CallbackCommit("Uspjesno je prosao commit na DMS-u");
        }
      
        public void Prepare(Delta delta)
        {
            Console.WriteLine("Pozvan je prepare na DMS-u");

            newTree = DMSService.Instance.InitializeNetwork(delta);
            DMSService.updatesCount += 1;
            ITransactionCallback callback = OperationContext.Current.GetCallbackChannel<ITransactionCallback>();

            // i ovde puca nekad
            if (newTree.Data.Values.Count != 0)
            {
                callback.CallbackPrepare(true);
            }
            else
            {
                callback.CallbackPrepare(false);
            }
        }
        public void Rollback()
        {
            Console.WriteLine("Pozvan je RollBack na DMSu");
            newTree = null;
            DMSService.Instance.Tree = oldTree;
            ITransactionCallback callback = OperationContext.Current.GetCallbackChannel<ITransactionCallback>();
            callback.CallbackRollback("Something went wrong on DMS");
        }
    }
}