using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Wires;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace DataModelTest.WiresTest
{
    [TestFixture]
    public class EnergyConsumerTest : DataModelBaseTest
    {
        [OneTimeSetUp]
        public override void Init()
        {
            base.Init();
            entityGid = ModelCodeHelper.CreateGlobalId(10, (short)DMSType.ENERGCONSUMER, 22);
        }

        [SetUp]
        public void SetUp()
        {
            entity = new EnergyConsumer(entityGid);
        }

        [TestCase(ModelCode.ENERGCONSUMER_PFIXED)]
        [TestCase(ModelCode.ENERGCONSUMER_QFIXED)]
        public override void SettablePropertiesTest(ModelCode propertyId)
        {
            base.SettablePropertiesTest(propertyId);
        }
    }
}
