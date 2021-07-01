import threading
from threading import Thread
import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
import pandas as pd
import numpy as np
import tools.FilterMethods as fm
import tools.integratinoFunctions as ef
import tools.GetAngleMethods as gam
import tools.DisplayFunctions as df
from scipy import signal
from scipy.spatial.transform import Rotation as R
import math
import sys
import time


class MovuinoDataSet():
    """

    """
    def __init__(self, filepath, nbPointfilter = 15):
        """

        :param filepath:
        :param nbPointfilter:
        """
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

        self.time = list(self.rawData["time"]*0.001)
        self.rawData["time"] = self.time

        self.Te = (self.time[-1]-self.time[0])/(len(self.time))

        self.nb_line = len(self.time)

        self.compteur_line = 0

    def run(self):
        """

        :return:
        """
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

        #--------------------------- FILTER ----------------------------------
        for k in range(self.nb_line):
            self.acceleration_lp.append(fm.MeandDat(self.acceleration[k], self.nbPointFilter, self.listMeanAcc))
            self.gyroscope_lp.append(fm.MeandDat(self.gyroscope[k], self.nbPointFilter, self.listMeanGyr))
            self.magnetometer_lp.append(fm.MeandDat(self.magnetometer[k], self.nbPointFilter, self.listMeanMag))

        self.acceleration_lp = np.array(self.acceleration_lp)
        self.gyroscope_lp = np.array(self.gyroscope_lp)
        self.magnetometer_lp = np.array(self.magnetometer_lp)



    def StockIntoNewFile(self, filepath):
        """

        :param filepath:
        :return:
        """
        self.rawData.to_csv(filepath + "_treated" + ".csv", sep=",", index=False, index_label=False)

    def PlotImage(self):
        """

        :return:
        """
        df.PlotVector(self.time, self.acceleration, 'Acceleration (m/s2)', 331)
        df.PlotVector(self.time, self.magnetometer, 'Magnetometer', 332)
        df.PlotVector(self.time, self.gyroscope, 'Gyroscope (deg/s)', 333)

    def AddingRawData(self):
        """

        :return:
        """
        self.rawData["normAccel"] = self.normAcceleration
        self.rawData["normMag"] = self.normMagnetometer
        self.rawData["normGyr"] = self.normGyroscope





def ConvertArray(*args):
    for arg in args:
        arg = np.array(arg)