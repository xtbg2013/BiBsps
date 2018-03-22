using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using log4net;
using Moq;
using BiBsps.BiDrivers.TosaBosa;
using BiBspsUnitTest.MesServiceReference;

namespace BiBspsUnitTest
{
    [TestClass]
    public class UnitTestDriverCfp8
    {
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
                    ["SEATS_COUNT"] = "16",
                    ["OVEN_TEMP_CHECK"] = "TRUE",
                    ["SET_BIAS_RANGE"] = "0,120",
                    ["CONNECT_CHECK_ICC_RANGE"] = "95,200",
                    ["CHECK_TEMPERATURE"] = "50",
                    ["TARGET_OVEN_TEMPERATURE"] = "60",
                    ["OVEN_TIMEOUT"] = "60",

                    ["TARGET_CURRENT"] = "80",
                    ["MES_STEP"] = "CFP8_Burn In",
                    ["READ_CURRENTK"] = "0.0003",
                    ["READ_CURRENTB"] = "0.0012",
                    ["READ_ITECK"] = "1.1615",
                    ["READ_ITECB"] = "-1740.2",

                    ["TARGET_TEC_TEMPERATURE"] = "90",
                    ["SET_TEMPERATUREB"] = "0.0006",
                    ["SET_TEMPERATUREK"] = "0.0222",
                    ["CONNECT_CHECK_BIAS"] = "30",
                    ["CAL_BIAS_ARRAY"] = "16",
                    ["CONNECT_CHECK_TEC_RANGE"] = "15,100",
                    ["DELTA_ICCB"] = "0.928",
                    ["DELTA_ICCK"] = "2.649",
                    ["DAC_MA"] = "0.75",
                    ["COC_TYPE"] = "BH1",
                    ["OvenPort"] = "7",

                    ["ControlPort"] = "6",
                    ["MES_CHECK"] = "TRUE",
                    ["HOLD_FLAG"] = "FALSE"
                };
                return param;
            }
        }

        [TestMethod]
        public void TestMethodConstruct()
        {
            var drvCfp8Tosa = new Cfp8Tosa(CreateParam, MockIlog());
            Assert.AreNotEqual(drvCfp8Tosa, null);
        }

        [TestMethod]
        public void TestMethodGetCocTypeBySn()
        {
            var drvCfp8Tosa = new Cfp8Tosa(CreateParam, MockIlog());
            string[] info;
            string msg;
            const string sn = "TW1729J04-07";
            var mes = new MesServiceClient();
            mes.GetCocInfoBySn(sn, out info, out msg);
            var bhType = drvCfp8Tosa.GetCocTypeBySn("TW1729J04-07", info);
            Assert.AreEqual(bhType, "BH1");
        }
    }
}
