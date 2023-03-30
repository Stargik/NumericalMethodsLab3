using System;
using NumericalMethodsLab3.Exceptions;

namespace NumericalMethodsLab3
{
	public class Polynomial
	{
        private readonly List<PolynomialMember> polynomial;
        public Polynomial()
        {
            polynomial = new List<PolynomialMember>();
        }

        public Polynomial(PolynomialMember member)
        {
            polynomial = new List<PolynomialMember> { member };
        }

        public Polynomial(IEnumerable<PolynomialMember> members)
        {
            polynomial = new List<PolynomialMember>();
            foreach (var member in members)
            {
                polynomial.Add(member);
            }
        }

        public Polynomial((double degree, double coefficient) member)
        {
            polynomial = new List<PolynomialMember> { new PolynomialMember(member.degree, member.coefficient) };
        }

        public Polynomial(IEnumerable<(double degree, double coefficient)> members)
        {
            polynomial = new List<PolynomialMember>();
            foreach (var member in members)
            {
                polynomial.Add(new PolynomialMember(member.degree, member.coefficient));
            }
        }

        /// <summary>
        /// The amount of not null polynomial members in polynomial 
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;
                foreach (var member in polynomial)
                {
                    if (member.Coefficient != 0)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// The biggest degree of polynomial member in polynomial
        /// </summary>
        public double Degree
        {
            get
            {
                double maxDegree = Double.MinValue;
                foreach (var member in polynomial)
                {
                    if (member.Coefficient != 0 && maxDegree < member.Degree)
                    {
                        maxDegree = member.Degree;
                    }
                }
                return maxDegree;

            }
        }

        /// <summary>
        /// Adds new unique member to polynomial 
        /// </summary>
        /// <param name="member">The member to be added</param>
        /// <exception cref="PolynomialArgumentException">Throws when member to add with such degree already exist in polynomial</exception>
        /// <exception cref="PolynomialArgumentNullException">Throws when trying to member to add is null</exception>
        public void AddMember(PolynomialMember member)
        {
            if (member == null)
            {
                throw new PolynomialArgumentNullException();
            }
            if (member.Coefficient == 0)
            {
                throw new PolynomialArgumentException();
            }
            foreach (var member2 in polynomial)
            {
                if (member2.Degree == member.Degree)
                {
                    throw new PolynomialArgumentException();
                }
            }
            polynomial.Add(member);
        }

        /// <summary>
        /// Adds new unique member to polynomial from tuple
        /// </summary>
        /// <param name="member">The member to be added</param>
        /// <exception cref="PolynomialArgumentException">Throws when member to add with such degree already exist in polynomial</exception>
        public void AddMember((double degree, double coefficient) member)
        {
            if (member.coefficient == 0)
            {
                throw new PolynomialArgumentException();
            }
            foreach (var member2 in polynomial)
            {
                if (member2.Degree == member.degree)
                {
                    throw new PolynomialArgumentException();
                }
            }
            polynomial.Add(new PolynomialMember(member.degree, member.coefficient));
        }

        /// <summary>
        /// Removes member of specified degree
        /// </summary>
        /// <param name="degree">The degree of member to be deleted</param>
        /// <returns>True if member has been deleted</returns>
        public bool RemoveMember(double degree)
        {
            foreach (var member in polynomial)
            {
                if (member.Degree == degree)
                {
                    polynomial.Remove(member);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Searches the polynomial for a method of specified degree
        /// </summary>
        /// <param name="degree">Degree of member</param>
        /// <returns>True if polynomial contains member</returns>
        public bool ContainsMember(double degree)
        {
            foreach (var member in polynomial)
            {
                if (member.Degree == degree)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Finds member of specified degree
        /// </summary>
        /// <param name="degree">Degree of member</param>
        /// <returns>Returns the found member or null</returns>
        public PolynomialMember Find(double degree)
        {
            foreach (var member in polynomial)
            {
                if (member.Degree == degree)
                {
                    return member;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets and sets the coefficient of member with provided degree
        /// If there is no null member for searched degree - return 0 for get and add new member for set
        /// </summary>
        /// <param name="degree">The degree of searched member</param>
        /// <returns>Coefficient of found member</returns>
        public double this[double degree]
        {
            get
            {
                if (ContainsMember(degree))
                {
                    return Find(degree).Coefficient;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (degree != 0)
                {
                    if (ContainsMember(degree))
                    {
                        Find(degree).Coefficient = value;
                    }
                    else
                    {
                        polynomial.Add(new PolynomialMember(degree, value));
                    }
                }
                else
                {
                    if (ContainsMember(degree))
                    {
                        polynomial.Remove(Find(degree));
                    }
                }
            }
        }

        /// <summary>
        /// Convert polynomial to array of included polynomial members 
        /// </summary>
        /// <returns>Array with not null polynomial members</returns>
        public PolynomialMember[] ToArray()
        {
            List<PolynomialMember> polynomialCopy = new List<PolynomialMember>();
            foreach (var member in polynomial)
            {
                if (member.Coefficient != 0)
                {
                    polynomialCopy.Add(member);
                }
            }
            return polynomialCopy.ToArray();
        }

        /// <summary>
        /// Adds two polynomials
        /// </summary>
        /// <param name="a">The first polynomial</param>
        /// <param name="b">The second polynomial</param>
        /// <returns>New polynomial after adding</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if either of provided polynomials is null</exception>
        public static Polynomial operator +(Polynomial a, Polynomial b)
        {

            if (a == null || b == null)
            {
                throw new PolynomialArgumentNullException();
            }
            Polynomial result = new Polynomial();
            PolynomialMember[] aArr = a.ToArray();
            PolynomialMember[] bArr = b.ToArray();
            for (int i = 0; i < aArr.Length; i++)
            {
                result.AddMember(new PolynomialMember(aArr[i].Degree, aArr[i].Coefficient));
            }
            for (int i = 0; i < bArr.Length; i++)
            {
                if (result.ContainsMember(bArr[i].Degree))
                {
                    result.Find(bArr[i].Degree).Coefficient += bArr[i].Coefficient;
                }
                else
                {
                    result.AddMember(new PolynomialMember(bArr[i].Degree, bArr[i].Coefficient));
                }
            }
            return result;
        }

        /// <summary>
        /// Subtracts two polynomials
        /// </summary>
        /// <param name="a">The first polynomial</param>
        /// <param name="b">The second polynomial</param>
        /// <returns>Returns new polynomial after subtraction</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if either of provided polynomials is null</exception>
        public static Polynomial operator -(Polynomial a, Polynomial b)
        {
            if (a == null || b == null)
            {
                throw new PolynomialArgumentNullException();
            }
            Polynomial result = new Polynomial();
            PolynomialMember[] aArr = a.ToArray();
            PolynomialMember[] bArr = b.ToArray();
            for (int i = 0; i < aArr.Length; i++)
            {
                result.AddMember(new PolynomialMember(aArr[i].Degree, aArr[i].Coefficient));
            }
            for (int i = 0; i < bArr.Length; i++)
            {
                if (result.ContainsMember(bArr[i].Degree))
                {
                    result.Find(bArr[i].Degree).Coefficient -= bArr[i].Coefficient;
                }
                else
                {
                    result.AddMember(new PolynomialMember(bArr[i].Degree, (-bArr[i].Coefficient)));
                }
            }
            return result;
        }

        /// <summary>
        /// Multiplies two polynomials
        /// </summary>
        /// <param name="a">The first polynomial</param>
        /// <param name="b">The second polynomial</param>
        /// <returns>Returns new polynomial after multiplication</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if either of provided polynomials is null</exception>
        public static Polynomial operator *(Polynomial a, Polynomial b)
        {
            if (a == null || b == null)
            {
                throw new PolynomialArgumentNullException();
            }
            Polynomial result = new Polynomial();
            PolynomialMember[] aArr = a.ToArray();
            PolynomialMember[] bArr = b.ToArray();

            for (int i = 0; i < bArr.Length; i++)
            {
                Polynomial resultAdd = new Polynomial();
                for (int j = 0; j < aArr.Length; j++)
                {
                    resultAdd.AddMember(new PolynomialMember((aArr[j].Degree + bArr[i].Degree), (aArr[j].Coefficient * bArr[i].Coefficient)));
                }
                result += resultAdd;
            }
            return result;
        }

        /// <summary>
        /// Adds polynomial to polynomial
        /// </summary>
        /// <param name="polynomial">The polynomial to add</param>
        /// <returns>Returns new polynomial after adding</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if provided polynomial is null</exception>
        public Polynomial Add(Polynomial polynomial)
        {
            return this + polynomial;
        }

        /// <summary>
        /// Adds tuple to polynomial
        /// </summary>
        /// <param name="member">The tuple to add</param>
        /// <returns>Returns new polynomial after adding</returns>
        public Polynomial Add((double degree, double coefficient) member)
        {
            return this + new Polynomial(member);
        }

        /// <summary>
        /// Subtracts polynomial from polynomial
        /// </summary>
        /// <param name="polynomial">The polynomial to subtract</param>
        /// <returns>Returns new polynomial after subtraction</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if provided polynomial is null</exception>
        public Polynomial Subtraction(Polynomial polynomial)
        {
            return this - polynomial;
        }

        /// <summary>
        /// Subtracts tuple from polynomial
        /// </summary>
        /// <param name="member">The tuple to subtract</param>
        /// <returns>Returns new polynomial after subtraction</returns>
        public Polynomial Subtraction((double degree, double coefficient) member)
        {
            return this - new Polynomial(member);
        }

        /// <summary>
        /// Multiplies polynomial with polynomial
        /// </summary>
        /// <param name="polynomial">The polynomial for multiplication </param>
        /// <returns>Returns new polynomial after multiplication</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if provided polynomial is null</exception>
        public Polynomial Multiply(Polynomial polynomial)
        {
            return this * polynomial;
        }

        /// <summary>
        /// Multiplies tuple with polynomial
        /// </summary>
        /// <param name="member">The tuple for multiplication </param>
        /// <returns>Returns new polynomial after multiplication</returns>
        public Polynomial Multiply((double degree, double coefficient) member)
        {
            return this * new Polynomial(member);
        }

        /// <summary>
        /// Adds polynomial and tuple
        /// </summary>
        /// <param name="a">The polynomial</param>
        /// <param name="b">The tuple</param>
        /// <returns>Returns new polynomial after adding</returns>
        public static Polynomial operator +(Polynomial a, (double degree, double coefficient) b)
        {
            return a + new Polynomial(b);
        }

        /// <summary>
        /// Subtract polynomial and tuple
        /// </summary>
        /// <param name="a">The polynomial</param>
        /// <param name="b">The tuple</param>
        /// <returns>Returns new polynomial after subtraction</returns>
        public static Polynomial operator -(Polynomial a, (double degree, double coefficient) b)
        {
            return a - new Polynomial(b);
        }

        /// <summary>
        /// Multiplies polynomial and tuple
        /// </summary>
        /// <param name="a">The polynomial</param>
        /// <param name="b">The tuple</param>
        /// <returns>Returns new polynomial after multiplication</returns>
        public static Polynomial operator *(Polynomial a, (double degree, double coefficient) b)
        {
            return a * new Polynomial(b);
        }


    }
}

