import matplotlib.pyplot as plt
import matplotlib.animation as animation
import numpy as np
import os

#path = "../data/" #Binary file path for Windows, macOS or Linux
path = "/mnt/c/Users/Your_username/Desktop/data/" #Binary file path for WSL2
if not os.path.exists(path):
    os.makedirs(path)

def InitializeData(CFL = 0.9, evolutionTime = 0.0, n = 200, L = 1.0, gamma = 1.4):
    parameters = np.array([np.double(CFL), np.double(evolutionTime), np.double(n), np.double(L), np.double(gamma)])
    parameters.tofile(os.path.join(path, "0BinaryParameters.bin"))
    N = n+2
    dL = L/n

    density = np.zeros(N)
    velocity = np.zeros(N)
    pressure = np.zeros(N)
    x = np.linspace(-dL/2, L+dL/2, N)

    density = np.where(x<0.5, 1.0, 0.125)
    velocity = np.where(x<0.5, 0.0, 0.0)
    pressure = np.where(x<0.5, 1.0, 0.1)

    flattenPrimitive = np.concatenate([
    np.double(density.flatten()), 
    np.double(velocity.flatten()), 
    np.double(pressure.flatten())])
    flattenPrimitive.tofile(os.path.join(path, "0BinaryVariables.bin"))

def DrawData(path = path):
    def sortInteger(fileName):
        try:
            step = fileName.split("Binary")[0] 
            return int(step)
        except (ValueError, IndexError):
            return 

    fileList = [file for file in os.listdir(path) if file.endswith(".bin")]
    sortedFileList = sorted(fileList, key=sortInteger)
    stepNameList = []
    for i in sortedFileList:
        if i.endswith("Parameters.bin"):
            stepNameList.append(i.split("Binary")[0] )

    parametersHistory = []
    variablesHistory = []
    for file in sortedFileList:
        if file.endswith("Parameters.bin"):
            parameters = np.fromfile(path+file)
            parametersHistory.append(parameters)
        elif file.endswith("Variables.bin"):
            flattenVariables = np.fromfile(path+file)
            variables = flattenVariables.reshape((3, -1))
            variablesHistory.append(variables)

    for i in range(len(parametersHistory)):
        n = np.int32(parametersHistory[i][2])
        L = parametersHistory[i][3]
        rho = variablesHistory[i][0]
        v = variablesHistory[i][1]
        p = variablesHistory[i][2] 

        N = n+2
        dL = L/n 
        x = np.linspace(-dL/2, L+dL/2, N) 
        
        figProfile = plt.figure(figsize=(8, 6))
        plt.subplot(311)
        plt.plot(x, rho, '-', lw=2)
        plt.ylabel('Density')
        plt.grid(True)
        plt.subplot(312)
        plt.plot(x, v, '-', lw=2)
        plt.ylabel('Velocity')
        plt.grid(True)
        plt.subplot(313)
        plt.plot(x, p, '-', lw=2)
        plt.ylabel('Pressure')
        plt.grid(True)
        plt.tight_layout()
        plt.savefig(path+"fig"+str(stepNameList[i])+".png")
        plt.close()
        figProfile.clf()

#InitializeData()
#DrawData()