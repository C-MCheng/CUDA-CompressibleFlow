
using Utilities;

namespace LocalVariables;

public static class LocalConvertor
{
    public static double PrimitiveToConserved(int variableIndex, double[] primitiveVector, double heatCapacityRatio)
    {
        double rho = primitiveVector[(int)PrimitiveVariables.density];
        double v = primitiveVector[(int)PrimitiveVariables.velocity];
        double P = primitiveVector[(int)PrimitiveVariables.pressure];
        double gamma = heatCapacityRatio;

        double p = rho * v;
        double E = P / (gamma - 1) + 0.5 * rho * v * v;

        if (variableIndex == (int)ConservedVariables.density)
            return rho;
        else if (variableIndex == (int)ConservedVariables.momentum)
            return p;
        else
            return E;
    }

    public static double ConservedToPrimitive(int variableIndex, double[] conservedVector, double heatCapacityRatio)
    {
        double rho = conservedVector[(int)ConservedVariables.density];
        double p = conservedVector[(int)ConservedVariables.momentum];
        double E = conservedVector[(int)ConservedVariables.energy];
        double gamma = heatCapacityRatio;

        double v = p / rho;
        double P = (gamma - 1) * (E - 0.5 * rho * v * v);

        if (variableIndex == (int)PrimitiveVariables.density)
            return rho;
        else if (variableIndex == (int)PrimitiveVariables.velocity)
            return v;
        else
            return P;
    }
}

public static class LocalFace
{
    public static double GetFlux(int variableIndex, double[] primitiveVector, double heatCapacityRatio)
    {
        double rho = primitiveVector[(int)PrimitiveVariables.density];
        double v = primitiveVector[(int)PrimitiveVariables.velocity];
        double P = primitiveVector[(int)PrimitiveVariables.pressure];
        double gamma = heatCapacityRatio;

        double E = P / (gamma - 1) + 0.5 * rho * v * v;

        if (variableIndex == (int)PrimitiveVariables.density)
            return rho * v;
        else if (variableIndex == (int)PrimitiveVariables.velocity)
            return rho * v * v + P;
        else
            return (E + P) * v;
    }

    public static double GetSoundSpeed(double[] primitiveVector, double heatCapacityRatio)
    {
        double rho = primitiveVector[(int)PrimitiveVariables.density];
        double P = primitiveVector[(int)PrimitiveVariables.pressure];
        double gamma = heatCapacityRatio;

        return Math.Sqrt(gamma * P / rho);
    }
}