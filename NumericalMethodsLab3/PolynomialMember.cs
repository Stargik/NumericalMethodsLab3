using System;
namespace NumericalMethodsLab3
{
    public class PolynomialMember : ICloneable
    {
        public double Degree { get; set; }
        public double Coefficient { get; set; }

        public PolynomialMember(double degree, double coefficient)
        {
            this.Degree = degree;
            this.Coefficient = coefficient;
        }

        public object Clone()
        {
            return new PolynomialMember(Degree, Coefficient);
        }
    }

}

