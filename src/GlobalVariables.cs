
using Settings;
using static LocalVariables.LocalConvertor;
using static LocalVariables.LocalFace;
using RiemannSolvers;
using Utilities;
using static Utilities.Slicing2DArray;

namespace GlobalVariables;

public class GlobalFlow
{
    private GridAndModel _parameter;
    private HLL _solver;
    private double _heatCapacityRatio;
    public double[] MaxWaveSpeed { get; private set; }
    public double[,] NumericalFlux { get; private set; }

    public GlobalFlow(
    GridAndModel parameter,
    HLL solver,
    double[,] primitiveField)
    {
        _parameter = parameter;
        int n = _parameter.NumberOfCells;
        _solver = solver;
        _heatCapacityRatio = _parameter.HeatCapacityRatio;
        MaxWaveSpeed = new double[n + 1];
        NumericalFlux = new double[Enum.GetValues<PrimitiveVariables>().Length, n + 1];

        SetNumericalFlux(primitiveField);
    }

    public void SetNumericalFlux(double[,] primitiveField)
    {
        int n = _parameter.NumberOfCells;
        
        Parallel.For(0, n + 1, xi =>
        {
            double[] WL = ColumnSlicing(xi, primitiveField);
            double vL = WL[1];
            double cL = GetSoundSpeed(WL, _heatCapacityRatio);

            double[] WR = ColumnSlicing(xi + 1, primitiveField);
            double vR = WR[1];
            double cR = GetSoundSpeed(WR, _heatCapacityRatio);

            double SL = Math.Min(vL - cL, vR - cR);
            double SR = Math.Max(vL + cL, vR + cR);
            MaxWaveSpeed[xi] = Math.Max(Math.Abs(SL), Math.Abs(SR));

            for (int i = 0; i < primitiveField.GetLength(0); i++)
            {
                NumericalFlux[i, xi] = _solver.GetNumericalFlux(i, WL, WR, SL, SR, _heatCapacityRatio);
            }
        });
    }

    public double[,] GetUpdatedPrimitiveField(double[,] primitiveField, double timeStep)
    {
        int n = _parameter.NumberOfCells;
        int N = _parameter.TotalNumberOfCells;
        double dL = _parameter.CellLength;
        double dt = timeStep;

        double[,] newPrimitiveField = new double[Enum.GetValues<ConservedVariables>().Length, N];

        Parallel.For(0, n, xi =>
        {
            double[] newU = new double[Enum.GetValues<ConservedVariables>().Length];
            for (int i = 0; i < Enum.GetValues<ConservedVariables>().Length; i++)
            {
                double u = PrimitiveToConserved(i, ColumnSlicing(xi + 1, primitiveField), _heatCapacityRatio);
                double LeftF = ColumnSlicing(xi, NumericalFlux)[i];
                double RightF = ColumnSlicing(xi + 1, NumericalFlux)[i];

                newU[i] = u - dt / dL * (RightF - LeftF);
            }

            for (int i = 0; i < Enum.GetValues<ConservedVariables>().Length; i++)
            {
                newPrimitiveField[i, xi + 1] = ConservedToPrimitive(i, newU, _heatCapacityRatio);
                if (xi == 0)
                    newPrimitiveField[i, 0] = newPrimitiveField[i, xi + 1];
                else if (xi == n - 1)
                    newPrimitiveField[i, N - 1] = newPrimitiveField[i, xi + 1];
                else
                    continue;
            }
        });

        return newPrimitiveField;
    }
}