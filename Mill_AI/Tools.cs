using System.Diagnostics;

namespace Mill_AI
{
    internal static class Tools
    {
        public static string IntToStringWithZeros(int number)
        {
            if(number < 10)
            {
                return "0" + number;
            }

            return number.ToString();
        }

        public static double ElapsedSeconds(this Stopwatch stopwatch) => stopwatch.ElapsedMilliseconds / 1000d;
    }
}
