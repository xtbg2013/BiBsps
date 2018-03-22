using BiBsps.BiData;
using BiBsps.BiInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiBsps.BiProtocols.Parallel;

namespace BiBspsUnitTest
{
    [TestClass]// Ignore
    public class UnitTestParallelProtocolQsfp28G
    {
        private readonly int _floor;
        private readonly int _location;
        private readonly int _seat;

        private static IParallelProtocol CreateParallelProtocol()
        {
            var vbmsAddr = VbmsDriverAddr.Inst();
            var cmd = VbmsCmd.Inst();
            return ParallelProtocolQsfp28G.Inst(vbmsAddr, cmd);
        }

        public UnitTestParallelProtocolQsfp28G()
        {
            _floor = 1;
            _location = 6;
            _seat = 13;
        }

        [TestMethod]
        public void TestConstructProtocol()
        {
            var protocol = CreateParallelProtocol();
            Assert.AreNotEqual(protocol, null);
        }

        [TestMethod]
        public void TestSelectDut()
        {
            string msg;
            var protocol = CreateParallelProtocol();
            Assert.AreNotEqual(protocol, null);
            var res = protocol.SelectDut(_floor, _location, _seat, out msg);
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void TestEnableVoltage()
        {
            string msg;
            var protocol = CreateParallelProtocol();
            Assert.AreNotEqual(protocol, null);
            var res = protocol.SelectDut(_floor, _location, _seat, out msg);
            Assert.AreEqual(true, res);
            res = protocol.EnableVoltage(_seat, true, out msg);
            Assert.AreEqual(true, res);
        }


        [TestMethod]
        public void TestSendPassWord()
        {
            string msg;
            var protocol = CreateParallelProtocol();
            Assert.AreNotEqual(protocol, null);
            var res = protocol.SelectDut(_floor, _location, _seat, out msg);
            Assert.AreEqual(true, res);
            res = protocol.EnableVoltage(_seat, true, out msg);
            Assert.AreEqual(true, res);
            res = protocol.SendPassword(out msg);
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void TestReadSn()
        {
            string msg;
            var protocol = CreateParallelProtocol();
            Assert.AreNotEqual(protocol, null);
            var res = protocol.SelectDut(_floor, _location, _seat, out msg);
            Assert.AreEqual(true, res);
            res = protocol.EnableVoltage(_seat, true, out msg);
            Assert.AreEqual(true, res);
            byte[] data;
            res = protocol.ReadParam(CmdType.ReadSn, out data, out msg);
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void TestReadBias()
        {
            string msg;
            var protocol = CreateParallelProtocol();
            Assert.AreNotEqual(protocol, null);
            var res = protocol.SelectDut(_floor, _location, _seat, out msg);
            Assert.AreEqual(true, res);
            res = protocol.EnableVoltage(_seat, true, out msg);
            Assert.AreEqual(true, res);
            byte[] data;
            res = protocol.ReadParam(CmdType.ReadBias0, out data, out msg);
            Assert.AreEqual(true, res);
            res = protocol.ReadParam(CmdType.ReadBias1, out data, out msg);
            Assert.AreEqual(true, res);
            res = protocol.ReadParam(CmdType.ReadBias2, out data, out msg);
            Assert.AreEqual(true, res);
            res = protocol.ReadParam(CmdType.ReadBias3, out data, out msg);
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void TestReadTemperature()
        {
            string msg;
            var protocol = CreateParallelProtocol();
            Assert.AreNotEqual(protocol, null);
            var res = protocol.SelectDut(_floor, _location, _seat, out msg);
            Assert.AreEqual(true, res);
            res = protocol.EnableVoltage(_seat, true, out msg);
            Assert.AreEqual(true, res);
            byte[] data;
            res = protocol.ReadParam(CmdType.ReadTemperature, out data, out msg);
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void TestReadReadMode()
        {
            string msg;
            var protocol = CreateParallelProtocol();
            Assert.AreNotEqual(protocol, null);
            var res = protocol.SelectDut(_floor, _location, _seat, out msg);
            Assert.AreEqual(true, res);
            res = protocol.EnableVoltage(_seat, true, out msg);
            Assert.AreEqual(true, res);
            byte[] data;
            res = protocol.ReadParam(CmdType.ReadMode, out data, out msg);
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void TestWriteMode()
        {
            string msg;
            var protocol = CreateParallelProtocol();
            Assert.AreNotEqual(protocol, null);
            var res = protocol.SelectDut(_floor, _location, _seat, out msg);
            Assert.AreEqual(true, res);
            res = protocol.EnableVoltage(_seat, true, out msg);
            Assert.AreEqual(true, res);
            res = protocol.WriteParam(CmdType.WriteBurninMode, out msg);
            Assert.AreEqual(true, res);
            res = protocol.WriteParam(CmdType.WriteEngineerMode, out msg);
            Assert.AreEqual(true, res);
            res = protocol.WriteParam(CmdType.WritePeralignMode, out msg);
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void TestWriteBias()
        {
            string msg;
            var protocol = CreateParallelProtocol();
            Assert.AreNotEqual(protocol, null);
            var res = protocol.SelectDut(_floor, _location, _seat, out msg);
            Assert.AreEqual(true, res);
            res = protocol.EnableVoltage(_seat, true, out msg);
            Assert.AreEqual(true, res);
            var data = new byte[] {0x00, 0x01};
            res = protocol.WriteParam(CmdType.WriteChanBiasManualMode0, out msg, data);
            Assert.AreEqual(true, res);
            res = protocol.WriteParam(CmdType.WriteChanModulaitonMod0, out msg, data);
            Assert.AreEqual(true, res);
            res = protocol.WriteParam(CmdType.WriteChanModulaitonManualMode1, out msg, data);
            Assert.AreEqual(true, res);
            res = protocol.WriteParam(CmdType.WriteChanModulaitonMod1, out msg, data);
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void TestDisableVoltage()
        {
            string msg;
            var protocol = CreateParallelProtocol();
            Assert.AreNotEqual(protocol, null);
            var res = protocol.SelectDut(_floor, _location, _seat, out msg);
            Assert.AreEqual(true, res);
            res = protocol.EnableVoltage(_seat, false, out msg);
            Assert.AreEqual(true, res);
        }
    }
}
