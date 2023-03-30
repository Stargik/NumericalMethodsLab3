using System;
using System.Data;

namespace NumericalMethodsLab3;

class Program
{

    static void Main(string[] args)
    {
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
        ShowTableNode(nodeTable);
        double[,] nodeTable2 = new double[3, 2];
        nodeTable2[0, 0] = 0;
        nodeTable2[1, 0] = 1;
        nodeTable2[2, 0] = 4;
        nodeTable2[0, 1] = -1;
        nodeTable2[1, 1] = 1;
        nodeTable2[2, 1] = 1;
        LagrangePolynomial = GetLagrangePolynomial(nodeTable, rootCount);
        ShowPolynomial(LagrangePolynomial);

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
    public static void ShowPolynomial(Polynomial polynomial)
    {
        int i = 0;
        foreach (var item in polynomial.ToArray())
        {
            if (i < polynomial.Count - 1)
            {
                if (item.Degree != 0)
                {
                    Console.Write($"({item.Coefficient:f6})*x^({item.Degree})+");
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
                    Console.Write($"({item.Coefficient:f6})*x^({item.Degree})");
                }
                else
                {
                    Console.Write($"({item.Coefficient:f6})");
                }
                
            }
            i++;
        }
    }

    public static Polynomial GetLagrangePolynomial(double[,] nodes, int n)
    {
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
                    if (nodes[j, 0] != 0)
                    {
                        polynomial1.AddMember((0, -nodes[j, 0]));
                    }
                    polynomial = polynomial.Multiply(polynomial1);
                }
            }

            double divider = 1;
            for (int j = 0; j < n; j++)
            {
                if (i != j)
                {
                    divider *= nodes[i, 0] - nodes[j, 0];
                }
            }
            polynomial = polynomial.Multiply((0, 1.0 / divider));
            polynomial = polynomial.Multiply((0, nodes[i, 1]));

            resPol = resPol.Add(polynomial);
        }
        return resPol;
    }
}

