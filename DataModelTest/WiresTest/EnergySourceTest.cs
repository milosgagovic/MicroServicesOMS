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
    public class EnergySourceTest : DataModelBaseTest
    {
        [OneTimeSetUp]
        public override void Init()
        {
            base.Init();
            entityGid = ModelCodeHelper.CreateGlobalId(10, (short)DMSType.ENERGSOURCE, 22);
        }

        [SetUp]
        public void SetUp()
        {
            entity = new EnergySource(entityGid);
        }

        [TestCase(ModelCode.ENERGSOURCE_ACTPOW)]
        [TestCase(ModelCode.ENERGSOURCE_NOMVOLT)]
        public override void SettablePropertiesTest(ModelCode propertyId)
        {
            base.SettablePropertiesTest(propertyId);
        }
    }
}
