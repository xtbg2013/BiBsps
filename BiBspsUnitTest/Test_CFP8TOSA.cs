using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiBsps;
using BiBsps.Log;
using BILib;
using BIModel;
using Moq;
using BiBsps.BiDrivers;
using BiBsps.BiProtocols;
using BiBsps.BiOven;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Reflection;
namespace BiUnitTest
{
    class Test_CFP8TOSA:BaseDriverCase
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
            param["CHECK_TEMP"] = "TRUE";
            param["BIAS_SET_RANGE_LOW"] = "-1";
            param["BIAS_SET_RANGE_HIGH"] = "120";
            param["ICC_CONNECT_LOW"] = "95";
            param["ICC_CONNECT_HIGH"] = "200";

            param["OVEN_CHECK_TEMPERATURE"] = "50";
            param["OVEN_TARGET_TEMPERATURE"] = "60";
            param["HEAT_TIME"] = "5";
            param["COC_CHECK"] = "TRUE";

            param["TARGET_CURRENT"] = "80";
            param["STEP_NAME"] = "CFP8_Burn In";
            param["READ_CURRENTSK"] = "0.0003";
            param["READ_CURRENTSB"] = "0.0012";
            param["READ_ITECK"] = "1.1615";
            param["READ_ITECB"] = "-1740.2";
            param["TEC_TEMPERATURE"] = "90";
            param["TEC_TEMPERATUREK"] = "0.0006";
            param["TEC_TEMPERATUREB"] = "0.0222";
            param["START_DAC"] = "106";
            param["DELTA_ICCB"] = "0.928";
            param["DELTA_ICCK"] = "2.649";
            param["DAC_MA"] = "0.75";

            param["COC_TYPE"] = "BH1";
            return param;

        }
        private BaseProtocol MockBaseProtocol()
        {
            Mock<BaseProtocol> protocol = new Mock<BaseProtocol>();
            int[] icc = new int[16];
            int[] itec = new int[16];
            int[] currentDac;
            int[] mpdDac;
            int[] tecDac =new int[16];
            int t1Dac=30, t2Dac;
            for (int i = 0; i < 16; i++)
            {
                icc[i] = 0x148;
                tecDac[i] = 0x0830;
            }
                
            protocol.Setup(m => m.ReadIccDac()).Returns(icc);
            protocol.Setup(m => m.ReadItecDac()).Returns(itec);
            protocol.Setup(m => m.ReadCurrentDacAndMpdDac(out currentDac, out mpdDac));
            protocol.Setup(m=>m.ReadTecTemperatureDac(out tecDac, out t1Dac, out t2Dac));
           
            return protocol.Object;    
        }
        private BaseOvenControl MockBaseOvenControl()
        {
            Mock<BaseOvenControl> ovenCtrol = new Mock<BaseOvenControl>();
            ovenCtrol.Setup(m => m.GetBoardTemperature(1, 1)).Returns(60.0);
            return ovenCtrol.Object;
        }

        public Test_CFP8TOSA(ILog logger)
        {
            _logger = logger;
        }

     
        public override void CreateDriver()
        {
            //string asmPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"BiBsps.dll");
            //if (!System.IO.File.Exists(asmPath))
            //{
            //    throw new Exception("adfa");
            //}
            //Assembly ass = System.Reflection.Assembly.LoadFile(asmPath);
            //Type type = ass.GetType("BiBsps.DriverFactory");
     
            //IDriverFactory fact = (IDriverFactory)Activator.CreateInstance(type, new object[] { _logger });
            //string[] x = fact.GetSupportDriver();

            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.CFP8TOSA, param);
            Assert.AreNotEqual(test, null);
        }


        public override void SeatCount()
        { 
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.CFP8TOSA, param);
            this._logger.Info("the seats count:\t" + test.SeatsCount);
            Assert.AreEqual(test.SeatsCount, 16);
            
        }
        public override void AddSeat()
        {
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.CFP8TOSA, param);
            for (int i = 1; i <= 16; i++)
                test.AddSeat(i,string.Format("SN_{0:D2}", i));
        }
        public override void GetSnSet()
        {
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.CFP8TOSA, param);
            this._logger.Info("Add seat sn info:");
            Dictionary<int, string> seats = new Dictionary<int, string>();
            for (int i = 1; i <= 16; i++)
            {
                seats[i] = string.Format("SN_{0:D2}", i);
                test.AddSeat(i, seats[i]);
            }
                
            this._logger.Info("Get  sn set:");
            Dictionary<int, string> snSet =  test.GetSnSet();
            foreach (var sn in snSet)
            {
                this._logger.Info("Sn info:\t" + sn.Key + ":" + sn.Value);
                Assert.AreEqual(sn.Value, seats[sn.Key]);
            }
        }
        public override void RemoveSeat()
        {
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.CFP8TOSA, param);
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
            IBoard test = new CFP8TOSA(protocol, ovenCtrol, param, this._logger);
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
            IBoard test = new CFP8TOSA(protocol, ovenCtrol, param, this._logger);
            this._logger.Info("Add seats:");
            for (int i = 1; i <= 16; i++)
                test.AddSeat(i, string.Format("SN_{0:D2}", i));
            this._logger.Info("Check connections:");
            Dictionary<int, bool> ret =  test.CheckConnections();
            foreach (var info in ret)
            {
                this._logger.Info("Connections status:"+info.Key+" = "+info.Value);
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
            IBoard test = new CFP8TOSA(protocol, ovenCtrol, param, this._logger);
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
            IBoard test = new CFP8TOSA(protocol, ovenCtrol, param, this._logger);

            Dictionary < int, List<KeyValuePair<string, string>>>  dataSet = test.ReadDataSet("");
           
            
        }
        public override void SetUpTemperature()
        {
          
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            BaseProtocol protocol = MockBaseProtocol();
            BaseOvenControl ovenCtrol = MockBaseOvenControl();
            IBoard test = new CFP8TOSA(protocol, ovenCtrol, param, this._logger);
            bool result = test.SetUpTemperature(60.0) ;
            Assert.AreEqual(result, true);
        }
        public override void TearDownTemperature()
        {
            
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            BaseProtocol protocol = MockBaseProtocol();
            BaseOvenControl ovenCtrol = MockBaseOvenControl();
            IBoard test = new CFP8TOSA(protocol, ovenCtrol, param, this._logger);
            bool result = test.TearDownTemperature();
            Assert.AreEqual(result, true);
        }
        public override void CatchException()
        {
           
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            BaseProtocol protocol = MockBaseProtocol();
            BaseOvenControl ovenCtrol = MockBaseOvenControl();
            IBoard test = new CFP8TOSA(protocol, ovenCtrol, param, this._logger);
            test.CatchException(1);
            
        }
        public override void GetMesStepName()
        {
           
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.CFP8TOSA, param);
            string stepName = test.GetMesStepName();
            Assert.AreEqual(stepName,"CFP8_Burn In");
        }
        public override void IsCocCheck()
        {
            
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.CFP8TOSA, param);
            bool result = test.IsCocCheck();
            Assert.AreEqual(result, true);

        }
        public override void GetTargetOvenTemperature()
        {
          
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            BaseProtocol protocol = MockBaseProtocol();
            BaseOvenControl ovenCtrol = MockBaseOvenControl();
            IBoard test = new CFP8TOSA(protocol, ovenCtrol, param, this._logger);
            Assert.AreEqual( test.GetTargetOvenTemperature(),60.0);

          
        }
        public override void GetCocTypeBySn()
        {

            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.CFP8TOSA, param);
            string[] info1 = new string[16]
            {
                "","1256031","","",
                "","1256032","","",
                "","1256033","","",
                "","1256034","",""
            };
            string[] info2 = new string[16]
            {
                "","1261571","","",
                "","1261572","","",
                "","1261573","","",
                "","1261574","",""
            };
            string[] info3 = new string[16]
            {
                "","1256031","","",
                "","1256032","","",
                "","1261571","","",
                "","1261572","",""
            };
            Assert.AreEqual( test.GetCocTypeBySn("TEST_1", info1),"BH1");
            Assert.AreEqual(test.GetCocTypeBySn("TEST_2", info2), "BH2");
            Assert.AreEqual(test.GetCocTypeBySn("TEST_3", info3), "BH2");

        }
          
        public override void GetCocTypeByPlan()
        {
            Dictionary<string, string> param = CreateParam(_boardName, _floorNum, _boardNum, _ovenPort, _ctlPort);
            IBoard test = DriverFactory.Instance(_logger).CreateDriver(DriverType.CFP8TOSA, param);
            string cocType = test.GetCocTypeByPlan();
            Assert.AreEqual(cocType, "BH1"); 
        }
    }
}
