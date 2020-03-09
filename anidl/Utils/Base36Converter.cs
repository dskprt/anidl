using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anidl.Utils {

    public class Base36Converter {

        private const int Base = 36;
        private const string Chars = "0123456789abcdefghijklmnopqrstuvwxyz";

        public static string ConvertTo(int value) {
            string result = "";

            while (value > 0) {
                result = Chars[value % Base] + result; // use StringBuilder for better performance
                value /= Base;
            }

            return result;
        }
    }
}
