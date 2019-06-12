using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    static class Tools {
        public static string IntToStringWithZeros(int number) {
            if(number < 10) {
                return "0" + number;
            }

            return number.ToString();
        }

        public static double ElapsedSeconds(this Stopwatch stopwatch) => stopwatch.ElapsedMilliseconds / 1000d;
    }
}
