using System.Linq;

namespace BiBsps.BiGlobalFiies
{
    public static class Calculation
    {
        public static double Intercept(double[] y, double[] x)
        {
            return Average(y) - Slope(y, x) * Average(x);
        }
        public static double Slope(double[] y, double[] x)
        {
            var avgXy = Average(Mux(x, y));
            var xAvgYAvg = Average(x) * Average(y);
            var avgXx = Average(Mux(x, x));
            var xAvgxAvg = Average(x) * Average(x);
            return (avgXy - xAvgYAvg) / (avgXx - xAvgxAvg);
        }
        public static double Average(double[] x)
        {
            var sum = x.Sum();
            return sum / x.Length;
        }
        public static double[] Mux(double[] x, double[] y)
        {
            double[] ret = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
                ret[i] = x[i] * y[i];
            return ret;
        }
        
        public static T MidValue<T>(T[] currentList)
        {
            var orderList = currentList.ToList();
            orderList.Sort();
            return orderList[orderList.Count / 2];
        }
    }
}
