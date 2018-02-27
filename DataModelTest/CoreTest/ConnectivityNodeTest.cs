using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace DataModelTest.CoreTest
{

    [TestFixture]
    public class ConnectivityNodeTest : DataModelBaseTest
    {
        [OneTimeSetUp]
        public override void Init()
        {
            base.Init();
            entityGid = ModelCodeHelper.CreateGlobalId(10, (short)DMSType.CONNECTNODE, 22);
        }

        [SetUp]
        public void SetUp()
        {
            entity = new ConnectivityNode(entityGid);
        }

        [TestCase(ModelCode.CONNECTNODE_CONNECTNODECONT)]
        public override void SettableReferencePropertiesTest(ModelCode propertyId)
        {
            base.SettableReferencePropertiesTest(propertyId);
        }
    }
}
