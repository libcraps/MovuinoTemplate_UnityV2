import threading
from threading import Thread
import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
import GetAngleMethods as gam
import pandas as pd
import numpy as np
import FilterMethods as fm
from scipy import signal
from scipy.spatial.transform import Rotation as R
import math
import sys
import time


class MovuinoDataSet():

    def __init__(self, filepath, nbPointfilter = 15):
        self.filepath = filepath
        self.rawData = pd.read_csv(filepath + ".csv", sep=",")

        self.nbPointFilter = nbPointfilter
        self.time = []

        # liste de vecteurs numpy
        self.acceleration = []
        self.gyroscope = []
        self.magnetometer = []

        self.listMeanAcc = [np.array([0, 0, 0])]
        self.listMeanGyr = [np.array([0, 0, 0])]
        self.listMeanMag = [np.array([0, 0, 0])]

        self.acceleration_lp = []
        self.gyroscope_lp = []
        self.magnetometer_lp = []

        self.normAcceleration = [0]
        self.normGyroscope = [0]
        self.normMagnetometer = [0]

        self.velocity = [np.array([0, 0, 0])]
        self.pos = [np.array([0, 0, 0])]
        self.ThetaGyr = [np.array([0, 0, 0])]

        self.time = list(self.rawData["time"]*0.001)
        self.rawData["time"] = self.time

        self.Te = (self.time[-1]-self.time[0])/(len(self.time))

        self.nb_line = len(self.time)

        self.compteur_line = 0

    def run(self):
        for k in range(self.nb_line):
            self.acceleration.append(np.array([self.rawData["ax"][k], self.rawData["ay"][k], self.rawData["az"][k]]))
            self.gyroscope.append(np.array([self.rawData["gx"][k], self.rawData["gy"][k], self.rawData["gz"][k]])*180/np.pi)
            self.magnetometer.append(np.array([self.rawData["mx"][k], self.rawData["my"][k], self.rawData["mz"][k]]))

            if k < self.nb_line-1:
                self.normAcceleration.append(np.linalg.norm(self.acceleration[k]))
                self.normGyroscope.append(np.linalg.norm(self.gyroscope[k]))
                self.normMagnetometer.append(np.linalg.norm(self.magnetometer[k]))

        self.acceleration = np.array(self.acceleration)
        self.gyroscope = np.array(self.gyroscope)
        self.magnetometer = np.array(self.magnetometer)

        self.velocity = EulerIntegration(self.acceleration, self.Te)
        self.ThetaGyr = EulerIntegration(self.gyroscope, self.Te)
        self.pos = EulerIntegration(self.velocity, self.Te)

        self.ThetaGyr = np.array(self.ThetaGyr)
        self.pos = np.array(self.pos)
        self.velocity = np.array(self.velocity)

        self.rawData["normAccel"] = self.normAcceleration
        self.rawData["normMag"] = self.normMagnetometer
        self.rawData["normGyr"] = self.normGyroscope

        #--------------------------- FILTER ----------------------------------
        for k in range(self.nb_line):
            self.acceleration_lp.append(fm.MeandDat(self.acceleration[k], self.nbPointFilter, self.listMeanAcc))
            self.gyroscope_lp.append(fm.MeandDat(self.gyroscope[k], self.nbPointFilter, self.listMeanGyr))
            self.magnetometer_lp.append(fm.MeandDat(self.magnetometer[k], self.nbPointFilter, self.listMeanMag))

        self.acceleration_lp = np.array(self.acceleration_lp)
        self.gyroscope_lp = np.array(self.gyroscope_lp)
        self.magnetometer_lp = np.array(self.magnetometer_lp)


    def StockIntoNewFile(self, filepath):
        self.rawData.to_csv(filepath + "_treated" + ".csv", sep=",", index=False, index_label=False)

    def PlotImage(self):
        PlotVector(self.time, self.acceleration, 'Acceleration (m/s2)', 331)
        PlotVector(self.time, self.magnetometer, 'Magnetometer', 332)
        PlotVector(self.time, self.gyroscope, 'Gyroscope (deg/s)', 333)

def EulerIntegration(Uprime, dt):
    U = [np.array([0, 0, 0])]
    n = len(Uprime)
    for k in range(n-1):
        Ux = Uprime[k][0] * dt + U[k][0]
        Uy = Uprime[k][1] * dt + U[k][1]
        Uz = Uprime[k][2] * dt + U[k][2]
        U.append(np.array([Ux, Uy, Uz]))
    return U

def PlotVector(t, v, title, pos):
    fig = plt.subplot(pos)
    fig.plot(t, v[:, 0], color="r")
    fig.plot(t, v[:, 1], color="green")
    fig.plot(t, v[:, 2], color="blue")
    fig.set_title(title)






def ConvertArray(*args):
    for arg in args:
        arg = np.array(arg)



"""
class MovuinoDataSetThread(Thread):

    def __init__(self, filepath):

        self.filepath = filepath
        self.rawData = pd.read_csv(filepath + ".csv", sep=",")

        self.time = []

        # liste de vecteurs numpy
        self.acceleration = []
        self.gyroscope = []
        self.magnetometer = []

        self.normAcceleration = []
        self.normGyroscope = []
        self.normMagnetometer = []

        self.velocity = [np.array([0, 0, 0])]
        self.pos = [np.array([0, 0, 0])]
        self.posAng = [np.array([0, 0, 0])]

        posAngX = 0
        posAngY = 0
        posAngZ = 0

        posX = 0
        posY = 0
        posZ = 0

        vx = 0
        vy = 0
        vz = 0

        self.time = list(self.rawData["time"])

        self.nb_line = len(self.time)

        self.compteur_line = 0
        self.thread = Thread.__init__(self)

    def run(self):
        for k in range(self.nb_line):
            self.acceleration.append(np.array([self.rawData["ax"][k], self.rawData["ay"][k], self.rawData["az"][k]]))
            self.gyroscope.append(np.array([self.rawData["gx"][k], self.rawData["gy"][k], self.rawData["gz"][k]]))
            self.magnetometer.append(np.array([self.rawData["mx"][k], self.rawData["my"][k], self.rawData["mz"][k]]))

            self.normAcceleration.append(np.linalg.norm(self.acceleration[k]))
            self.normGyroscope.append(np.linalg.norm(self.gyroscope[k]))

            #---- integration -----
            if k < self.nb_line-1:
                pas = self.time[k+1] - self.time[k]

                posAngX = self.gyroscope[k][0]*pas*0.001*180/np.pi + self.posAng[k][0]  # 360/2np.pi
                posAngY = self.gyroscope[k][1]*pas*0.001*180/np.pi + self.posAng[k][1]
                posAngZ = self.gyroscope[k][2]*pas*0.001*180/np.pi + self.posAng[k][2]
                self.posAng.append(np.array([posAngX, posAngY, posAngZ]))

                vx = self.acceleration[k][0]*pas*0.001*180/np.pi + self.velocity[k][0]  # 360/2np.pi
                vy = self.acceleration[k][1]*pas*0.001*180/np.pi + self.velocity[k][1]
                vz = self.acceleration[k][2]*pas*0.001*180/np.pi + self.velocity[k][2]
                self.posAng.append(np.array([vx, vy, vz]))

                posX = self.velocity[k][0]*pas*0.001*180/np.pi + self.posX[k][0]  # 360/2np.pi
                posY = self.velocity[k][1]*pas*0.001*180/np.pi + self.posY[k][1]
                posZ = self.velocity[k][2]*pas*0.001*180/np.pi + self.posZ[k][2]
                self.pos.append(np.array([posX, posY, posZ]))



            if k == self.nb_line-1:
                self.ConvertArray()
                self.StockIntoNewFile()
                self.plotImage()

    def ConvertArray(self):
        self.acceleration = np.array(self.acceleration)
        self.gyroscope = np.array(self.gyroscope)
        self.magnetometer = np.array(self.magnetometer)

        self.posAng = np.array(self.posAng)
        self.pos = np.array(self.pos)
        self.velocity = np.array(self.velocity)

        self.rawData["normAccel"] = self.normAcceleration
        self.rawData["normGyr"] = self.normGyroscope

    def StockIntoNewFile(self):
        self.rawData.to_csv(self.filepath + "_treated" + ".csv", sep=",", index=False, index_label=False)

    def plotImage(self):

        a = plt.subplot(331)

        a.plot(self.time, self.acceleration[:, 0], color = "r")
        a.plot(self.time, self.acceleration[:, 1], color = "green")
        a.plot(self.time, self.acceleration[:, 2], color = "blue")
        g.set_title('Acceleration')

        g = plt.subplot(332)
        g.plot(self.time, self.self.gyroscope[:, 0], color="r")
        g.plot(self.time, self.self.gyroscope[:, 1], color="green")
        g.plot(self.time, self.self.gyroscope[:, 2], color="blue")
        g.set_title('Gyroscope')
        plt.show()

    def TrapezeIntegration(self, y1, y2, t1, t2, dt=0):
        return np.trapz([y1, y2], [t1, t2])
"""