using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using log4net;
using BiBsps.BiGlobalFiies;
using BiBsps.BiLog;
using BiBsps.BiProtocols.TosaBosa;

namespace BiBspsUnitTest
{
    [TestClass, Ignore]
    public class UnitTestProtocol
    {
        private readonly ILog _log = LogManager.GetLogger("BiBspLocalLog");

        private BaseProtocol CreateProtocolObj()
        {
            BaseProtocol protocols = new ProtocolTosa32G(3, new Comm("COM16"), new DriverLog(_log, "", ""));
            protocols.SelectProductType();
            return protocols;
        }

        [TestMethod]
        public void Test_SelectProductType()
        {
            var obj = CreateProtocolObj();

            obj.SelectProductType();
        }

        [TestMethod]
        public void Test_InitBurnInMode()
        {
            var obj = CreateProtocolObj();
            obj.InitBurnInMode();
        }

        [TestMethod]
        public void Test_EnableVoltage()
        {
            var ls = new List<int>();
            for (var i = 1; i <= 16; i++)
                ls.Add(i);
            BaseProtocol obj = CreateProtocolObj();
            obj.EnableVoltage(ls, true);
            obj.EnableVoltage(ls, false);
        }

        [TestMethod]
        public void Test_EnableLaser()
        {
            var ls = new List<int>();
            for (var i = 1; i <= 16; i++)
                ls.Add(i);
            var obj = CreateProtocolObj();
            obj.EnableLaser(ls, true);
            obj.EnableLaser(ls, false);
        }

        [TestMethod]
        public void Test_EnableBiasSync()
        {
            var obj = CreateProtocolObj();
            obj.EnableBiasSync(true);
            obj.EnableBiasSync(false);
        }

        [TestMethod]
        public void Test_SetSyncBias()
        {
            CreateProtocolObj().SetSyncBias(30);
            CreateProtocolObj().SetSyncBias(0);
        }

        [TestMethod]
        public void Test_SetSyncBiasByChannel()
        {
            CreateProtocolObj().SetSyncBiasByChannel(1, 30);
            CreateProtocolObj().SetSyncBiasByChannel(2, 30);
        }

        [TestMethod]
        public void Test_SelectProduct()
        {
            CreateProtocolObj().SelectProduct(1);
        }

        [TestMethod]
        public void Test_SetSingleBiasByChannel()
        {
            CreateProtocolObj().SetSingleBiasByChannel(1, 30);
        }

        [TestMethod]
        public void Test_SetSingleBias()
        {
            CreateProtocolObj().SetSingleBias(1, 30);
            CreateProtocolObj().SetSingleBias(1, 0);
        }

        [TestMethod]
        public void Test_ReadIccDac()
        {
            CreateProtocolObj().ReadIccDac();
        }

        [TestMethod]
        public void Test_ReadItecDac()
        {
            CreateProtocolObj().ReadItecDac();
        }

        [TestMethod]
        public void Test_ReadCurrentDacAndMpdDac()
        {
            int[] currentDac;
            int[] mpdDac;
            CreateProtocolObj().ReadCurrentDacAndMpdDac(out currentDac, out mpdDac);
        }

        [TestMethod]
        public void Test_ReadTecTemperatureDac()
        {
            int[] tecDac;
            int t1Dac;
            int t2Dac;
            CreateProtocolObj().ReadTecTemperatureDac(out tecDac, out t1Dac, out t2Dac);
        }


        [TestMethod]
        public void Test_SetTecTemperatureDac()
        {
            // CreateProtocolObj().SetTecTemperatureDac();
        }

        [TestMethod]
        public void Test_GetVoltageState()
        {
            var ls = new List<int>();
            for (var i = 1; i <= 16; i++)
                ls.Add(i);
            var obj = CreateProtocolObj();
            obj.EnableVoltage(ls, true);
            var res = CreateProtocolObj().GetVoltageState();
            obj.EnableVoltage(ls, true);
            for (int i = 0; i <= 15; i++)
            {
                Assert.AreEqual(res[i], true);
            }
        }

        [TestMethod]
        public void Test_CloseTecTemperature()
        {
            CreateProtocolObj().CloseTecTemperature();
        }
    }
}
