using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiBsps.Log;
using BILib;
using BIModel;
using BiBsps;
using Moq;
using BiBsps.BiDrivers;
using BiBsps.BiProtocols;
using BiBsps.BiOven;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace BiUnitTest
{
    class Test_TOSA25G : BaseDriverCase
    {
        private ILog _logger;
        private string _boardName = "A";
        private string _floorNum = "1";
        private string _boardNum = "1";
        private string _ovenPort = "1";
        private string _ctlPort = "2";
        private Dictionary<string, string> CreateParam(string boardName, string floorNum, string boardNum, string ovenPort, string ctlPort)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param["Name"] = boardName;
            param["Floor"] = floorNum;
            param["Locate"] = boardNum;
            param["OvenPort"] = ovenPort;
            param["ControlPort"] = ctlPort;
            param["BIAS_STEP"] = "20";
            param["TARGET_TEMPERATURE"] = "70";
            param["TEC_TARGET_TEMPERATURE"] = "93";
            param["CHECK_BIAS"] = "30";
            param["CHECK_ICC_SPEC"] = "40,300";
            param["CAL_BIAS_ARRAY"] = "40,60";
            param["TARGET_CURRENT"] = "80";
            param["TARGET_ADC_SPEC"] = "0,200";
            param["TEMPRATURE_RANGE"] = "5";
            param["HEAT_TIME"] = "720";
            param["READ_CURRENTK"] = "0.0002771";
            param["READ_CURRENTB"] = "0.0002238";
            param["READ_ITECK"] = "1.1615";
            param["READ_ITECB"] = "-1740.2"; 
            return param;

        }
        private BaseProtocol MockBaseProtocol()
        {
            Mock<BaseProtocol> protocol = new Mock<BaseProtocol>();
            int[] icc = new int[16];
            int[] itec = new int[16];
            int[] currentDac;
            int[] mpdDac;
            int[] tecDac = new int[16];
            int t1Dac = 30, t2Dac;
            for (int i = 0; i < 16; i++)
            {
                icc[i] = 0x148;
                tecDac[i] = 0x0830;
            }

            protocol.Setup(m => m.ReadIccDac()).Returns(icc);
            protocol.Setup(m => m.ReadItecDac()).Returns(itec);
            protocol.Setup(m => m.ReadCurrentDacAndMpdDac(out currentDac, out mpdDac));
            protocol.Setup(m => m.ReadTecTemperatureDac(out tecDac, out t1Dac, out t2Dac));

            return protocol.Object;
        }
        private BaseOvenControl MockBaseOvenControl()
        {
            Mock<BaseOvenControl> ovenCtrol = new Mock<BaseOvenControl>();
            ovenCtrol.Setup(m => m.GetBoardTemperature(1, 1)).Returns(70.0);
            return ovenCtrol.Object;
        }

        public Test_TOSA25G(ILog logger)
        {
            _logger = logger;
        }


        public override void CreateDriver()
        {
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.TOSA25G, param);
            Assert.AreNotEqual(test, null);
        }


        public override void SeatCount()
        {
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.TOSA25G, param);
            this._logger.Info("the seats count:\t" + test.SeatsCount);
            Assert.AreEqual(test.SeatsCount, 16);

        }
        public override void AddSeat()
        {
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.TOSA25G, param);
            for (int i = 1; i <= 16; i++)
                test.AddSeat(i, string.Format("SN_{0:D2}", i));
        }
        public override void GetSnSet()
        {
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.TOSA25G, param);
            this._logger.Info("Add seat sn info:");
            Dictionary<int, string> seats = new Dictionary<int, string>();
            for (int i = 1; i <= 16; i++)
            {
                seats[i] = string.Format("SN_{0:D2}", i);
                test.AddSeat(i, seats[i]);
            }

            this._logger.Info("Get  sn set:");
            Dictionary<int, string> snSet = test.GetSnSet();
            foreach (var sn in snSet)
            {
                this._logger.Info("Sn info:\t" + sn.Key + ":" + sn.Value);
                Assert.AreEqual(sn.Value, seats[sn.Key]);
            }
        }
        public override void RemoveSeat()
        {
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.TOSA25G, param);
            this._logger.Debug("Add seats:");
            Dictionary<int, string> seats = new Dictionary<int, string>();
            for (int i = 1; i <= 16; i++)
            {
                seats[i] = string.Format("SN_{0:D2}", i);
                test.AddSeat(i, seats[i]);
            }
            this._logger.Info("Remove test seat:");

            for (int i = 1; i <= 14; i++)
            {
                test.RemoveSeat(i);
            }

            this._logger.Info("Left sn:");
            Dictionary<int, string> snSet = test.GetSnSet();
            Assert.AreEqual(snSet.Count, 2);
            foreach (var sn in snSet)
            {
                this._logger.Info("Left sn info:\t" + sn.Key + ":" + sn.Value);
            }
        }

        public override void EnableBoard()
        {
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            BaseProtocol protocol = MockBaseProtocol();
            BaseOvenControl ovenCtrol = MockBaseOvenControl();
            IBoard test = new TOSA25G(protocol, ovenCtrol, param, this._logger);
            this._logger.Info("Add seats:");
            for (int i = 1; i <= 16; i++)
                test.AddSeat(i, string.Format("SN_{0:D2}", i));
            this._logger.Info("Enable board:");
            test.EnableBoard();

        }


        public override void CheckConnections()
        {
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            BaseProtocol protocol = MockBaseProtocol();
            BaseOvenControl ovenCtrol = MockBaseOvenControl();
            IBoard test = new TOSA25G(protocol, ovenCtrol, param, this._logger);
            this._logger.Info("Add seats:");
            for (int i = 1; i <= 16; i++)
                test.AddSeat(i, string.Format("SN_{0:D2}", i));
            this._logger.Info("Check connections:");
            Dictionary<int, bool> ret = test.CheckConnections();
            foreach (var info in ret)
            {
                this._logger.Info("Connections status:" + info.Key + " = " + info.Value);
            }
            foreach (var info in ret)
            {

                Assert.AreEqual(true, info.Value);
            }
        }

        public override void DisableBoard()
        {

            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            BaseProtocol protocol = MockBaseProtocol();
            BaseOvenControl ovenCtrol = MockBaseOvenControl();
            IBoard test = new TOSA25G(protocol, ovenCtrol, param, this._logger);
            this._logger.Info("Add seats:");
            for (int i = 1; i <= 16; i++)
                test.AddSeat(i, string.Format("SN_{0:D2}", i));
            this._logger.Info("Disable board:");

            bool result = test.DisableBoard();
            Assert.AreEqual(result, true);

        }
        public override void ReadDataSet()
        {

            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            BaseProtocol protocol = MockBaseProtocol();
            BaseOvenControl ovenCtrol = MockBaseOvenControl();
            IBoard test = new TOSA25G(protocol, ovenCtrol, param, this._logger);

            Dictionary<int, List<KeyValuePair<string, string>>> dataSet = test.ReadDataSet("");


        }
        public override void SetUpTemperature()
        {

            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            BaseProtocol protocol = MockBaseProtocol();
            BaseOvenControl ovenCtrol = MockBaseOvenControl();
            IBoard test = new TOSA25G(protocol, ovenCtrol, param, this._logger);
            bool result = test.SetUpTemperature(60.0);
            Assert.AreEqual(result, true);
        }
        public override void TearDownTemperature()
        {

            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            BaseProtocol protocol = MockBaseProtocol();
            BaseOvenControl ovenCtrol = MockBaseOvenControl();
            IBoard test = new TOSA25G(protocol, ovenCtrol, param, this._logger);
            bool result = test.TearDownTemperature();
            Assert.AreEqual(result, true);
        }
        public override void CatchException()
        {

            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            BaseProtocol protocol = MockBaseProtocol();
            BaseOvenControl ovenCtrol = MockBaseOvenControl();
            IBoard test = new TOSA25G(protocol, ovenCtrol, param, this._logger);
            test.CatchException(1);

        }
        public override void GetMesStepName()
        {

            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.TOSA25G, param);
            string stepName = test.GetMesStepName();
            Assert.AreEqual(stepName, "CWT25_Burn In");
        }
        public override void IsCocCheck()
        {

            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.TOSA25G, param);
            bool result = test.IsCocCheck();
            Assert.AreEqual(result, false);

        }
        public override void GetTargetOvenTemperature()
        {

            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            BaseProtocol protocol = MockBaseProtocol();
            BaseOvenControl ovenCtrol = MockBaseOvenControl();
            IBoard test = new TOSA25G(protocol, ovenCtrol, param, this._logger);
            Assert.AreEqual(test.GetTargetOvenTemperature(), 70.0);


        }
        public override void GetCocTypeBySn()
        {

            

        }

        public override void GetCocTypeByPlan()
        {
            
        }
    }
}
