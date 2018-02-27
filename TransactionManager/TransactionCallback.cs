using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionManagerContract;

namespace TransactionManager
{
    public class TransactionCallback : ITransactionCallback
    {
        private TransactionAnswer answerForEnlist;
        private TransactionAnswer answerForPrepare;

        public TransactionCallback()
        {
            AnswerForEnlist = TransactionAnswer.Unanswered;
            answerForPrepare = TransactionAnswer.Unanswered;                                                        
        }

        public TransactionAnswer AnswerForEnlist { get => answerForEnlist; set => answerForEnlist = value; }
        public TransactionAnswer AnswerForPrepare { get => answerForPrepare; set => answerForPrepare = value; }

        public void CallbackEnlist(bool prepare)
        {
            AnswerForEnlist = prepare ? TransactionAnswer.Prepared : TransactionAnswer.Unprepared;
            Console.WriteLine("Vratio za enlist");
        }

        public void CallbackPrepare(bool prepare)
        {
            AnswerForPrepare = prepare ? TransactionAnswer.Prepared : TransactionAnswer.Unprepared;
            Console.WriteLine("Odogovrio je: " + prepare);
        }

        public void CallbackCommit(string commit)
        {
            Console.WriteLine(commit);
        }

        public void CallbackRollback(string rollback)
        {
            Console.WriteLine(rollback);
        }
    }
}
