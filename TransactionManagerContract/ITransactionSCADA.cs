using OMSSCADACommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TransactionManagerContract
{
    [ServiceContract(CallbackContract = typeof(ITransactionCallback))]
    public interface ITransactionSCADA
    {
        [OperationContract]
        void Enlist();

        [OperationContract]
        void Prepare(ScadaDelta delta);

        [OperationContract]
        void Commit();

        [OperationContract]
        void Rollback();
    }
}
