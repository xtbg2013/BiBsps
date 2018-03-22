using System;

namespace BiBsps.BiDrivers.TosaBosa
{
    internal class BiConvert
    {
        public static double ConvertDacToIcc(int dac,double currentk,double currentb )
        {
            return (Convert.ToDouble(dac) * currentk + currentb) * 1000;
        }
        public static double ConvertDacToITec(int dac,double readIteck,double readItecb)
        {
            return Convert.ToDouble(dac) * readIteck + readItecb;
        }
        public static double ConvertDacToTemp(int dac)
        {
            double data = 3930 / Math.Log((((-0.0007 * (Convert.ToDouble(dac)) + 2.6116) / (2.5151 - (-0.0007 * Convert.ToDouble(dac) + 2.6116))) * 1000 / 0.018855621), Math.E) - 273.15;
            return data;
        }
        public static int ConvertTempToDac(double temperature,double setTemperaturek,double setTemperatureb)
        {
            var t = temperature;
            var r = 0.018855621 * Math.Exp(3930.0 * (1.0 / (Convert.ToDouble(t) + 273.15)));
            var v = 2.515 * r * 0.001 / (1 + r * 0.001);
            var k = setTemperaturek;
            var b = setTemperatureb;
            var tecDac = Convert.ToInt32((v - b) / k) * 0x10;
            return tecDac;
        }
        public static double ConvertToLdi(double icc, double icc0,double deltaIcck,double deltaIccb)
        {
            var ldi = (icc - icc0) * deltaIcck + deltaIccb;
            return ldi;
        }
    }
}
