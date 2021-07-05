from dataSet.MovuinoDataSet import *


class SensitivePenDataSet(MovuinoDataSet):

    def __init__(self, filepath, nbPointfilter=25):
        MovuinoDataSet.__init__(self, filepath, nbPointfilter)
        self.name = "SensitivePen"

        self.sensitivePenAngles = []

        self.posAngAcc = []
        self.initEulerAngles = []
        self.eulerAngles = []

    def run(self):
        MovuinoDataSet.run(self)

        # --- Getting initial euler angles
        initRotationMatrix = gam.rotationMatrixCreation(-self.acceleration_lp[15], self.magnetometer_lp[15])
        self.initPsi = math.atan2(initRotationMatrix[0, 1], initRotationMatrix[0,0])

        for k in range(len(self.time)):
            # --- Getting rotation matrix from filtered data
            rotationMatrix = gam.rotationMatrixCreation(-self.acceleration_lp[k], self.magnetometer_lp[k])

            # --- Get inclinaison of the pen (theta)
            self.posAngAcc.append(gam.getInclinaison(self.acceleration_lp[k]))
            theta = self.posAngAcc[k][0] - 90

            # --- getting oriantation of the pen (for psi)
            a00 = rotationMatrix[0, 0]
            a01 = rotationMatrix[0, 1]

            if (abs(theta) > 80):
                psi = 0
            else:
                psi = (math.atan2(a01, a00) - self.initPsi) * 180/math.pi

                if -180 > psi >= -360:
                    psi += 360
                elif 180 < psi <= 360:
                    psi -= 360

            self.sensitivePenAngles.append(np.array([psi, theta]))

        self.posAngAcc = np.array(self.posAngAcc)
        self.sensitivePenAngles = np.array(self.sensitivePenAngles)

        self.rawData["psi"] = self.sensitivePenAngles[:, 0]
        self.rawData["theta"] = self.sensitivePenAngles[:, 1]

        self.StockIntoNewFile(self.filepath)
        self.PlotImage()
        plt.show()

    def StockIntoNewFile(self, filepath):
        self.rawData.to_csv(filepath + "_treated_" + self.name + ".csv", sep=",", index=False, index_label=False)

    def PlotImage(self):
        MovuinoDataSet.PlotImage(self)

        df.PlotVector(self.time, self.acceleration_lp, 'Acceleration filtered (LP)', 334)
        df.PlotVector(self.time, self.magnetometer_lp, 'Magnetometer filtered (LP)', 335)

        normMag = plt.subplot(337)
        normMag.plot(self.time, self.normMagnetometer, color="black")
        normMag.set_title("Norm Magnetometer")

        normAcc = plt.subplot(336)
        normAcc.plot(self.time, self.normAcceleration, color="black")
        normAcc.set_title("Norm Acceleration")

        sensitivePenAngle = plt.subplot(339)
        sensitivePenAngle.plot(self.time, self.sensitivePenAngles[:, 0], color="red", label= 'psi')
        sensitivePenAngle.plot(self.time, self.sensitivePenAngles[:, 1], color="blue", label= 'theta')
        sensitivePenAngle.grid(b=True, which='major')
        sensitivePenAngle.grid(b=True, which='minor', color='#999999', linestyle='dotted')
        sensitivePenAngle.tick_params(axis='y', which='minor', labelsize=12, color="#999999")
        sensitivePenAngle.minorticks_on()

        sensitivePenAngle.set_yticks([-180, -90, 0, 90, 180])
        sensitivePenAngle.yaxis.set_minor_locator(MultipleLocator(45))

        sensitivePenAngle.legend(loc='upper right')
        sensitivePenAngle.set_title("Relevant angle (psi, theta) (deg)")

        patchX = mpatches.Patch(color='red', label='x')
        patchY = mpatches.Patch(color='green', label='y')
        patchZ = mpatches.Patch(color='blue', label='z')
        #plt.legend(handles=[patchX, patchY, patchZ], loc="center right", bbox_to_anchor=(-2.5,3.6),ncol=1)

