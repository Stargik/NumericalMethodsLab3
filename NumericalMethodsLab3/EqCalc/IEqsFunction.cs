namespace CalcEqs
{
    public interface IEqsFunction
    {
        double Calc(Func<double, double> func, double x0);
    }
}