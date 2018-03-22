using System.Text;
using System.Collections.Generic;
using System.Linq;
using BiBsps.BiData;
using BiBsps.BiInterface;

namespace BiBsps.BiProtocols.Parallel
{
    public class BaseParallelProtocol: IParallelProtocol
    {
        protected readonly VbmsDriverAddr DriverAddr;
        protected Dictionary<CmdType, VbmsCtrlData> VbmsCtrlCmd;
        protected string GetDataStr(byte[] bytes)
        {
            return bytes.Aggregate("", (current, t) => current + (t.ToString("X2") + " "));
        }
        public BaseParallelProtocol(VbmsDriverAddr vbmsAddr,VbmsCmd cmd)
        {
            DriverAddr = vbmsAddr;
            VbmsCtrlCmd = cmd.GetQsfp28GCmd();
        }
        protected bool IsFloorValid(int floor, out string msg)
        {
            msg = "";
            if (floor >= 0 && floor <= DriverAddr.GetFloorCount()) return true;
            msg = $"Select board: floor is invalid [floor = {floor}]";
            return false;
        }
        protected bool IsLocationValid(int location, out string msg)
        {
            msg = "";
            if (location >= 0 && location <= DriverAddr.GetLocationCount()) return true;
            msg = $"Select board: location is invalid [location = {location}]";
            return false;
        }
        protected bool IsSeatValid(int seat, out string msg)
        {
            msg = "";
            if (seat >= 0 && seat <= DriverAddr.GetSeatCount()) return true;
            msg = $"Select seat: seat is invalid [seat = {seat}]";
            return false;
        }
        protected bool EnableFloor(int floor, bool enable,out string msg)
        {
            msg = "";
            if (!IsFloorValid(floor, out msg)) return false;
            var status = new byte[0x100];
            var addr = enable ? 0x0 : 0x01;

            int devAddr = DriverAddr.GetFloorAddr(floor);
            if (1 == eDriver_IO.Cls_edriverdll.i2c_write(devAddr, addr, 1, 0, new byte[] {0}, status)) return true;
            msg = Encoding.ASCII.GetString(status);
            return false;
        }
        protected bool SelectBoard(int floor, int location,out string msg)
        {
            msg = "";
            var status = new byte[0x100];
            if (!IsFloorValid(floor, out msg)) return false;
            if (!IsLocationValid(location, out msg)) return false;
            var floorAddr = DriverAddr.GetFloorAddr(floor);
            var locateAddr = DriverAddr.GetLocationAddr(location);
            if (1 == eDriver_IO.Cls_edriverdll.i2c_write(floorAddr, 0x01, 1, 0, new[] {locateAddr}, status))
                return true;
            msg = Encoding.ASCII.GetString(status);
            return false;
        }
        protected bool IsVoltageAdd(int seat,out bool enable,out string msg)
        {
            var status = new byte[0x100];
            enable = false;
            if (!IsSeatValid(seat, out msg)) return false;

            var chipAddr = DriverAddr.GetChipAddr();
            var subChipAddr = DriverAddr.GetSubChipAddr(seat);
            var ctrlAddr = DriverAddr.GetCtrlAddr(seat);
            var volAddr = DriverAddr.GetAddVolAddr();
            var ctrlByte = DriverAddr.GetCtrlByte(seat);
            //Select Chip
            if (1 != eDriver_IO.Cls_edriverdll.i2c_write(chipAddr, 0x01, 1, 0, new[] { subChipAddr }, status))
            {
                msg = Encoding.ASCII.GetString(status);
                return false;
            }
            //Set Control Bit
            var data = new byte[1];
            if (1 == eDriver_IO.Cls_edriverdll.i2c_read(volAddr, ctrlAddr, 1, 0, data, status))
            {
                enable = (0 != (byte) ((~data[0]) & ctrlByte));
                return true;
            }
            msg = Encoding.ASCII.GetString(status);
            return false;
        }
        protected bool SelectSeat(int seat,out string msg)
        {
            msg = "";
            var status = new byte[0x100];
            if (!IsSeatValid(seat, out msg)) return false;
            var devAddr = DriverAddr.GetChipAddr();
            var seatAddr = DriverAddr.GetSeatAddr(seat);
            if (1 == eDriver_IO.Cls_edriverdll.i2c_write(devAddr, 0x01, 1, 0, new[] {seatAddr}, status)) return true;
            msg = Encoding.ASCII.GetString(status);
            return false;
        }
        protected bool ReadTargetFloor(int floor,out byte status,out string msg)
        {
            msg = "";
            status = 0;
            var error = new byte[0x100];
            var data = new byte[] { 0xFF };
            if (!IsFloorValid(floor, out msg))
            {
                status = data[0];
                return false;
            }
            int devAddr = DriverAddr.GetFloorAddr(floor);
            if (1 == eDriver_IO.Cls_edriverdll.i2c_read(devAddr, 0x01, 1, 0, data, error))
            {
                status =  data[0];
                return true;
            }
            msg = Encoding.ASCII.GetString(error);
            return false;
        }
        protected bool EnableBiBoard(out string msg)
        {
            msg = "";
            var devAddr = DriverAddr.GetChipAddr();
            var status = new byte[0x100];
            if (1 != eDriver_IO.Cls_edriverdll.i2c_write(devAddr, 0x0F, 1, 0, new byte[] { 0x00 }, status))
            {
                msg = Encoding.ASCII.GetString(status);
                return false;
            }

            if (1 == eDriver_IO.Cls_edriverdll.i2c_write(devAddr, 0x03, 1, 0, new byte[] {0x00}, status)) return true;
            msg = Encoding.ASCII.GetString(status);
            return false;
        }


        public virtual bool ConfigI2C(out string msg)
        {
            msg = "";
            var status = new byte[0x100];
            var productInfo = new byte[256];
            if (1 != eDriver_IO.Cls_edriver_mem_dll.edriver_mem_config(productInfo, status))
            {
                msg = Encoding.ASCII.GetString(status);
                return false;
            }
            if (1 != eDriver_IO.Cls_edriver_security_dll.edriver_security_config(productInfo, status))
            {
                msg = Encoding.ASCII.GetString(status);
                return false;
            }

            if (1 != eDriver_IO.Cls_edriverdll.Edriver_Config(out status, status.Length))
            {
                msg = Encoding.ASCII.GetString(status);
                return false;
            }
            for (var floorIndex = 1; floorIndex < DriverAddr.GetFloorCount(); floorIndex++)
            {
                if (!EnableFloor(floorIndex, true, out msg)) return false;
            }
            return true;
        }
        public virtual bool SelectDut(int floor, int locate, int seat, out string msg)
        {
            msg = "";
            byte status;
            if (!ReadTargetFloor(floor, out status, out msg)) return false;
            if (status == 0)
            {
                if (!EnableFloor(floor, false, out msg)) return false;
            }
            if (!SelectBoard(floor, locate, out msg)) return false;
            if (!EnableBiBoard(out msg)) return false;
            return SelectSeat(seat, out msg);
        }
        public virtual bool EnableVoltage(int seat, bool enable, out string msg)
        {
            msg = "";
            
            var status = new byte[0x100];
            if (!IsSeatValid(seat, out msg)) return false;

            bool isVoltageAdd;
            if (!IsVoltageAdd(seat, out isVoltageAdd, out msg))return false;

            if (enable && isVoltageAdd) return true;//voltage has been added
            if (!enable && !isVoltageAdd) return true;//voltage has not been added


            var chipAddr = DriverAddr.GetChipAddr();
            var volAddr = DriverAddr.GetAddVolAddr();
            var subChipAddr = DriverAddr.GetSubChipAddr(seat);
            var ctrlAddr = DriverAddr.GetCtrlAddr(seat);
            var ctrlByte = DriverAddr.GetCtrlByte(seat);
            //Select Chip
            if (1 != eDriver_IO.Cls_edriverdll.i2c_write(chipAddr, 0x01, 1, 0, new[] { subChipAddr }, status))
            {
                msg = Encoding.ASCII.GetString(status);
                return false;
            }
            //Set Control Bit
            var data = new byte[1];
            if (1 != eDriver_IO.Cls_edriverdll.i2c_read(volAddr, ctrlAddr, 1, 0, data, status))
            {
                msg = Encoding.ASCII.GetString(status);
                return false;
            }
            var target = enable ? (byte)(data[0] & (~ctrlByte)) : (byte)(data[0] | ctrlByte);
            if (1 == eDriver_IO.Cls_edriverdll.i2c_write(volAddr, ctrlAddr, 1, 0, new[] {target}, status)) return true;
            msg = Encoding.ASCII.GetString(status);
            return false;
        }
        public virtual bool WriteParam(CmdType type,out string msg, byte[] data = null)
        {
            msg = "";
            int frame = VbmsCtrlCmd[type].Frame;
            int page = VbmsCtrlCmd[type].Page;
            int address = VbmsCtrlCmd[type].Address;
            if (data != null)
            {
                VbmsCtrlCmd[type].Data = data;
            }
            var dataLen = VbmsCtrlCmd[type].Data.Length;
            var error = new byte[0x100];
            if (1 == eDriver_IO.Cls_edriver_mem_dll.edriver_mem_write(0xA0, frame, page * 256 + address, dataLen, 1,
                    dataLen * 8 - 1, dataLen * 8, 0, VbmsCtrlCmd[type].Data, error)) return true;
            msg = Encoding.ASCII.GetString(error);
            return false;
        }
        public virtual bool ReadParam(CmdType type, out byte[] data, out string msg)
        {
            msg = "";
            data = new byte[VbmsCtrlCmd[type].DataLen];
            int frame = VbmsCtrlCmd[type].Frame;
            int page = VbmsCtrlCmd[type].Page;
            int address = VbmsCtrlCmd[type].Address;
            var error = new byte[0x100];
            if (1 == eDriver_IO.Cls_edriver_mem_dll.edriver_mem_read(0xA0, frame, page * 256 + address, data.Length, 1,
                    data.Length * 8 - 1, data.Length * 8, 0, data, error)) return true;
            msg = Encoding.ASCII.GetString(error);
            return false;
        }
        public  virtual bool SendPassword(out string msg)
        {
            msg = "";
            var status = new byte[0x100];
            if (1 == eDriver_IO.Cls_edriver_security_dll.Edriver_Finisar_Password_EN(0xA0, 0xA0, out status,
                    status.Length)) return true;
            msg = Encoding.ASCII.GetString(status);
            return false;
        }
    }
}
