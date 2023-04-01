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
        #region
        const int rootCount = 10;
        double a = -1;
        double b = 0;
        double[] xVector = new double[rootCount];
        double[,] nodeTable = new double[rootCount, 2];
        Polynomial LagrangePolynomial = new Polynomial();
        Func<double, double> CurrentFunc = x => { return Math.Pow(x, 3) - 2 * Math.Pow(x, 2) + x + 1 + Math.Cos(x); };
        for (int i = 0; i < rootCount; i++)
        {
            xVector[i] = GetChebPolRoot(a, b, rootCount, i);
        }
        Array.Sort(xVector);
        for (int i = 0; i < rootCount; i++)
        {
            nodeTable[i, 0] = xVector[i];
            nodeTable[i, 1] = CurrentFunc(nodeTable[i, 0]);
        }

        Console.WriteLine("Вузли: ");
        ShowTableNode(nodeTable);
        Console.WriteLine();

        Console.WriteLine("Пряма інтерполяція: ");
        LagrangePolynomial = GetLagrangePolynomial(nodeTable, rootCount, true);
        Console.WriteLine("Інтерполяційний поліном Лагранжа (L(x)): ");
        ShowPolynomial(LagrangePolynomial, "x");
        Console.WriteLine();
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
        Console.WriteLine();
        Console.WriteLine("Рівняння: L(0) = x");
        res = LagrangePolynomial.Find(0).Coefficient;
        Console.WriteLine($"Результат: {res}");
        #endregion

        #region
        double[] hiVector = new double[rootCount - 1];
        double[] miVector = new double[rootCount];
        double[] fVector = new double[rootCount];
        double[,] matrixA = new double[rootCount - 2, rootCount - 2];
        double[,] matrixH = new double[rootCount - 2, rootCount - 1];
        double[] bVector = new double[rootCount];
        List<Polynomial> spline = new List<Polynomial>();
        for (int i = 0; i < hiVector.Length; i++)
        {
            hiVector[i] = GetHiVector(nodeTable[i, 0], nodeTable[i + 1, 0]);
        }
        Console.WriteLine();
        Console.WriteLine("Природній кубічний інтерполяційний сплайн: ");
        Console.WriteLine("hi: ");
        for (int i = 0; i < hiVector.Length; i++)
        {
            Console.Write($"h{i + 1}={hiVector[i]:f6}; ");
        }
        Console.WriteLine("\n");

        for (int i = 0; i < fVector.Length; i++)
        {
            fVector[i] = nodeTable[i, 1];
        }

        matrixA = GetMatrixA(hiVector);
        Console.WriteLine("Матриця A: ");
        ShowMatrix(matrixA);
        Console.WriteLine();

        matrixH = GetMatrixH(hiVector);
        Console.WriteLine("Матриця H: ");
        ShowMatrix(matrixH);
        Console.WriteLine();

        bVector = MultipleMatrixVector(matrixH, fVector);

        double[] resVector = RunningMethod(matrixA, bVector);
        miVector[0] = 0;
        miVector[miVector.Length - 1] = 0;
        Console.WriteLine("Am = Hf (метод прогонки):");
        Console.WriteLine("Результат: ");
        for (int i = 0; i < miVector.Length - 2; i++)
        {
            miVector[i + 1] = resVector[i];
        }
        Console.WriteLine("mi: ");
        for (int i = 0; i < miVector.Length; i++)
        {
            Console.Write($"m{i}={miVector[i]:f6}; ");
        }
        Console.WriteLine("\n");

        for (int i = 1; i < rootCount; i++)
        {
            Polynomial s = GetSplinePart(nodeTable, miVector, hiVector, i - 1, i);
            spline.Add(s);
        }
        Console.WriteLine("Сплайн: ");
        ShowSpline(spline, nodeTable);

        #endregion



        Console.Read();
    }


    public static double GetChebPolRoot(double a, double b, int n, int k)
    {
        double res = (a + b) / 2 + ((b - a) / 2) * Math.Cos((2 * k + 1) * Math.PI / (2 * n));
        return res;
    }

    public static void SortArray(double[,] arr)
    {

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
        foreach (var item in polynomial.ToArray().OrderByDescending(x => x.Degree))
        {
            if (i != 0 && item.Coefficient > 0)
            {
                if (item.Degree != 0)
                {
                    Console.Write($"+{item.Coefficient:f6}*({varLetter}^{item.Degree})");
                }
                else
                {
                    Console.Write($"+{item.Coefficient:f6}");
                }

            }
            else
            {
                if (item.Degree != 0)
                {
                    Console.Write($"{item.Coefficient:f6}*({varLetter}^{item.Degree})");
                }
                else
                {
                    Console.Write($"{item.Coefficient:f6}");
                }

            }
            i++;
        }
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

    public static double GetHiVector(double xi, double xi1)
    {
        return xi1 - xi;
    }

    public static double[,] GetMatrixA(double[] hiVector)
    {
        double[,] matrix = new double[hiVector.Length - 1, hiVector.Length - 1];
        for (int i = 0; i < hiVector.Length - 1; i++)
        {
            for (int j = 0; j < hiVector.Length - 1; j++)
            {
                if (i == j)
                {
                    matrix[i, j] = (hiVector[i] + hiVector[i + 1] / 3);
                }
                else if (i == j + 1)
                {
                    matrix[i, j] = hiVector[i] / 6;
                }
                else if (i == j - 1)
                {
                    matrix[i, j] = hiVector[j] / 6;
                }
                else
                {
                    matrix[i, j] = 0;
                }
            }
        }
        return matrix;
    }

    public static double[,] GetMatrixH(double[] hiVector)
    {
        double[,] matrix = new double[hiVector.Length - 1, hiVector.Length + 1];
        for (int i = 0; i < hiVector.Length - 1; i++)
        {
            for (int j = 0; j < hiVector.Length + 1; j++)
            {
                if (i == j)
                {
                    matrix[i, j] = 1 / hiVector[i];
                }
                else if (i == j - 1)
                {
                    matrix[i, j] = -((1 / hiVector[i]) + (1 / hiVector[j]));
                }
                else if (i == j - 2)
                {
                    matrix[i, j] = 1 / hiVector[i + 1];
                }
                else
                {
                    matrix[i, j] = 0;
                }
            }
        }
        return matrix;
    }

    public static void ShowMatrix(double[,] matrix)
    {
        int rA = matrix.GetLength(0);
        int cA = matrix.GetLength(1);
        for (int i = 0; i < rA; i++)
        {
            for (int j = 0; j < cA; j++)
            {
                Console.Write($"{matrix[i, j],10:F3}");
            }
            Console.WriteLine();
        }
    }

    public static double[,] MultipleMatrix(double[,] matrixA, double[,] matrixB)
    {
        int rA = matrixA.GetLength(0);
        int cA = matrixA.GetLength(1);
        int rB = matrixB.GetLength(0);
        int cB = matrixB.GetLength(1);
        double[,] matrixRes = new double[rA, cB];
        for (int i = 0; i < rA; i++)
        {
            for (int j = 0; j < cB; j++)
            {
                matrixRes[i, j] = 0;
                for (int k = 0; k < cA; k++)
                {
                    matrixRes[i, j] += matrixA[i, k] * matrixB[k, j];
                }
            }
        }
        return matrixRes;
    }

    public static double[] MultipleMatrixVector(double[,] matrixA, double[] vectorB)
    {
        int rA = matrixA.GetLength(0);
        int cA = matrixA.GetLength(1);
        int rB = vectorB.Length;
        double[] vectorRes = new double[rB];
        for (int i = 0; i < rA; i++)
        {
            vectorRes[i] = 0;
            for (int j = 0; j < rA; j++)
            {
                vectorRes[i] += matrixA[i, j] * vectorB[j];
            }
        }
        return vectorB;
    }

    public static double[] RunningMethod(double[,] matrixA, double[] vectorF)
    {

        double[] resVector = new double[matrixA.GetLength(0)];
        double[] aVector = new double[matrixA.GetLength(0)];
        double[] bVector = new double[matrixA.GetLength(0)];
        double[] zVector = new double[matrixA.GetLength(0)];

        aVector[1] = -(matrixA[0, 1] / matrixA[0, 0]);
        bVector[1] = vectorF[0] / matrixA[0, 0];
        zVector[1] = -matrixA[1, 1] - aVector[1] * matrixA[1, 0];

        for (int i = 2; i < aVector.Length; i++)
        {
            aVector[i] = matrixA[i - 1, i] / zVector[i - 1];
            bVector[i] = (-vectorF[i] + matrixA[i, i - 1]) / zVector[i - 1];
            zVector[i] = -matrixA[i, i] - aVector[i] * matrixA[i, i - 1];
        }
        resVector[resVector.Length - 1] = (-vectorF[resVector.Length - 1] + aVector[resVector.Length - 1] * bVector[resVector.Length - 1]) / zVector[resVector.Length - 1];
        for (int i = resVector.Length - 2; i >= 0; i--)
        {
            resVector[i] = aVector[i + 1] * resVector[i + 1] + bVector[i + 1];
        }
        return resVector;
    }

    public static Polynomial GetSplinePart(double[,] nodes, double[] miVector, double[] hiVector, int a, int b)
    {
        Polynomial s1 = new Polynomial((0, miVector[a]));
        Polynomial polynomial = new Polynomial();
        polynomial.AddMember((0, nodes[b, 0]));
        polynomial.AddMember((1, -1));
        for (int i = 0; i < 3; i++)
        {
            s1 = s1.Multiply(polynomial);
        }
        s1.Multiply((0, 1 / (6 * hiVector[b - 1])));

        Polynomial s2 = new Polynomial((0, miVector[b]));
        polynomial = new Polynomial();
        polynomial.AddMember((0, -nodes[a, 0]));
        polynomial.AddMember((1, 1));
        for (int i = 0; i < 3; i++)
        {
            s2 = s2.Multiply(polynomial);
        }
        s2 = s2.Multiply((0, 1 / (6 * hiVector[b - 1])));

        Polynomial s3 = new Polynomial((0, (nodes[a, 1] - (miVector[a] * hiVector[b - 1] * hiVector[b - 1]) / 6)));
        polynomial = new Polynomial();
        polynomial.AddMember((0, nodes[b, 0]));
        polynomial.AddMember((1, -1));
        polynomial = polynomial.Multiply((0, 1 / hiVector[b - 1]));
        s3 = s3.Multiply(polynomial);

        Polynomial s4 = new Polynomial((0, (nodes[b, 1] - (miVector[b] * hiVector[b - 1] * hiVector[b - 1]) / 6)));
        polynomial = new Polynomial();
        polynomial.AddMember((0, -nodes[a, 0]));
        polynomial.AddMember((1, 1));
        polynomial = polynomial.Multiply((0, 1 / hiVector[b - 1]));
        s4 = s4.Multiply(polynomial);

        Polynomial s = new Polynomial();
        s = s.Add(s1);
        s = s.Add(s2);
        s = s.Add(s3);
        s = s.Add(s4);

        return s;
    }

    public static void ShowSpline(List<Polynomial> spline, double[,] nodes)
    {
        int i = 0;
        int maxLength = 0;
        int startTopPosition = 0;
        Console.WriteLine("  /-");
        startTopPosition = Console.CursorTop;
        foreach (var s in spline)
        {
            if (i == spline.Count / 2)
            {
                Console.Write("/  ");
            }
            else if (i == (spline.Count / 2) + 1)
            {
                Console.Write("\\  ");
            }
            else
            {
                Console.Write(" | ");
            }
            ShowPolynomial(s, "x");
            maxLength = Console.CursorLeft > maxLength ? Console.CursorLeft : maxLength;
            Console.WriteLine(",");
            i++;
        }
        Console.WriteLine("  \\-");
        for (int j = i; j > 0; j--)
        {
            Console.SetCursorPosition(maxLength + 5, startTopPosition - j - 1);
            Console.Write($"[{nodes[i - j, 0]:f4}; {nodes[i - j + 1, 0]:f4}]");
        }
    }

    public static double GetWPolynomialRes(double x, double[,] nodes)
    {
        {
            double res = x - nodes[0, 0];
            for (int i = 1; i < nodes.GetLength(0); i++)
            {
                res *= x - nodes[i, 0];
            }
            return res;
        }
    }
}
