
namespace Utilities;

public enum Parameters { CFL, evolutionTime, numberOfCells, domainLength, heatCapacityRatio };
public enum PrimitiveVariables { density, velocity, pressure };
public enum ConservedVariables { density, momentum, energy };

public static class Slicing2DArray
{
    public static double[] ColumnSlicing(int columnIndex, double[,] array2D)
    {
        double[] columnVector = new double[array2D.GetLength(0)];
        for (int i = 0; i < array2D.GetLength(0); i++)
        {
            columnVector[i] = array2D[i, columnIndex];
        }
        return columnVector;
    }

    public static double[] RowSlicing(int rowIndex, double[,] array2D)
    {
        double[] rowVector = new double[array2D.GetLength(1)];
        for (int i = 0; i < array2D.GetLength(1); i++)
        {
            rowVector[i] = array2D[rowIndex, i];
        }
        return rowVector;
    }
}