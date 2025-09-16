
using static LocalVariables.LocalConvertor;
using static LocalVariables.LocalFace;

namespace RiemannSolvers;

public class HLL
{
    public double GetNumericalFlux(int variableIndex,
    double[] leftStatePrimitiveVector, double[] rightStatePrimitiveVector,
    double leftStateMaxWaveSpeed, double rightStateMaxWaveSpeed,
    double heatCapacityRatio)
    {
        double[] WL = leftStatePrimitiveVector;
        double[] WR = rightStatePrimitiveVector;
        double SL = leftStateMaxWaveSpeed;
        double SR = rightStateMaxWaveSpeed;
        double gamma = heatCapacityRatio;

        if (SL >= 0) return GetFlux(variableIndex, WL, gamma);
        else if (SL < 0 && 0 < SR)
        {
            double UL = PrimitiveToConserved(variableIndex, WL, gamma);
            double UR = PrimitiveToConserved(variableIndex, WR, gamma);
            double FL = GetFlux(variableIndex, WL, gamma);
            double FR = GetFlux(variableIndex, WR, gamma);
            return (SR * FL - SL * FR + SR * SL * (UR - UL)) / (SR - SL);
        }
        else return GetFlux(variableIndex, WR, gamma);
    }
}