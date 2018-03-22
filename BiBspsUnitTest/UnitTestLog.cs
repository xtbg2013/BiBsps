using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiBsps.BiLog;
using log4net;

namespace BiBspsUnitTest
{
    [TestClass]
    public class UnitTestLog
    {
        [TestMethod]
        public void TestMethodInfo()
        {
            var log = LogManager.GetLogger("BiBspLocolLog");
            var log1 = new DriverLog(log, "CFP8", "A");
            log1.LogInfo("This is a test");
            log1.LogError("This is a test");
            log1.LogWarn("This is a test");
            log1.LogDebug("This is a test");
        }
    }
}
