using NUnit.Framework;
using SCADA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicEngineTest

{
    [TestFixture]
    public class SCADACommuncEngineServiceTest
    {
        private SCADACommuncEngineService serviceUnderTest;
        [OneTimeSetUp]
        public void SetupTest()
        {
            serviceUnderTest = new SCADACommuncEngineService();

        }
        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new SCADACommuncEngineService());

        }

        [Test]
        public void ConstructorTestNotNull()
        {
            Assert.NotNull(new SCADACommuncEngineService());

        }
        [Test]
        public void InvokeMeasurementsTestExp()
        {
            Assert.DoesNotThrow(() => serviceUnderTest.InvokeMeasurements());

        }
       
    }
}
