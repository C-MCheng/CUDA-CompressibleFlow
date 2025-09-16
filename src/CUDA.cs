
using Settings;
using RiemannSolvers;
using Utilities;
using System.Diagnostics;
using System.Runtime.InteropServices; 

namespace CUDA;

public class CUDADriver
{
    public static bool IsNvidiaGPUPresent()
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "nvidia-smi",
                Arguments = "-q",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
                {
                    return true;
                }
                else
                {
                    Console.WriteLine(error);
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something wrong: {ex.Message}");
            return false;
        }
    }
}

public static class CUDALibrary
{
    [DllImport("Program.so", EntryPoint = "UpdateFlow", CallingConvention = CallingConvention.Cdecl)]
    public static extern void CUDAUpdateFlow(double[,] primitiveField, double[,,] primitiveFieldResult, ref int numberOfCells, ref int totalNumberOfCells,
    ref double cellLength, ref double heatCapacityRatio, ref double CFL, ref double endTime, ref double time, ref int step, double[] timeList); 
}