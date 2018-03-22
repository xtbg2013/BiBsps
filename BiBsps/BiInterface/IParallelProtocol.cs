namespace BiBsps.BiInterface
{
    public enum CmdType
    {
        ReadRegisterStatus,
        ReadSn,
        ReadBias0,
        ReadBias1,
        ReadBias2,
        ReadBias3,
        ReadTemperature,
        ReadMode,
        WriteBurninMode,
        WriteEngineerMode,
        WritePeralignMode,
        WriteChanBiasManualMode0,
        WriteChanBias0,
        WriteChanBiasManualMode1,
        WriteChanBias1,
        WriteChanBiasManualMode2,
        WriteChanBias2,
        WriteChanBiasManualMode3,
        WriteChanBias3,
        WriteChanModulaitonManualMode0,
        WriteChanModulaitonMod0,
        WriteChanModulaitonManualMode1,
        WriteChanModulaitonMod1,
        WriteChanModulaitonManualMode2,
        WriteChanModulaitonMod2,
        WriteChanModulaitonManualMode3,
        WriteChanModulaitonMod3,
        WriteChanPolarity0,
        WriteChanPolarity1,
        WriteChanPolarity2,
        WriteChanPolarity3,
        WriteEnableAllChan,
        WriteDisableAllChan,

        WriteRegisterAddr,
        WriteRegisterLen,
        WriteRegisterVal,
        WriteRegisterReboot
    }

    public interface IParallelProtocol
    {
        bool ConfigI2C(out string msg);
        bool SelectDut(int floor, int locate, int seat, out string msg);
        bool EnableVoltage(int seat,bool enable, out string msg);
        bool WriteParam(CmdType type, out string msg, byte[] data = null);
        bool ReadParam(CmdType type, out byte[] data, out string msg);
        bool SendPassword(out string msg);
    }
}
