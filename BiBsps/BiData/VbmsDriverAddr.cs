namespace BiBsps.BiData
{
    public class VbmsDriverAddr
    {

        private static VbmsDriverAddr _driverAddr;

        public static VbmsDriverAddr Inst()
        {
            return _driverAddr ?? (_driverAddr = new VbmsDriverAddr());
        }

        protected VbmsDriverAddr()
        {
            ChipAddress = 0x44;
            AddVolAddr = 0x48;
            FloorAddr = new byte[]  { 0x0, 0xE0, 0xE2, 0xE4, 0xE6, 0xE8, 0xEA, 0xEC, 0xEE };
            LocateAddr = new byte[] { 0x0, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
            ChipAddr = new byte[]   { 0x0, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41 };
            CtrlAddr = new byte[]   { 0x0, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06 };
            CtrlByte = new byte[]   { 0x0, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
            SeatAddr = new byte[]   { 0, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 32, 33, 34, 35, 36, 37, 38, 39, 64, 65 };
        }

        public byte GetChipAddr()
        {
            return ChipAddress;
        }
        public byte GetAddVolAddr()
        {
            return AddVolAddr;
        }
        public byte GetFloorAddr(int floor)
        {
            return FloorAddr[floor];
        }
        public int GetFloorCount()
        {
            return FloorAddr.Length;
        }
        public byte GetLocationAddr(int location)
        {
            return LocateAddr[location];
        }

        public int GetLocationCount()
        {
            return LocateAddr.Length;
        }

        public byte GetSubChipAddr(int seat)
        {
            return ChipAddr[seat];
        }
        public byte GetCtrlAddr(int seat)
        {
            return CtrlAddr[seat];
        }
        public byte GetSeatAddr(int seat)
        {
            return SeatAddr[seat];
        }

        public int GetSeatCount()
        {
            return ChipAddr.Length;
        }

        public byte GetCtrlByte(int seat)
        {
            return CtrlByte[seat];
        }

        protected byte ChipAddress;
        protected byte AddVolAddr;   //加电地址
        protected byte[] FloorAddr;
        protected byte[] LocateAddr;
        protected byte[] ChipAddr;
        protected byte[] CtrlAddr;
        protected byte[] CtrlByte;
        protected byte[] SeatAddr;

    }
}
