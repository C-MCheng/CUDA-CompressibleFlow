# CUDA-CompressibleFlow
## Introduction
This software is to simulate one-dimensional compressible fluid dynamics.
## Requirements
- OS: Windows-x64 | Linux-x64 | macOS-arm64
- Graphics: Nvidia GPU (If you want to switch on CUDA-acceleration.)
## Software features
- Console APP
- Binary file I/O
- CPU Parallel computing
- GPU Parallel computing (only for Linux-x64)
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

Second, you need to set three variables:
### 2. Running a simulation
### 3. Reading and analyzing results
### 4. Restarting a simulaiton
## Performance
