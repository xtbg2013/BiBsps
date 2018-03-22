using BiBsps.BiGlobalFiies;
using BiBsps.BiOven;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BiBspsUnitTest
{
    [TestClass, Ignore]
    public class UnitTestOven
    {
        private string _com = "COM15";
        private int _baudrate = 9600;

        [TestMethod]
        public void Test_SetTempOffset()
        {
            BaseOvenControl oven = new OvenControl(new Comm(_com, _baudrate));

            oven.SetTempOffset(1, 1, 60);
        }

        [TestMethod]
        public void Test_SetBoardTemperature()
        {
            BaseOvenControl oven = new OvenControl(new Comm(_com, _baudrate));

            oven.SetBoardTemperature(10, 3, 60);
        }


        [TestMethod]
        public void Test_GetBoardTemperature()
        {
            BaseOvenControl oven = new OvenControl(new Comm(_com, _baudrate));
            double temp = oven.GetBoardTemperature(1, 1);
            Assert.AreEqual(true, temp > 57 && temp < 63);
        }


        [TestMethod]
        public void Test_StartBoardOven()
        {
            BaseOvenControl oven = new OvenControl(new Comm(_com, _baudrate));

            oven.StartBoardOven(1, 1);
        }

        [TestMethod]
        public void Test_StopBoardOven()
        {
            BaseOvenControl oven = new OvenControl(new Comm(_com, _baudrate));

            oven.StopBoardOven(1, 1);
        }
    }
}
