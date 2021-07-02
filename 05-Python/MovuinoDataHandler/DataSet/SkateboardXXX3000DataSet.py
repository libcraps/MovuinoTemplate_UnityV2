from dataSet.MovuinoDataSet import *

class SkateboardXXX3000DataSet(MovuinoDataSet):

    def __init__(self, filepath, nbPointfilter = 50):
        MovuinoDataSet.__init__(self, filepath, nbPointfilter)
        self.name = "skateboardXXX3000"

    def run(self):
        MovuinoDataSet.run(self)

        self.rawData["ThetaGyrx"] = self.ThetaGyr[:, 0]
        self.rawData["ThetaGyry"] = self.ThetaGyr[:, 1]
        self.rawData["ThetaGyrz"] = self.ThetaGyr[:, 2]

        self.StockIntoNewFile(self.filepath)
        self.PlotImage()
        plt.show()

    def StockIntoNewFile(self, filepath):
        self.rawData.to_csv(filepath + "_treated_" + self.name + ".csv", sep=",", index=False, index_label=False)

    def PlotImage(self):
        MovuinoDataSet.PlotImage(self)

        df.PlotVector(self.time, self.acceleration_lp, 'Acceleration filtered (LP)', 334)
        df.PlotVector(self.time, self.ThetaGyr, 'Angle (integration of gyroscope) (deg)', 336)

        normAcc = plt.subplot(335)
        normAcc.plot(self.time, self.normAcceleration, color="black")
        normAcc.set_title("Norm Acceleration")

        patchX = mpatches.Patch(color='red', label='x')
        patchY = mpatches.Patch(color='green', label='y')
        patchZ = mpatches.Patch(color='blue', label='z')
        plt.legend(handles=[patchX, patchY, patchZ], loc="center right", bbox_to_anchor=(-2.5, 3.6), ncol=1)