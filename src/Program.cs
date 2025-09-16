
using CUDA;
using GlobalVariables;
using IO;
using RiemannSolvers;
using Settings;
using Utilities;
using System.Diagnostics;

Console.WriteLine("Hello! Wellcome to use this software to simulate your own one-dimensional hydrodynamics.\n\n");

string? initialStep;
while (true)
{
    Console.WriteLine("Please input your initial step:");
    initialStep = Console.ReadLine();
    try
    {
        int intInitialStep = Convert.ToInt32(initialStep);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.WriteLine("The file name must be an integer!\n\n");
        continue;
    }
    break;
}

GridAndModel parameter = new GridAndModel(initialStep!);

double endTime;
while (true)
{
    Console.WriteLine("\nPlease input the value of the end time for your simulation:");
    try
    {
        endTime = Convert.ToDouble(Console.ReadLine());
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.WriteLine("The value must be an number!\n\n");
        continue;
    }
    
    if (endTime <= 0)
    {
        Console.WriteLine("The value must be larger than zero!\n\n");

    }
    else if (parameter.InitialTime > endTime)
    {
        Console.WriteLine("It's impossible that initial time is larger than end time.");
        continue;
    }
    else
        break;
}

HLL solver = new HLL();
double[,] PrimitiveField = new double[Enum.GetValues<PrimitiveVariables>().Length, parameter.TotalNumberOfCells];
BinaryIO.InputVariables(initialStep!, PrimitiveField);

CUDADriver? accelerator;
if (OperatingSystem.IsLinux() && CUDADriver.IsNvidiaGPUPresent())
{
    Console.WriteLine("\nDo you want parallel computing with CUDA? Please press Y/y if you want or any other key if not.");
    string? CUDASwitch = Console.ReadLine();
    if (CUDASwitch!.ToUpper() == "Y")
    {
        accelerator = new CUDADriver();
        Console.WriteLine("CUDA is activated!\n\n");
    }
    else
    {
        accelerator = null;
        Console.WriteLine("CUDA isn't activated!\n\n");
    }
}
else accelerator = null;

Console.WriteLine(@"
Congratulations! All initial settings are already set up successfully.
Your simulation starts now!" + "\n\n");

double time = parameter.InitialTime;
int step = Convert.ToInt32(initialStep);
Stopwatch CPUWatch = new Stopwatch(); CPUWatch.Start();

if (accelerator == null)
{
    while (time < endTime)
    {
        GlobalFlow fluid = new GlobalFlow(parameter, solver, PrimitiveField);

        double dL = parameter.CellLength;
        double dt;

        if (fluid.MaxWaveSpeed.Max() == 0.0) dt = 1e-9;
        else dt = parameter.CFL * dL / fluid.MaxWaveSpeed.Max();
        if (time + dt > endTime) dt = endTime - time;

        PrimitiveField = fluid.GetUpdatedPrimitiveField(PrimitiveField, dt);

        time += dt;
        step += 1;

        double[] parameters = [parameter.CFL, time, parameter.NumberOfCells, parameter.DomainLength, parameter.HeatCapacityRatio];
        BinaryIO.Output(step.ToString(), parameters, PrimitiveField);

        Console.WriteLine($"Evolution time in this step {step}: " + time.ToString());
    }
}
else
{
    int fileName = step;
    int n = parameter.NumberOfCells;
    int N = parameter.TotalNumberOfCells;
    double dL = parameter.CellLength;
    double gamma = parameter.HeatCapacityRatio;
    double CFL = parameter.CFL;
    double[,,] NewPrimitiveField = new double[100, Enum.GetValues<PrimitiveVariables>().Length, N];
    double[] timeList = new double[100];
    CUDALibrary.CUDAUpdateFlow(PrimitiveField, NewPrimitiveField, ref n, ref N, ref dL, ref gamma, ref CFL, ref endTime, ref time, ref step, timeList);

    if (step % 100 != 0) step -= step / 100 * 100;
    else step -= (step / 100 - 1) * 100;
    for (int i = 1; i <= step; i++)
    {
        for (int j = 0; j < Enum.GetValues<PrimitiveVariables>().Length; j++)
        {
            for (int xi = 0; xi < N; xi++)
                PrimitiveField[j, xi] = NewPrimitiveField[i - 1, j, xi];
        }

        fileName += 1;
        double[] parameters = [CFL, timeList[i - 1], n, parameter.DomainLength, gamma];
        BinaryIO.Output(fileName.ToString(), parameters, PrimitiveField);
    }
}

CPUWatch.Stop(); Console.WriteLine("Runtime for this simulation: " + CPUWatch.ToString() + "\n");
Console.WriteLine("Simulation finished! Please press any key to exit!");
Console.ReadKey(true);
