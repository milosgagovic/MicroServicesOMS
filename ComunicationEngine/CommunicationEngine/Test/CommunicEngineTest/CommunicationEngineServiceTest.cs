using CommunicationEngine;
using CommunicationEngineContract;
using NSubstitute;
using NUnit.Framework;
using OMSSCADACommon;
using OMSSCADACommon.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicEngineTest
{
    [TestFixture]
    public class CommunicationEngineServiceTest
    {
        private CommunicationEngine.CommunicationEngine serviceUnderTest;
        [OneTimeSetUp]
        public void SetupTest()
        {
            serviceUnderTest = new CommunicationEngine.CommunicationEngine();
            CommunicationEngine.CommunicationEngine.Callback = Substitute.For<ICommuncEngineContract_CallBack>();
            CommunicationEngine.CommunicationEngine.Callback.SendCommand(Arg.Is<WriteSingleDigital>(x => x.Id == "1")).Returns(true);
            CommunicationEngine.CommunicationEngine.Callback.SendCommand(Arg.Is<WriteSingleDigital>(x => x.Id == "0")).Returns(false);
            CommunicationEngine.CommunicationEngine.Callback.SendCommand(Arg.Is<WriteSingleDigital>(x => x.Id == "-1")).Returns(x => { throw new Exception(); });
            CommunicationEngine.CommunicationEngine.Callback.InvokeMeasurements().Returns(true);

        }
        [Test]
        public void ConstructorTest()
        {
            Assert.DoesNotThrow(() => new CommunicationEngine.CommunicationEngine());
        }

        [Test]
        public void ConstructorTestNotNull()
        {
            Assert.NotNull(new CommunicationEngine.CommunicationEngine());

        }

        [Test]
        public void ReceiveValueExp()
        {
            Assert.Throws<NullReferenceException>(() => serviceUnderTest.ReceiveValue());
        }
        [Test]
        public void SendCommantTrue()
        {
            WriteSingleDigital digital = new WriteSingleDigital() { Id = "1" };
            bool result = CommunicationEngine.CommunicationEngine.Callback.SendCommand(digital);
            Assert.IsTrue(result);
        }
        [Test]
        public void SendCommantFalse()
        {
            WriteSingleDigital digital = new WriteSingleDigital() { Id = "0" };
            bool result = CommunicationEngine.CommunicationEngine.Callback.SendCommand(digital);
            Assert.IsFalse(result);
        }
        [Test]
        public void SendCommantExp()
        {
            WriteSingleDigital digital = new WriteSingleDigital() { Id = "-1" };

            Assert.Throws<Exception>(() => CommunicationEngine.CommunicationEngine.Callback.SendCommand(digital));
        }

    }
}
