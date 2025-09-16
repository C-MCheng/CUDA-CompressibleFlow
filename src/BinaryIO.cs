
using Utilities;
using System.Diagnostics;

namespace IO;

public static class BinaryIO
{
    public static void InputParameters(string stepName, double[] parameters)
    {
        try
        {
            string baseDirectory;
            string filePath;
            string? wslDistroName = Environment.GetEnvironmentVariable("WSL_DISTRO_NAME"); 

            if (!string.IsNullOrEmpty(wslDistroName))
            {
                filePath = Path.Combine(GetDesktopPath(), "data", stepName + "BinaryParameters.bin");
            }
            else
            {
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory; 
                filePath = Path.Combine(baseDirectory, "data", stepName + "BinaryParameters.bin");
            }

            using var binaryParameters = new BinaryReader(File.Open(filePath, FileMode.Open));
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = binaryParameters.ReadDouble();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Please check initial binary file in data folder! Please press any key to exit!");
            Console.ReadKey(true);
            Environment.Exit(-1);
        }
    }

    public static void InputVariables(string stepName, double[,] primitiveFieid)
    {
        try
        {
            string baseDirectory;
            string filePath;
            string? wslDistroName = Environment.GetEnvironmentVariable("WSL_DISTRO_NAME"); 

            if (!string.IsNullOrEmpty(wslDistroName))
            {
                filePath = Path.Combine(GetDesktopPath(), "data", stepName + "BinaryVariables.bin");
            }
            else
            {
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory; 
                filePath = Path.Combine(baseDirectory, "data", stepName + "BinaryVariables.bin");
            }
            
            using var binaryVariables = new BinaryReader(File.Open(filePath, FileMode.Open));
            double[] flattenVariables = new double[primitiveFieid.GetLength(0) * primitiveFieid.GetLength(1)];

            for (int i = 0; i < primitiveFieid.GetLength(0) * primitiveFieid.GetLength(1); i++)
            {
                flattenVariables[i] = binaryVariables.ReadDouble();
            }
            
            for (int i = 0; i < primitiveFieid.GetLength(0); i++)
            {
                for (int xi = 0; xi < primitiveFieid.GetLength(1); xi++)
                {
                    primitiveFieid[i, xi] = flattenVariables[i * primitiveFieid.GetLength(1) + xi];
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Please check initial binary file in data folder! Please press any key to exit!");
            Console.ReadKey(true);
            Environment.Exit(-1);
        }
    }

    public static void Output(string stepName, double[] parameters, double[,] primitiveFieid)
    {
        string baseDirectory; 
        string parametersPath;
        string variablesPath;
        string? wslDistroName = Environment.GetEnvironmentVariable("WSL_DISTRO_NAME");

        if (!string.IsNullOrEmpty(wslDistroName))
        {
            parametersPath = Path.Combine(GetDesktopPath(), "data", stepName + "BinaryParameters.bin");
            variablesPath = Path.Combine(GetDesktopPath(), "data", stepName + "BinaryVariables.bin");
        }
        else
        {
            baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            parametersPath = Path.Combine(baseDirectory, "data", stepName + "BinaryParameters.bin");
            variablesPath = Path.Combine(baseDirectory, "data", stepName + "BinaryVariables.bin"); 
        }

        using BinaryWriter parametersWriter = new BinaryWriter(File.Open(parametersPath, FileMode.Create));
        foreach (double value in parameters)
        {
            parametersWriter.Write(value);
        }

        double[] flattenVariables = new double[primitiveFieid.GetLength(0) * primitiveFieid.GetLength(1)];
        for (int i = 0; i < primitiveFieid.GetLength(0); i++)
        {
            for (int xi = 0; xi < primitiveFieid.GetLength(1); xi++)
            {
                flattenVariables[i * primitiveFieid.GetLength(1) + xi] = primitiveFieid[i, xi];
            }
        }

        using BinaryWriter variablesWriter = new BinaryWriter(File.Open(variablesPath, FileMode.Create));
        foreach (double value in flattenVariables)
        {
            variablesWriter.Write(value);
        }
    }

    public static string GetDesktopPath()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "/mnt/c/Windows/System32/WindowsPowerShell/v1.0/powershell.exe",
            Arguments = "-Command \"(Get-ItemPropertyValue 'HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders' -Name 'Desktop')\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();
            string output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();

            return output.Replace(@"\", @"/").Replace("C:", "/mnt/c");
        }
    }

}