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
    class TerminalTest : DataModelBaseTest
    {
        [OneTimeSetUp]
        public override void Init()
        {
            base.Init();
            entityGid = ModelCodeHelper.CreateGlobalId(10, (short)DMSType.TERMINAL, 22);
        }

        [SetUp]
        public void SetUp()
        {
            entity = new Terminal(entityGid);
        }

        [TestCase(ModelCode.TERMINAL_CONNECTNODE)]
        [TestCase(ModelCode.TERMINAL_CONDEQUIP)]
        public override void SettableReferencePropertiesTest(ModelCode propertyId)
        {
            base.SettableReferencePropertiesTest(propertyId);
        }

    }
}
