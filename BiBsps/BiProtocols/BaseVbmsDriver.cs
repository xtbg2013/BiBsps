using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiBsps.BiProtocols
{
    
    public class BaseVbmsDriver
    {
        protected int _chipAddress;
        protected byte[] floorAddr;
        protected byte[] locateAddr;
        protected byte[] chipAddr;
        protected byte[] ctrlAddr;
        protected byte[] ctrlByte;
        protected byte[] slotAddr;
        protected void SelectBoard(int floor, int location)
        {
            byte[] status = new byte[0x100];
            byte devAddr = floorAddr[floor];
            byte lAddr = locateAddr[location];
            if (1 != eDriver_IO.Cls_edriverdll.i2c_write(devAddr, 0x01, 1, 0, new byte[] { lAddr }, status))
            {
                throw new Exception(ASCIIEncoding.ASCII.GetString(status));
            }
        }
        protected bool IsSlotEnable(int slot)
        {
            byte[] status = new byte[0x100];
            //Select Chip
            if (1 != eDriver_IO.Cls_edriverdll.i2c_write(this._chipAddress, 0x01, 1, 0, new byte[] { chipAddr[slot] }, status))
            {
                throw new Exception(ASCIIEncoding.ASCII.GetString(status));
            }
            //Set Control Bit
            byte[] data = new byte[1];
            if (1 != eDriver_IO.Cls_edriverdll.i2c_read(0x48, ctrlAddr[slot], 1, 0, data, status))
                throw new Exception(ASCIIEncoding.ASCII.GetString(status));
            return (0 != (byte)((~data[0]) & ctrlByte[slot]));
        }
        protected void SelectSlot(int slot)
        {
            byte[] status = new byte[0x100];
            
            if (this.IsSlotEnable(slot) == false)
            {
                //Select 7312 Chip 1
                if (1 != eDriver_IO.Cls_edriverdll.i2c_write(this._chipAddress, 0x01, 1, 0, new byte[] { 0x40 }, status))
                {
                    throw new Exception(ASCIIEncoding.ASCII.GetString(status));
                }
                if (1 != eDriver_IO.Cls_edriverdll.i2c_write(0x48, 0x06, 1, 0, new byte[] { 0x00 }, status))
                {
                    throw new Exception(ASCIIEncoding.ASCII.GetString(status));
                }
                if (1 != eDriver_IO.Cls_edriverdll.i2c_write(0x48, 0x07, 1, 0, new byte[] { 0x00 }, status))
                {
                    throw new Exception(ASCIIEncoding.ASCII.GetString(status));
                }
                //Select 7312 Chip 2
                if (1 != eDriver_IO.Cls_edriverdll.i2c_write(this._chipAddress, 0x01, 1, 0, new byte[] { 0x41 }, status))
                {
                    throw new Exception(ASCIIEncoding.ASCII.GetString(status));
                }
                if (1 != eDriver_IO.Cls_edriverdll.i2c_write(0x48, 0x06, 1, 0, new byte[] { 0x00 }, status))
                {
                    throw new Exception(ASCIIEncoding.ASCII.GetString(status));
                }
                System.Threading.Thread.Sleep(1000);
            }
            if (1 != eDriver_IO.Cls_edriverdll.i2c_write(this._chipAddress, 0x01, 1, 0, new byte[] { slotAddr[slot] }, status))
            {
                throw new Exception(ASCIIEncoding.ASCII.GetString(status));
            }

        }
        protected byte ReadDeviceStatus(int floor)
        {
            byte[] error = new byte[0x100];
            byte[] data = new byte[] { 0xFF };
            int devAddr = floorAddr[floor];
            if (1 != eDriver_IO.Cls_edriverdll.i2c_read(devAddr, 0x01, 1, 0, data, error))
            {
                throw new Exception(ASCIIEncoding.ASCII.GetString(error));
            }
            return data[0];
        }
        protected void EnableBIBoard()
        {
            byte[] status = new byte[0x100];
            if (1 != eDriver_IO.Cls_edriverdll.i2c_write(this._chipAddress, 0x0F, 1, 0, new byte[] { 0x00 }, status))
            {
                throw new Exception(ASCIIEncoding.ASCII.GetString(status));
            }

            if (1 != eDriver_IO.Cls_edriverdll.i2c_write(this._chipAddress, 0x03, 1, 0, new byte[] { 0x00 }, status))
            {
                throw new Exception(ASCIIEncoding.ASCII.GetString(status));
            }
        }
        protected void EnableFloor(bool enable)
        {
            byte[] status = new byte[0x100];
            int addr = enable ? 0x0 : 0x01;
            for (int i = 1; i < floorAddr.Count(); i++)
            {
                int devAddr = floorAddr[i];
                if (1 != eDriver_IO.Cls_edriverdll.i2c_write((int)devAddr, addr, 1, 0, new byte[] { 0 }, status))
                {
                    throw new Exception(ASCIIEncoding.ASCII.GetString(status));
                }
            }
        }


        public BaseVbmsDriver(int chipAdrress = 0x44)
        {
            this._chipAddress = chipAdrress;
            floorAddr = new byte[] { 0x0, 0xE0, 0xE2, 0xE4, 0xE6, 0xE8, 0xEA, 0xEC, 0xEE };
            locateAddr= new byte[] { 0x0, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
            chipAddr  = new byte[] { 0x0, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41 };
            ctrlAddr  = new byte[] { 0x0, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06 };
            ctrlByte  = new byte[] { 0x0, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
            slotAddr  = new byte[] { 0, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 32, 33, 34, 35, 36, 37, 38, 39, 64, 65 };
        }
        public void ConfigI2C()
        {
            byte[] status = new byte[0x100];
            byte[] ProductInfo = new byte[256];
            if (1 != eDriver_IO.Cls_edriver_mem_dll.edriver_mem_config(ProductInfo, status))
            {
                throw new Exception(ASCIIEncoding.ASCII.GetString(status));
            }
            if (1 != eDriver_IO.Cls_edriver_security_dll.edriver_security_config(ProductInfo, status))
            {
                throw new Exception(ASCIIEncoding.ASCII.GetString(status));
            }

            if (1 != eDriver_IO.Cls_edriverdll.Edriver_Config(out status, status.Length))
            {
                throw new Exception(ASCIIEncoding.ASCII.GetString(status));
            }
            EnableFloor(true);
        }
        public void EnableSlot(int slot, bool isEnable)
        {
            byte[] status = new byte[0x100];
            //Select Chip
            if (1 != eDriver_IO.Cls_edriverdll.i2c_write(this._chipAddress, 0x01, 1, 0, new byte[] { chipAddr[slot] }, status))
            {
                throw new Exception(ASCIIEncoding.ASCII.GetString(status));
            } 
            //Set Control Bit
            byte[] data = new byte[1];
            if (1 != eDriver_IO.Cls_edriverdll.i2c_read(0x48, ctrlAddr[slot], 1, 0, data, status))
            {
                throw new Exception(ASCIIEncoding.ASCII.GetString(status));
            }
            byte target = isEnable ? (byte)(data[0] & (~ctrlByte[slot])) : (byte)(data[0] | ctrlByte[slot]);
            if (1 != eDriver_IO.Cls_edriverdll.i2c_write(0x48, ctrlAddr[slot], 1, 0, new byte[] { target }, status))
            {
                throw new Exception(ASCIIEncoding.ASCII.GetString(status));
            }
                
        }
        public bool SelectDut(int floor, int locate,int slot,out string msg)
        {
            msg = "";
            try
            {
                if (ReadDeviceStatus(floor) == 0)
                {
                    EnableFloor(false);
                }
                SelectBoard(floor, locate);
                EnableBIBoard();
                SelectSlot(slot);
                EnableSlot(slot,true);
            }
            catch (Exception ex)
            {
                EnableFloor(false);
                msg = ex.Message;
                return false;
            }
            return true;
        }
        public void UnlockData()
        {
            byte[] status = new byte[0x100];
            if (1 != eDriver_IO.Cls_edriver_security_dll.Edriver_Finisar_Password_EN(0xA0, 0xA0, out status, status.Length))
            {
                throw new Exception(ASCIIEncoding.ASCII.GetString(status));
            }      
        }
        public bool Read(int frame, int page, int address, byte[] data, out string status)
        {
            byte[] msg = new byte[0x100];
            if (1 != eDriver_IO.Cls_edriver_mem_dll.edriver_mem_read(0xA0, frame, page * 256 + address, data.Length, 1, data.Length * 8 - 1, data.Length * 8, 0, data, msg))
            {
                status = ASCIIEncoding.ASCII.GetString(msg);
                return false;
            }
            status = "OK";
            return true;
        }
        public bool Write(int frame, int page, int address, byte[] data, out string status)
        {
            byte[] msg = new byte[0x100];
            if (1 != eDriver_IO.Cls_edriver_mem_dll.edriver_mem_write(0xA0, frame, page * 256 + address, data.Length, 1, data.Length * 8 - 1, data.Length * 8, 0, data, msg))
            {
                status = ASCIIEncoding.ASCII.GetString(msg);
                return false;
            }
            status = "OK";
            return true;
        }
    }
}
