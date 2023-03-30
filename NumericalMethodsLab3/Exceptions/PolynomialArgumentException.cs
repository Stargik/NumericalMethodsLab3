using System;
namespace NumericalMethodsLab3
{
    public class PolynomialArgumentException : Exception
    {

        public PolynomialArgumentException() { }

        public PolynomialArgumentException(string message) { }
        protected PolynomialArgumentException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {

        }
    }

}

