using CommunicationEngineContract;
using NSubstitute;
using NUnit.Framework;
using SCADA;
using SCADA.ClientHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommunicEngineTest
{
    [TestFixture]
    public class SCADAProxyTest
    {
        private SCADAProxy clientUnderTest;
        private InstanceContext context;
        private NetTcpBinding binding;
        private EndpointAddress adress;

        [OneTimeSetUp]
        public void SetupTest()
        {
            context = new InstanceContext( new SCADACommuncEngineService());
            adress = new EndpointAddress("net.tcp://localhost:4100/CommunEngine");
            binding = new NetTcpBinding();
            clientUnderTest = new SCADAProxy(context, binding, adress);
            clientUnderTest.Factory = Substitute.For<ICommunicationEngineContract>();

            clientUnderTest.Factory.ReceiveValue().Returns(true);
        }
        [Test]
        public void ScadaConstructorTestWithParams()
        {
            Assert.DoesNotThrow(() => new SCADAProxy(context, binding, adress));
        }
        [Test]
        public void ScadaReceiveValueOk()
        {
            bool result = clientUnderTest.Factory.ReceiveValue();
            Assert.IsTrue(result);
        }
        [Test]
        public void ScadaReceiveValueExce()
        {
            Assert.DoesNotThrow(() => clientUnderTest.ReceiveValue());
        }
        [Test]
        public void ScadaReceiveValueNotNull()
        {
            bool result = clientUnderTest.Factory.ReceiveValue();
            Assert.IsNotNull(result);
        }
    }
}
