using System.Collections.Generic;
using BiBsps.BiInterface;

namespace BiBsps.BiData
{
    public enum FixValue
    {
        RegisterStatus = 0X8B00,
        ModeBurnin = 0X10,
        ModeEngineer = 0X20,
        ModePeralign = 0X40,
        ModeBiasManual = 0X08,
        ModeModulaitonManual = 0X10,
        PolarityEnable = 0X1,
        PolarityDisable = 0X0,
        BurnInModeAdd = 0X1411

    }
    public class VbmsCmd
    {

        private static VbmsCmd _vbmsCmd;

        public static VbmsCmd Inst()
        {
            return _vbmsCmd ?? (_vbmsCmd = new VbmsCmd());
        }

        protected VbmsCmd()
        {
        }

        public Dictionary<CmdType, VbmsCtrlData> GetQsfp28GCmd()
        {
            return new Dictionary<CmdType, VbmsCtrlData>
            {
                [CmdType.ReadRegisterStatus] = new VbmsCtrlData(0X00, 0X32, 0XE2, 2),
                [CmdType.ReadSn] = new VbmsCtrlData(0X00, 0X0, 0XC4, 16),
                [CmdType.ReadBias0] = new VbmsCtrlData(0XA0, 0X0, 0x2a, 2),
                [CmdType.ReadBias1] = new VbmsCtrlData(0XA0, 0X0, 0x2c, 2),
                [CmdType.ReadBias2] = new VbmsCtrlData(0XA0, 0X0, 0x2e, 2),
                [CmdType.ReadBias3] = new VbmsCtrlData(0XA0, 0X0, 0x30, 2),
                [CmdType.ReadTemperature] = new VbmsCtrlData(0XA0, 0X1E, 0XDE, 2),
                [CmdType.ReadMode] = new VbmsCtrlData(0XA0, 0X29, 0X80, 2),

                [CmdType.WriteBurninMode] = new VbmsCtrlData(0XA0, 0x29, 0x80, new[] { (byte)FixValue.ModeBurnin }),
                [CmdType.WriteEngineerMode] = new VbmsCtrlData(0XA0, 0x29, 0x80, new[] { (byte)FixValue.ModeEngineer }),
                [CmdType.WritePeralignMode] = new VbmsCtrlData(0XA0, 0x29, 0x80, new[] { (byte)FixValue.ModePeralign }),
                [CmdType.WriteChanBiasManualMode0] = new VbmsCtrlData(0XA0, 0X25, 0X98, new[] { (byte)FixValue.ModeBiasManual }),
                [CmdType.WriteChanBias0] = new VbmsCtrlData(0XA0, 0X25, 0X9E, new byte[] { 0X0 }),
                [CmdType.WriteChanBiasManualMode1] = new VbmsCtrlData(0XA0, 0X25, 0XAA, new[] { (byte)FixValue.ModeBiasManual }),
                [CmdType.WriteChanBias1] = new VbmsCtrlData(0XA0, 0X25, 0XB0, new byte[] { 0x0 }),
                [CmdType.WriteChanBiasManualMode2] = new VbmsCtrlData(0XA0, 0X25, 0XBC, new[] { (byte)FixValue.ModeBiasManual }),
                [CmdType.WriteChanBias2] = new VbmsCtrlData(0XA0, 0X25, 0XC2, new byte[] { 0x0 }),
                [CmdType.WriteChanBiasManualMode3] = new VbmsCtrlData(0XA0, 0X25, 0XCE, new[] { (byte)FixValue.ModeBiasManual }),
                [CmdType.WriteChanBias3] = new VbmsCtrlData(0XA0, 0X25, 0XD4, new byte[] { 0x0 }),
                [CmdType.WriteChanModulaitonManualMode0] = new VbmsCtrlData(0XA0, 0X25, 0X98, new[] { (byte)FixValue.ModeModulaitonManual }),
                [CmdType.WriteChanModulaitonMod0] = new VbmsCtrlData(0XA0, 0X25, 0X9D, new byte[] { 0x0 }),
                [CmdType.WriteChanModulaitonManualMode1] = new VbmsCtrlData(0XA0, 0X25, 0XAA, new[] { (byte)FixValue.ModeModulaitonManual }),
                [CmdType.WriteChanModulaitonMod1] = new VbmsCtrlData(0XA0, 0X25, 0XAF, new byte[] { 0x0 }),
                [CmdType.WriteChanModulaitonManualMode2] = new VbmsCtrlData(0XA0, 0X25, 0XBC, new[] { (byte)FixValue.ModeModulaitonManual }),
                [CmdType.WriteChanModulaitonMod2] = new VbmsCtrlData(0XA0, 0X25, 0XC1, new byte[] { 0x0 }),
                [CmdType.WriteChanModulaitonManualMode3] = new VbmsCtrlData(0XA0, 0X25, 0XCE, new[] { (byte)FixValue.ModeModulaitonManual }),
                [CmdType.WriteChanModulaitonMod3] = new VbmsCtrlData(0XA0, 0X25, 0XD3, new byte[] { 0x0 }),
                [CmdType.WriteChanPolarity0] = new VbmsCtrlData(0XA0, 0x19, 0x92, new byte[] { 0x0 }),
                [CmdType.WriteChanPolarity1] = new VbmsCtrlData(0XA0, 0x19, 0x98, new byte[] { 0x0 }),
                [CmdType.WriteChanPolarity2] = new VbmsCtrlData(0XA0, 0x19, 0xAA, new byte[] { 0x0 }),
                [CmdType.WriteChanPolarity3] = new VbmsCtrlData(0XA0, 0x19, 0xB0, new byte[] { 0x0 }),
                [CmdType.WriteEnableAllChan] = new VbmsCtrlData(0XA0, 0x00, 0x56, new byte[] { 0x0F }),
                [CmdType.WriteDisableAllChan] = new VbmsCtrlData(0XA0, 0x00, 0x56, new byte[] { 0x00 }),
                [CmdType.WriteRegisterAddr] = new VbmsCtrlData(0XA0, 0x32, 0xE4, new byte[] { 0X14, 0x11 }),
                [CmdType.WriteRegisterLen] = new VbmsCtrlData(0XA0, 0x32, 0xE6, new byte[] { 0x02 }),
                [CmdType.WriteRegisterVal] = new VbmsCtrlData(0XA0, 0x32, 0xE8, new[] { (byte)FixValue.ModeBurnin }),
                [CmdType.WriteRegisterReboot] = new VbmsCtrlData(0XA0, 0x32, 0xE2, new byte[] { 0x0B })
            };
        }
    }
}
