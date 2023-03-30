using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcEqs
{
    internal class ConsoleLogger : Ilogger
    {
        public void Log(double xn, double funcxn)
        {
            Console.WriteLine($"{xn, 26} ## {funcxn, 26}");
        }

        public void NCountLog(string mes, int n)
        {
            Console.WriteLine($"{mes}: {n}");
        }
    }
}
