using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiBsps.BiProtocols.TosaBosa
{
    internal interface IProtocol
    {
        void SelectProductType();
        void InitBurnInMode();
        void EnableVoltage(List<int> seats, bool enable);
        void EnableLaser(List<int> seats, bool enable);
        void EnableBiasSync(bool enable);
        void SetSyncBias(int value);
        void SetSyncBiasByChannel(int channel, int value);
        void SelectProduct(int position);
        bool SetSingleBiasByChannel(int channel, int value);
        void SetSingleBias(int position, int value);
        int[] ReadIccDac();
        int[] ReadItecDac();
        void ReadCurrentDacAndMpdDac(out int[] currentDac, out int[] mpdDac);
        void ReadTecTemperatureDac(out int[] tecDac, out int t1Dac, out int t2Dac);
        void SetTecTemperatureDac(int position, int tecDac);
        void CloseTecTemperature(int positon = 0);
        bool[] GetVoltageState();
    }
}
