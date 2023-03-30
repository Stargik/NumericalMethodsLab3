using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcEqs
{
    internal class RelaxationCalc : IEqsFunction
    {
        double a;
        double b;
        double m1;
        double m2;
        double t;
        double q;
        bool mode;
        double eps;
        Ilogger logger;

        public RelaxationCalc(double a, double b, double m1, double m2, double eps, bool mode, Ilogger logger)
        {
            this.a = a;
            this.b = b;
            this.m1 = m1;
            this.m2 = m2;
            this.eps = eps;
            this.logger = logger;
            this.mode = mode;
        }

        public double Calc(Func<double, double> func, double x0)
        {
            t = 2.0 / (m2 + m1);
            q = (m2 - m1) / (m2 + m1);
            double z = Math.Abs(x0 - a) > Math.Abs(x0 - b) ? Math.Abs(x0 - a) : Math.Abs(x0 - b);
            int prevN = (int)Math.Truncate(Math.Log(z/eps)/Math.Log(1/q)) + 1;
            double res;
            if (mode)
            {
                res = PositiveIteration(func, x0);
            }
            else
            {
                res = NegativeIteration(func, x0);
            }
            logger.NCountLog("Апріорна оцінка", prevN);
            return res;
        }

        double PositiveIteration(Func<double, double> func, double x0)
        {
            int n = 0;
            double xNext, xPrev, xBuf;
            xPrev = x0;
            xNext = xPrev + t * func(x0);
            logger.Log(xNext, Math.Abs(xNext - xPrev));
            n++;
            while (Math.Abs(xNext - xPrev) > eps)
            {
                xBuf = xNext;
                xNext = xNext + t * func(xNext);
                xPrev = xBuf;
                n++;
                logger.Log(xNext, Math.Abs(xNext - xPrev));
            }
            logger.NCountLog("Апостеріорна оцінка", n);
            return xNext;
        }
        double NegativeIteration(Func<double, double> func, double x0)
        {
            int n = 0;
            double xNext, xPrev, xBuf;
            xPrev = x0;
            xNext = x0 - t * func(x0);
            logger.Log(xNext, Math.Abs(xNext - xPrev));
            n++;
            while (Math.Abs(xNext - xPrev) > eps)
            {
                xBuf = xNext;
                xNext = xNext - t * func(xNext);
                xPrev = xBuf;
                n++;
                logger.Log(xNext, Math.Abs(xNext - xPrev));
            }
            logger.NCountLog("Апостеріорна оцінка", n);
            return xNext;
        }

    }
}
