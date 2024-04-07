using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Common
{
    public static class RandomExtensions 
    {
        public static double NextDouble(this Random random, double minValue, double maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), "minValue cannot be bigger than maxValue");
            }
            double x = random.NextDouble();
            return x * maxValue + (1 - x) * minValue;
        }
    }
}
