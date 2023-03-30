using System;
namespace NumericalMethodsLab3.Exceptions
{
    public class PolynomialArgumentNullException : Exception
    {
        public PolynomialArgumentNullException() { }

        public PolynomialArgumentNullException(string message) { }
        protected PolynomialArgumentNullException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {

        }
    }

}

