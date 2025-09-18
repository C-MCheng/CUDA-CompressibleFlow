# CUDA-CompressibleFlow
## Introduction
This software is to simulate one-dimensional compressible fluid dynamics.
## Requirements
- OS: Windows-x64 | Linux-x64 | macOS-arm64
- Graphics: Nvidia GPU (If you want to switch on CUDA-acceleration.)
- Programming skills: Python and NumPy
## Software features
- Console APP
- Binary file I/O
- CPU Parallel computing
- GPU Parallel computing (only for Linux-x64 or Windows Subsystem for Linux 2 (WSL2) on Windows-x64)
## Numerical algorithms
- Finite volume method
- Piecewise constant reconstruction
- HLLE solver
- Euler method
## User guide
### 1. Writing initial conditions into a binary file
The most difficult thing for users is to write a initial state of fluid into a binary file. However, don't worry, it's not too hard if you ara able to write Python and NumPy. 
Although many languages are able to read/write binary files, Python is easiest such that I will teach you how to write with Python.

Sod shock tube is a classical test problem for 1D compressible flow so let's learn how to write the inital binary file through this pedagogical example.
I had put a complete Python code in examples folder in this repository, please refer it.

First, you need to set five parameters:
- CFL: This is the Courant–Friedrichs–Lewy number which is lower than 1.0. The lower the CFL is, the more stable numerically the simulation is.
- evolutionTime: The evolution time means a time interval the fluid flows from start in the simuation. For initial condition, we set 0.0 usually.
- n: n is the number of cells. The space domain is divided by these cells and each cell has a coordinate and physical variables. The more the the number of cells, the more accurate numerically the simulation is.
- L: The length of the space domain.
- gamma: gamma is heat capacity ratio of fluid. For ideal gas, gamma is 5.0/3.0 and 1.4 for standard Sod shock tube test.

You need to save this five numbers as a NumPy array, then output a binary file. Please note that their type is double, `numpy.double()` can covert a numeber into a double. About the order of parameters in a NumPy array, please follow the code: 
```
parameters = np.array([np.double(CFL), np.double(evolutionTime), np.double(n), np.double(L), np.double(gamma)])
``` 
Then, you output a binary file saving parameters and the file name is the number of step plus BinaryParameters.bin. We set the number of initial step as 0 usually:
```
parameters.tofile(os.path.join(path, "0BinaryParameters.bin"))
```
`path` is the binary files output path. In this software, for not WSL2 OS, binary files are saved in the folder named data placed in the path includeing exe file and, for WSL2, in the desktop.
Because of storage limit of WSL2, saving binary files in desktop is more convenient.

Numerical algorithms adopted in this software requires one ghost cell at each endpoint so the total number of cells is `N = n + 2` but the space domain excludes ghost cell such that the cell length is `dL = L/n`.
According to this ghost cell setting, you write down coordinates of each cells with NumPy: `x = np.linspace(-dL/2, L+dL/2, N)`.

Second, you need to set three variables. They are physics fields with N data:
```
density = np.zeros(N)
velocity = np.zeros(N)
pressure = np.zeros(N)
```
Now, we want to simulate Sod shock tube so set initial conditions for Sod shock tube in the domain $x\in[0, 1]$ (It includes ghost cells in numerical simulations.):

$$
\begin{align*} 
&\rho(x)=\begin{cases}
1.0, &x<0.5\\
0.125, &x\ge0.5\\
\end{cases}\\
&v(x)=0\\
&p(x)=\begin{cases}
1.0, &x<0.5\\
0.1, &x\ge0.5\\
\end{cases}
\end{align*}
$$

We fill above values into three fields with `numpy.where()`. Please note that the length of `x` must equal to of three field arrays:
```
density = np.where(x<0.5, 1.0, 0.125)
velocity = np.where(x<0.5, 0.0, 0.0)
pressure = np.where(x<0.5, 1.0, 0.1)
```
Before output field varables data, you need to connect them into a flattened array in the order of density, velocity and pressure due to the data structure of the binary file:
```
flattenPrimitive = np.concatenate([
np.double(density), 
np.double(velocity), 
np.double(pressure)])
```
Then, you output a binary file saving variables and the file name is the number of step plus BinaryVariables.bin. We set the number of initial step as 0 usually. The `path` is the same as BinaryParameters.bin:
```
flattenPrimitive.tofile(os.path.join(path, "0BinaryVariables.bin"))
```
Now, you have initial binary files 0BinaryParameters.bin and 0BinaryVariables.bin in proper path and next we click exe file to start the simulation.
### 2. Running a simulation
### 3. Reading and analyzing results
### 4. Restarting a simulaiton
## Performance
