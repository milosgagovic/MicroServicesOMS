using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Meas;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace DataModelTest.MeasTest
{
    [TestFixture]
    public class AnalogTest : DataModelBaseTest
    {
        [OneTimeSetUp]
        public override void Init()
        {
            base.Init();
            entityGid = ModelCodeHelper.CreateGlobalId(10, (short)DMSType.ANALOG, 22);
        }

        [SetUp]
        public void SetUp()
        {
            entity = new Analog(entityGid);
        }

        [TestCase(ModelCode.ANALOG_MAXVAL)]
        [TestCase(ModelCode.ANALOG_MINVAL)]
        [TestCase(ModelCode.ANALOG_NORMVAL)]
        [TestCase(ModelCode.MEASUREMENT_DIRECTION)]
        public override void SettablePropertiesTest(ModelCode propertyId)
        {
            base.SettablePropertiesTest(propertyId);
        }


        [TestCase(ModelCode.MEASUREMENT_PSR)]
        public override void SettableReferencePropertiesTest(ModelCode propertyId)
        {
            base.SettableReferencePropertiesTest(propertyId);
        }

    }
}
