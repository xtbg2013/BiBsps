namespace BiBsps.BiData
{
    public class VbmsCtrlData
    {
        public byte Frame { get; set; }
        public byte Page  { get; set; }
        public byte Address { get; set; }
        public byte DataLen { get; set; }
        public byte[] Data { get; set; }
      
        public VbmsCtrlData(byte frame, byte page, byte address, byte dataLen)
        {
            Frame = frame;
            Page = page;
            Address = address;
            DataLen = dataLen;
        }

        public VbmsCtrlData(byte frame, byte page, byte address,byte[] data)
        {
            Frame = frame;
            Page = page;
            Address = address;
            Data = data;
           
        }
    }

     
    
}
