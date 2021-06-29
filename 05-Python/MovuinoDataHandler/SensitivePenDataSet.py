from DataManager import *
from GetAngleMethods import rotationMatrixCreation


class SensitivePenDataSet(MovuinoDataSet):

    def __init__(self, filepath, nbPointfilter = 25):
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
        self.initEulerAngles = gam.rotationMatrixToEulerAngles(initRotationMatrix)
        self.initEulerAngles = np.array(self.initEulerAngles)


        for k in range(len(self.time)):
            #Get inclinaison of pen
            self.posAngAcc.append(gam.getInclinaison(self.acceleration_lp[k]))

            #--- Getting euler angles from filtered data
            rotationMatrix = gam.rotationMatrixCreation(self.acceleration_lp[k], self.magnetometer_lp[k])

            angle = gam.rotationMatrixToEulerAngles(rotationMatrix)

            rp = gam.eulerAnglesToRotationMatrix(-self.initEulerAngles)
            id = np.dot(rotationMatrix, rp)
            xplan = [float(id[:, 0][0]) * 10, float(id[:, 0][1] * 10)]
            xplan = xplan / np.linalg.norm(xplan)
            angle2 = math.atan2(id[:, 1][1], id[:, 1][0]) * 360/(2*math.pi)
            if (math.fabs(id[:, 0][0]) < 0.000001):
                angle2 = 0

            if self.time[k]>10 and self.time[k]<30:
                print(self.time[k])
                print(np.linalg.norm(id[:,1]))
                print(id)
                print(gam.isRotationMatrix(id))
                print("angle : " + str(angle2))
                print(float(id[:, 0][0]))

                V = np.array([[float(id[:, 0][0])*10, float(id[:, 0][1]*10)], [1,-1], [0.1,0.21]])
                origin = np.array([[0, 0, 0], [0, 0, 0]])  # origin point

                plt.quiver(*origin, V[:, 0], V[:,1], color = ['r','b','g'], scale=100)
                plt.pause(0.25)
                plt.close()

            self.eulerAngles.append(angle)
            self.sensitivePenAngles.append([angle2,0])
            """
            psi = -(self.eulerAngles[k][2] - self.initEulerAngles[2])
            theta = self.posAngAcc[k][0] - 90

            if -180 > psi >= -360:
                psi += 360
            elif 180 < psi <= 360:
                psi -= 360

            self.sensitivePenAngles.append(np.array([psi, theta]))
            """

        self.posAngAcc = np.array(self.posAngAcc)
        self.eulerAngles = np.array(self.eulerAngles)
        self.sensitivePenAngles = np.array(self.sensitivePenAngles)

        #self.rawData["psi"] = self.sensitivePenAngles[:, 0]
        #self.rawData["theta"] = self.sensitivePenAngles[:, 1]

        self.StockIntoNewFile(self.filepath)
        self.PlotImage()
        plt.show()


    def StockIntoNewFile(self, filepath):
        self.rawData.to_csv(filepath + "_treated_" + self.name + ".csv", sep=",", index=False, index_label=False)

    def PlotImage(self):
        MovuinoDataSet.PlotImage(self)

        PlotVector(self.time, self.acceleration_lp, 'Acceleration filtered (LP)', 334)
        PlotVector(self.time, self.magnetometer_lp, 'Magnetometer filtered (LP)', 335)
        PlotVector(self.time, self.eulerAngles, 'Euler Angles (deg)', 338)

        normMag = plt.subplot(337)
        normMag.plot(self.time, self.normMagnetometer, color="black")
        normMag.set_title("Norm Magnetometer")

        normAcc = plt.subplot(336)
        normAcc.plot(self.time, self.normAcceleration, color="black")
        normAcc.set_title("Norm Acceleration")

        sensitivePenAngle = plt.subplot(339)
        sensitivePenAngle.plot(self.time, self.sensitivePenAngles[:, 0], color="blue")
        sensitivePenAngle.plot(self.time, self.sensitivePenAngles[:, 1], color="red")
        sensitivePenAngle.set_title("Relevant angle (psi, theta) (deg)")

        patchX = mpatches.Patch(color='red', label='x')
        patchY = mpatches.Patch(color='green', label='y')
        patchZ = mpatches.Patch(color='blue', label='z')
        plt.legend(handles=[patchX, patchY, patchZ], loc="center right", bbox_to_anchor=(-2.5,3.6),ncol=1)
