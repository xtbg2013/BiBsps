using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiBsps.BiDrivers.TosaBosa;
using BiBsps.BiGlobalFiies;

namespace BiBspsUnitTest
{
    [TestClass]
    public class UnitTestInitTemp
    {
        private readonly string _path;

        public UnitTestInitTemp()
        {
            _path = @"D:\InitTemp.xml";
        }

        [TestMethod]
        public void TestConstructInitTemp()
        {
            var temp = new InitTemp(_path);
            Assert.AreNotEqual(temp, null);
        }

        [TestMethod]
        public void TestSaveCalData()
        {
            var temp = new InitTemp(_path);
            var current = new BoardCurrents {BoardName = "a"};
            current.SlotInit.Add(
                new SeatInit
                {
                    Sn = "X1",
                    Position = 1,
                    CurrentDac0 = 1,
                    Icc0 = 1
                });
            current.SlotInit.Add(
                new SeatInit
                {
                    Sn = "X2",
                    Position = 2,
                    CurrentDac0 = 1,
                    Icc0 = 1
                });

            current.SlotInit.Add(
                new SeatInit
                {
                    Sn = "X3",
                    Position = 3,
                    CurrentDac0 = 1,
                    Icc0 = 1
                });

            temp.SaveCalData(current);

            SeatInit seat;
            var res = temp.GetCalData("a", 1, out seat);
            Assert.AreEqual(res, true);
            Assert.AreEqual(seat.Sn, "X1");
            res = temp.GetCalData("a", 2, out seat);
            Assert.AreEqual(res, true);
            Assert.AreEqual(seat.Sn, "X2");
            res = temp.GetCalData("a", 3, out seat);
            Assert.AreEqual(res, true);
            Assert.AreEqual(seat.Sn, "X3");

            res = temp.GetCalData("a", 4, out seat);
            Assert.AreEqual(res, false);
            res = temp.GetCalData("b", 1, out seat);
            Assert.AreEqual(res, false);
        }

        [TestMethod]
        public void TestRemoveCalData()
        {
            var temp = new InitTemp(_path);
            var current = new BoardCurrents {BoardName = "a"};
            current.SlotInit.Add(
                new SeatInit
                {
                    Sn = "X1",
                    Position = 1,
                    CurrentDac0 = 1,
                    Icc0 = 1
                });
            current.SlotInit.Add(
                new SeatInit
                {
                    Sn = "X2",
                    Position = 2,
                    CurrentDac0 = 1,
                    Icc0 = 1
                });

            current.SlotInit.Add(
                new SeatInit
                {
                    Sn = "X3",
                    Position = 3,
                    CurrentDac0 = 1,
                    Icc0 = 1
                });
            temp.SaveCalData(current);

            SeatInit seat;
            var res = temp.RemoveCalData("a", 1);
            Assert.AreEqual(res, true);
            res = temp.GetCalData("a", 1, out seat);
            Assert.AreEqual(res, false);
        }

        [TestMethod]
        public void TestIsCalDataExist()
        {
            var temp = new InitTemp(_path);
            var current = new BoardCurrents {BoardName = "a"};
            current.SlotInit.Add(
                new SeatInit
                {
                    Sn = "X1",
                    Position = 1,
                    CurrentDac0 = 1,
                    Icc0 = 1
                });
            current.SlotInit.Add(
                new SeatInit
                {
                    Sn = "X2",
                    Position = 2,
                    CurrentDac0 = 1,
                    Icc0 = 1
                });

            current.SlotInit.Add(
                new SeatInit
                {
                    Sn = "X3",
                    Position = 3,
                    CurrentDac0 = 1,
                    Icc0 = 1
                });
            temp.SaveCalData(current);

            SeatInit seat;
            var res = temp.IsCalDataExist("a", 1);
            Assert.AreEqual(res, true);
            res = temp.GetCalData("a", 4, out seat);
            Assert.AreEqual(res, false);

            res = temp.GetCalData("b", 4, out seat);
            Assert.AreEqual(res, false);
        }
    }
}
