using System.Collections.Generic;
using log4net;
using BiBsps.BiDrivers.Parallel;
using BiBsps.BiDrivers.TosaBosa;
using BiBsps.BiProtocols.Parallel;
using BiBsps.BiDrivers.Iqm;
using BiBsps.BiData;
using BiInterface;

namespace BiBsps
{
    public class DriverFactory:IDriverFactory
    {

        private ILog _log;
        public string[] GetSupportDriver()
        {
            var table = new[]
            {
                "TEST_TYPE",
                "CFP8TOSA",
                "TOSA25G",
                "QSFP28G_TEC",  //WITH TEC TEST
                "QSFP28G_NOTEC",//HAVE NO TEC TEST
                "TOSA32G",
                "CFP4TOSA",
                "IQMGenI",
                "PARALLEL_QSFP28G_SEMTECH",
                "PARALLEL_QSFP28G_INTERNAL"
            };
            return table;
        }
        public IBoard CreateDriver(string driverType, ILog logger, Dictionary<string, string> param)
        {
            _log = logger;
            IBoard driver = null;
            switch (driverType.Trim())
            {
                case "TEST_TYPE":
                    driver = new TestType(param, _log);
                    break;
                case "CFP8TOSA":
                    driver = new Cfp8Tosa(param, _log);
                    break;
                case "TOSA25G":
                    driver = new Tosa25G(param, _log);
                    break;
                case "QSFP28G_TEC":
                    driver = new Qsfp28G(param, _log);
                    break;
                case "QSFP28G_NOTEC":
                    driver = new Qsfp28GNoTec(param, _log);
                    break;
                case "TOSA32G":
                    driver = new Tosa32G(param, _log);
                    break;
                case "CFP4":
                    //driver = new QSFP28GNoTec(param, _log);
                    break;
                case "IQMGenI":
                    driver = new IqmGenI(param, _log);
                    break;
                case "PARALLEL_QSFP28G_INTERNAL":
                    var vbmsAddr = VbmsDriverAddr.Inst();
                    var cmd =  VbmsCmd.Inst();
                    var parallelProtocol = ParallelProtocolQsfp28G.Inst(vbmsAddr, cmd);
                    driver = new ParallelQsfp28(param, parallelProtocol, _log);
                    break;
                default:
                    _log.Error("BiBsp don't support the driver:"+driverType);
                    break;
            }
            return driver;
        }
    }

}