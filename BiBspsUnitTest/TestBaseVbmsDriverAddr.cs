using BiBsps.BiData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BiBspsUnitTest
{
    [TestClass]
    public class TestBaseVbmsDriverAddr
    {
        private readonly VbmsDriverAddr _vbmsDriverAddr;

        public TestBaseVbmsDriverAddr()
        {
            _vbmsDriverAddr = VbmsDriverAddr.Inst();
        }

        [TestMethod]
        public void TestGetChipAddr()
        {
            Assert.AreEqual(_vbmsDriverAddr.GetChipAddr(), 0x44);
        }

        [TestMethod]
        public void TestGetAddVolAddr()
        {
            Assert.AreEqual(_vbmsDriverAddr.GetAddVolAddr(), 0x48);
        }

        [TestMethod]
        public void TestGetFloorAddr()
        {
            Assert.AreEqual(_vbmsDriverAddr.GetFloorAddr(0), 0x0);
            Assert.AreEqual(_vbmsDriverAddr.GetFloorAddr(1), 0xE0);
            Assert.AreEqual(_vbmsDriverAddr.GetFloorAddr(2), 0xE2);
            Assert.AreEqual(_vbmsDriverAddr.GetFloorAddr(3), 0xE4);
            Assert.AreEqual(_vbmsDriverAddr.GetFloorAddr(4), 0xE6);
            Assert.AreEqual(_vbmsDriverAddr.GetFloorAddr(5), 0xE8);
            Assert.AreEqual(_vbmsDriverAddr.GetFloorAddr(6), 0xEA);
            Assert.AreEqual(_vbmsDriverAddr.GetFloorAddr(7), 0xEC);
            Assert.AreEqual(_vbmsDriverAddr.GetFloorAddr(8), 0xEE);
        }

        [TestMethod]
        public void TestGetFloorCount()
        {
            Assert.AreEqual(_vbmsDriverAddr.GetFloorCount(), 9);
        }

        [TestMethod]
        public void TestGetLocationAddr()
        {
            Assert.AreEqual(_vbmsDriverAddr.GetLocationAddr(0), 0x0);
            Assert.AreEqual(_vbmsDriverAddr.GetLocationAddr(1), 0x08);
            Assert.AreEqual(_vbmsDriverAddr.GetLocationAddr(2), 0x09);
            Assert.AreEqual(_vbmsDriverAddr.GetLocationAddr(3), 0x0A);
            Assert.AreEqual(_vbmsDriverAddr.GetLocationAddr(4), 0x0B);
            Assert.AreEqual(_vbmsDriverAddr.GetLocationAddr(5), 0x0C);
            Assert.AreEqual(_vbmsDriverAddr.GetLocationAddr(6), 0x0D);
            Assert.AreEqual(_vbmsDriverAddr.GetLocationAddr(7), 0x0E);
            Assert.AreEqual(_vbmsDriverAddr.GetLocationAddr(8), 0x0F);
        }

        [TestMethod]
        public void TestGetLocationCount()
        {
            Assert.AreEqual(_vbmsDriverAddr.GetLocationCount(), 9);
        }

        [TestMethod]
        public void TestGetSubChipAddr()
        {
            Assert.AreEqual(_vbmsDriverAddr.GetSubChipAddr(0), 0x0);
            for (var i = 1; i <= 16; i++)
            {
                Assert.AreEqual(_vbmsDriverAddr.GetSubChipAddr(i), 0x40);
            }

            for (var j = 17; j <= 24; j++)
            {
                Assert.AreEqual(_vbmsDriverAddr.GetSubChipAddr(j), 0x41);
            }
        }

        [TestMethod]
        public void TestGetCtrlAddr()
        {
            Assert.AreEqual(_vbmsDriverAddr.GetCtrlAddr(0), 0x0);
            for (var i = 1; i <= 8; i++)
            {
                Assert.AreEqual(_vbmsDriverAddr.GetCtrlAddr(i), 0x06);
            }

            for (var j = 9; j <= 16; j++)
            {
                Assert.AreEqual(_vbmsDriverAddr.GetCtrlAddr(j), 0x07);
            }

            for (var z = 17; z <= 24; z++)
            {
                Assert.AreEqual(_vbmsDriverAddr.GetCtrlAddr(z), 0x06);
            }
        }

        [TestMethod]
        public void TestGetSeatAddr()
        {
            var value = 8;
            Assert.AreEqual(_vbmsDriverAddr.GetSeatAddr(0), 0x0);
            for (var i = 1; i <= 16; i++)
            {
                Assert.AreEqual(_vbmsDriverAddr.GetSeatAddr(i), value++);
            }

            value = 32;
            for (var j = 17; j <= 24; j++)
            {
                Assert.AreEqual(_vbmsDriverAddr.GetSeatAddr(j), value++);
            }

            value = 64;
            for (var z = 25; z <= 26; z++)
            {
                Assert.AreEqual(_vbmsDriverAddr.GetSeatAddr(z), value++);
            }
        }

        [TestMethod]
        public void TestGetSeatCount()
        {
            Assert.AreEqual(_vbmsDriverAddr.GetSeatCount(), 25);
        }

        [TestMethod]
        public void TestGetCtrlByte()
        {
            Assert.AreEqual(_vbmsDriverAddr.GetCtrlByte(0), 0);
            for (var i = 1; i <= 8; i++)
            {
                var val = 0x01 << (i - 1);
                Assert.AreEqual(_vbmsDriverAddr.GetCtrlByte(i), val);
            }

            for (var i = 9; i <= 16; i++)
            {
                var val = 0x01 << (i - 9);
                Assert.AreEqual(_vbmsDriverAddr.GetCtrlByte(i), val);
            }

            for (var i = 17; i <= 24; i++)
            {
                var val = 0x01 << (i - 17);
                Assert.AreEqual(_vbmsDriverAddr.GetCtrlByte(i), val);
            }
        }
    }
}
