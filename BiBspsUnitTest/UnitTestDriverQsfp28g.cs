using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using log4net;
using Moq;
using BiBsps.BiInterface;
using BiBsps.BiDrivers.Parallel;

namespace BiBspsUnitTest
{
    [TestClass]
    public class UnitTestDriverQsfp28G
    {
        private readonly int _seat;

        private static ILog MockIlog()
        {
            var protocol = new Mock<ILog>();
            return protocol.Object;
        }

        private static Dictionary<string, string> CreateParam
        {
            get
            {
                var param = new Dictionary<string, string>
                {
                    ["Name"] = "A1",
                    ["Floor"] = "1",
                    ["Locate"] = "1",
                    ["SEATS_COUNT"] = "26",
                    ["SET_BIAS_STEP"] = "2",
                    ["TARGET_BIAS"] = "13",
                    ["SET_BIAS_RANGE"] = "0,20",
                    ["MES_STEP"] = "PARALLEL_QSFP28G",
                    ["MES_CHECK"] = "FALSE",
                    ["HOLD_FLAG"] = "FALSE"
                };
                return param;
            }
        }

        public UnitTestDriverQsfp28G()
        {
            _seat = 13;
        }

        [TestMethod]
        public void TestConstructDriver()
        {
            var log = MockIlog();
            var protocol = new Mock<IParallelProtocol>();
            var parallelProtocol = protocol.Object;
            var param = CreateParam;
            var board = new ParallelQsfp28(param, parallelProtocol, log);
            Assert.AreNotEqual(board, null);
        }

        [TestMethod]
        public void TestVersion()
        {
            var log = MockIlog();
            var protocol = new Mock<IParallelProtocol>();
            var parallelProtocol = protocol.Object;
            var param = CreateParam;
            var board = new ParallelQsfp28(param, parallelProtocol, log);
            var vsn = board.GetVersion();
            Assert.AreEqual(vsn, "V1.00");
        }

        [TestMethod]
        public void TestAddSeat()
        {
            var param = CreateParam;
            var log = MockIlog();
            var protocol = new Mock<IParallelProtocol>();
            var parallelProtocol = protocol.Object;
            var board = new ParallelQsfp28(param, parallelProtocol, log);
            board.AddSeat(1, "A");
            board.AddSeat(2, "A");
            board.AddSeat(17, "B");
        }

        [TestMethod]
        public void TestCatchExecption()
        {
            string msg;
            var param = CreateParam;
            var log = MockIlog();
            var protocol = new Mock<IParallelProtocol>();
            var floorNum = int.Parse(param["Floor"]);
            var boardNum = int.Parse(param["Locate"]);

            var parallelProtocol = protocol.Object;
            var board = new ParallelQsfp28(param, parallelProtocol, log);
            protocol.Setup(m => m.SelectDut(floorNum, boardNum, _seat, out msg)).Returns(true);
            protocol.Setup(m => m.SendPassword(out msg)).Returns(true);
            board.CatchException(_seat);


            protocol.Setup(m => m.SelectDut(floorNum, boardNum, _seat, out msg)).Returns(false);
            protocol.Setup(m => m.SendPassword(out msg)).Returns(true);
            board.CatchException(_seat);

            protocol.Setup(m => m.SelectDut(floorNum, boardNum, _seat, out msg)).Returns(true);
            protocol.Setup(m => m.SendPassword(out msg)).Returns(false);
            board.CatchException(_seat);
        }

        [TestMethod]
        public void TestGetSnSet()
        {
            var msg = "ok";

            var param = CreateParam;
            var log = MockIlog();
            var protocol = new Mock<IParallelProtocol>();
            var floorNum = int.Parse(param["Floor"]);
            var boardNum = int.Parse(param["Locate"]);

            var parallelProtocol = protocol.Object;
            var board = new ParallelQsfp28(param, parallelProtocol, log);
            protocol.Setup(m => m.SelectDut(floorNum, boardNum, _seat, out msg)).Returns(true);
            protocol.Setup(m => m.EnableVoltage(_seat, true, out msg)).Returns(true);

            var data = Encoding.Default.GetBytes("ABCDEFG");
            protocol.Setup(m => m.ReadParam(CmdType.ReadSn, out data, out msg)).Returns(true);
            var snSet = board.GetSnSet();
            Assert.AreEqual(true, (snSet.Count == 1));
            Assert.AreEqual(snSet[_seat], "ABCDEFG");

            snSet.Clear();
            data = Encoding.Default.GetBytes("       ");
            protocol.Setup(m => m.ReadParam(CmdType.ReadSn, out data, out msg)).Returns(true);
            snSet = board.GetSnSet();
            Assert.AreEqual(true, (snSet.Count == 0));


            data = Encoding.Default.GetBytes("ABCDEFG");
            snSet.Clear();
            protocol.Setup(m => m.SelectDut(floorNum, boardNum, _seat, out msg)).Returns(false);
            protocol.Setup(m => m.ReadParam(CmdType.ReadSn, out data, out msg)).Returns(true);
            snSet = board.GetSnSet();
            Assert.AreEqual(true, (snSet.Count == 0));

            snSet.Clear();
            protocol.Setup(m => m.SelectDut(floorNum, boardNum, _seat, out msg)).Returns(true);
            protocol.Setup(m => m.EnableVoltage(_seat, true, out msg)).Returns(false);
            protocol.Setup(m => m.ReadParam(CmdType.ReadSn, out data, out msg)).Returns(true);
            Assert.AreEqual(true, (snSet.Count == 0));


            snSet.Clear();
            protocol.Setup(m => m.SelectDut(floorNum, boardNum, _seat, out msg)).Returns(true);
            protocol.Setup(m => m.EnableVoltage(_seat, true, out msg)).Returns(true);
            protocol.Setup(m => m.ReadParam(CmdType.ReadSn, out data, out msg)).Returns(false);
            Assert.AreEqual(true, (snSet.Count == 0));
        }

        [TestMethod]
        public void TestCheckCOnnections()
        {
            var param = CreateParam;
            var log = MockIlog();
            var protocol = new Mock<IParallelProtocol>();
            var parallelProtocol = protocol.Object;
            var board = new ParallelQsfp28(param, parallelProtocol, log);
            var connections = board.CheckConnections();
            foreach (var con in connections)
            {
                Assert.AreEqual(true, con);
            }
        }

        [TestMethod]
        public void TestDisableBoard()
        {
            var msg = "ok";

            var param = CreateParam;
            var log = MockIlog();
            var protocol = new Mock<IParallelProtocol>();
            var floorNum = int.Parse(param["Floor"]);
            var boardNum = int.Parse(param["Locate"]);

            var parallelProtocol = protocol.Object;
            var board = new ParallelQsfp28(param, parallelProtocol, log);
            protocol.Setup(m => m.SelectDut(floorNum, boardNum, _seat, out msg)).Returns(true);
            protocol.Setup(m => m.EnableVoltage(_seat, true, out msg)).Returns(true);

            var data = Encoding.Default.GetBytes("ABCDEFG");
            protocol.Setup(m => m.ReadParam(CmdType.ReadSn, out data, out msg)).Returns(true);
            var snSet = board.GetSnSet();
            Assert.AreEqual(true, (snSet.Count == 1));
            Assert.AreEqual(snSet[_seat], "ABCDEFG");

            snSet.Clear();
            data = Encoding.Default.GetBytes("       ");
            protocol.Setup(m => m.ReadParam(CmdType.ReadSn, out data, out msg)).Returns(true);
            snSet = board.GetSnSet();
            Assert.AreEqual(true, (snSet.Count == 0));


            data = Encoding.Default.GetBytes("ABCDEFG");
            snSet.Clear();
            protocol.Setup(m => m.SelectDut(floorNum, boardNum, _seat, out msg)).Returns(false);
            protocol.Setup(m => m.ReadParam(CmdType.ReadSn, out data, out msg)).Returns(true);
            snSet = board.GetSnSet();
            Assert.AreEqual(true, (snSet.Count == 0));

            snSet.Clear();
            protocol.Setup(m => m.SelectDut(floorNum, boardNum, _seat, out msg)).Returns(true);
            protocol.Setup(m => m.EnableVoltage(_seat, true, out msg)).Returns(false);
            protocol.Setup(m => m.ReadParam(CmdType.ReadSn, out data, out msg)).Returns(true);
            Assert.AreEqual(true, (snSet.Count == 0));


            snSet.Clear();
            protocol.Setup(m => m.SelectDut(floorNum, boardNum, _seat, out msg)).Returns(true);
            protocol.Setup(m => m.EnableVoltage(_seat, true, out msg)).Returns(true);
            protocol.Setup(m => m.ReadParam(CmdType.ReadSn, out data, out msg)).Returns(false);
            Assert.AreEqual(true, (snSet.Count == 0));
        }
    }
}
