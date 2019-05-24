using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mill_AI {
    class Tools {
        public static string IntToStringWithZeros(int number) {
            if(number < 10) {
                return "0" + number;
            }

            return number.ToString();
        }
    }
}
