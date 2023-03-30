using System;
using System.Data;
using CalcEqs;
using System.Text;

namespace NumericalMethodsLab3;

class Program
{

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        const int rootCount = 10;
        double a = -1;
        double b = 0;
        double[,] nodeTable = new double[rootCount, 2];
        Polynomial LagrangePolynomial = new Polynomial();
        Func<double, double> CurrentFunc = x => { return Math.Pow(x, 3) - 2 * Math.Pow(x, 2) + x + 1 + Math.Cos(x); };
        for (int i = 0; i < rootCount; i++)
        {
            nodeTable[i, 0] = GetChebPolRoot(a, b, rootCount, i);
            nodeTable[i, 1] = CurrentFunc(nodeTable[i, 0]);
        }
        Console.WriteLine("Вузли: ");
        ShowTableNode(nodeTable);
        Console.WriteLine();

        Console.WriteLine("Пряма інтерполяція: ");
        LagrangePolynomial = GetLagrangePolynomial(nodeTable, rootCount, true);
        Console.WriteLine("Інтерполяційний поліном Лагранжа (L(x)): ");
        ShowPolynomial(LagrangePolynomial, "x");
        Console.WriteLine("Рівняння: L(x) = 0");
        Console.WriteLine("Метод релаксації: ");
        Ilogger logger = new ConsoleLogger();
        IEqsFunction eqsFunction = new RelaxationCalc(-1, 0, 1, 8.83945, 1E-4, false, logger);
        Console.WriteLine($"{new String(' ', 24)}Xn ## {new String(' ', 11)}|X(n) - X(n+1)|");
        double res = eqsFunction.Calc(
            (x) => { return 0.000001 * Math.Pow(x, 9) + 0.000028 * Math.Pow(x, 8) + 0.000003 * Math.Pow(x, 7) - 0.001387 * Math.Pow(x, 6) + 0.000001 * Math.Pow(x, 5) + 0.041667 * Math.Pow(x, 4) + Math.Pow(x, 3) - 2.5 * Math.Pow(x, 2) + x + 2; },
            -0.5);
        Console.WriteLine($"Результат: {res}");
        Console.WriteLine();

        Console.WriteLine("Обернена інтерполяція: ");
        LagrangePolynomial = GetLagrangePolynomial(nodeTable, rootCount, false);
        Console.WriteLine("Інтерполяційний поліном Лагранжа (L(y)): ");
        ShowPolynomial(LagrangePolynomial, "y");
        Console.WriteLine("Рівняння: L(0) = x");
        res = LagrangePolynomial.Find(0).Coefficient;
        Console.WriteLine($"Результат: {res}");

        Console.Read();
    }

    public static double GetChebPolRoot(double a, double b, int n, int k)
    {
        double res = (a + b) / 2 + ((b - a) / 2) * Math.Cos((2 * k + 1) * Math.PI / (2 * n));
        return res;
    }
    public static void ShowTableNode(double[,] table)
    {
        for (int i = 0; i < table.GetLength(0); i++)
        {
            Console.WriteLine($"X{i}={table[i, 0],22}   |   F(X{i})={table[i, 1],22}");
        }
    }
    public static void ShowPolynomial(Polynomial polynomial, string varLetter)
    {
        int i = 0;
        foreach (var item in polynomial.ToArray())
        {
            if (i < polynomial.Count - 1)
            {
                if (item.Degree != 0)
                {
                    Console.Write($"({item.Coefficient:f6})*{varLetter}^({item.Degree})+");
                }
                else
                {
                    Console.Write($"({item.Coefficient:f6})+");
                }
                
            }
            else
            {
                if (item.Degree != 0)
                {
                    Console.Write($"({item.Coefficient:f6})*{varLetter}^({item.Degree})");
                }
                else
                {
                    Console.Write($"({item.Coefficient:f6})");
                }
                
            }
            i++;
        }
        Console.WriteLine();
    }

    public static Polynomial GetLagrangePolynomial(double[,] nodes, int n, bool isNotReverse)
    {
        int xIndex = 0, yIndex = 1;
        if (!isNotReverse)
        {
            xIndex = 1;
            yIndex = 0;
        }
        Polynomial resPol = new Polynomial();
        for (int i = 0; i < n; i++)
        {
            Polynomial polynomial = new Polynomial();
            polynomial.AddMember((0, 1));

            for (int j = 0; j < n; j++)
            {
                if (i != j)
                {
                    Polynomial polynomial1 = new Polynomial();
                    polynomial1.AddMember((1, 1));
                    if (nodes[j, xIndex] != 0)
                    {
                        polynomial1.AddMember((0, -nodes[j, xIndex]));
                    }
                    polynomial = polynomial.Multiply(polynomial1);
                }
            }

            double divider = 1;
            for (int j = 0; j < n; j++)
            {
                if (i != j)
                {
                    divider *= nodes[i, xIndex] - nodes[j, xIndex];
                }
            }
            polynomial = polynomial.Multiply((0, 1.0 / divider));
            polynomial = polynomial.Multiply((0, nodes[i, yIndex]));

            resPol = resPol.Add(polynomial);
        }
        return resPol;
    }
}

