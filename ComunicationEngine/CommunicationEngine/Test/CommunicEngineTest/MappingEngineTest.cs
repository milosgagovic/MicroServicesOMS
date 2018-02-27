using CommunicationEngine;
using FTN.Common;
using NSubstitute;
using NUnit.Framework;
using OMSSCADACommon.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicEngineTest
{
    [TestFixture]
    public class MappingEngineTest
    {
        [Test]
        public void ReturnInstanceTest()
        {
            Assert.AreEqual(MappingEngine.Instance.getInstanceForTest(), MappingEngine.Instance);
        }

        [Test]
        public void TestCommandMethod()
        {
            ReadAll comm = new ReadAll();
            Assert.AreEqual(MappingEngine.Instance.MappCommand(TypeOfSCADACommand.ReadAll).GetType(), comm.GetType());
        }

        [Test]
        public void TestReadAllMethod()
        {

            //ReadAll comm = new ReadAll();
            //Assert.AreEqual(MappingEngine.Instance.MappCommand(TypeOfSCADACommand.ReadAll).GetType(), comm.GetType());
        }
    }
}
