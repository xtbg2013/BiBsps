using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiBsps;
using System.Collections.Generic;

namespace BiBspsUnitTest
{
    [TestClass]
    public class UnitTestDriverFactory
    {
        [TestMethod]
        public void TestConstructTestTypeDriver()
        {
            var param = new Dictionary<string, string>
            {
                ["Floor"] = "10",
                ["Name"] = "A",
                ["Locate"] = "1"
            };
            var factory = new DriverFactory();
            var board = factory.CreateDriver("TEST_TYPE", null, param);
            Assert.AreNotEqual(board, null);
        }
    }
}
