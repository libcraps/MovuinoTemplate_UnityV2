from DataManager import *

class SensitivePenDataSet(MovuinoDataSet):

    def __init__(self, filepath, nbPointfilter = 50):
        MovuinoDataSet.__init__(self, filepath, nbPointfilter)
        self.name = "SensitivePen"

        self.sensitivePenAngles = []


    def run(self):
        MovuinoDataSet.run(self)

        for k in range(len(self.time)):
            psi = -(self.eulerAngles[k, 2] - self.initEulerAngles[2])
            theta = self.posAngAcc[k, 0] - 90

            if psi < -180 and psi >= -360 :
                psi += 360
            elif psi > 180 and psi <= 360 :
                psi -= 360

            self.sensitivePenAngles.append(np.array([psi, theta]))

        self.sensitivePenAngles = np.array(self.sensitivePenAngles)
        self.rawData["psi"] = self.sensitivePenAngles[:, 0]
        self.rawData["theta"] = self.sensitivePenAngles[:, 1]

        self.StockIntoNewFile(self.filepath)
        self.PlotImage()

        plt.plot(self.time, self.sensitivePenAngles[:, 0], color="blue")
        plt.plot(self.time, self.sensitivePenAngles[:, 1], color="red")
        plt.show()

    def StockIntoNewFile(self, filepath):
        self.rawData.to_csv(filepath + "_treated_" + self.name + ".csv", sep=",", index=False, index_label=False)