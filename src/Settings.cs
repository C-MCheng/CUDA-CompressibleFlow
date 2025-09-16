
using IO;
using Utilities;

namespace Settings;

public class GridAndModel
{
    public double CFL { get; protected set; }
    public double InitialTime { get; protected set; }
    public int NumberOfCells { get; private set; }
    public double DomainLength { get; private set; }
    public int TotalNumberOfCells { get; private set; }
    public double CellLength { get; private set; }
    public double HeatCapacityRatio { get; private set; }
    public GridAndModel(string initialStepName)
    {
        double[] parametersArray = new double[Enum.GetValues<Parameters>().Length];
        BinaryIO.InputParameters(initialStepName, parametersArray);

        CFL = parametersArray[(int)Parameters.CFL];
        InitialTime = parametersArray[(int)Parameters.evolutionTime];
        NumberOfCells = (int)parametersArray[(int)Parameters.numberOfCells];
        DomainLength = parametersArray[(int)Parameters.domainLength];
        TotalNumberOfCells = NumberOfCells + 2;
        CellLength = DomainLength / NumberOfCells;
        HeatCapacityRatio = parametersArray[(int)Parameters.heatCapacityRatio];
    }
}