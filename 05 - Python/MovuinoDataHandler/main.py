import serial
import DataManager as dm
import SensitivePenDataSet as sp
import SkateboardXXX3000DataSet as sk
import DisplayFunctions as disp
import OnlyExtract as extractMovDat
import os
import numpy as np
import matplotlib.pyplot as plt
from scipy import signal


#folderPath = "..\\Data\\Movuino-heel_50HZ_smooth15\\"

############ SETTINGS #############

device = 'sensitivePen' #skateboardXXX3000 or sensitivePen

folderPath = "..\\_data\\template_data_test\\"
fileName = "record_SIMPLE_2"

serialPort = 'COM5'

toDataManage = True
toExtract = False

###################################

nb_files = 0
nbRecord = 0
path = folderPath + fileName

# --------- Data Extraction from Movuino ----------
if toExtract:

    isReading = False
    ExtractionCompleted = False
    arduino = serial.Serial(serialPort, baudrate=115200, timeout=1.)
    line_byte = ''
    line_str = ''
    datafile = []
    nbRecord = 1

    while ExtractionCompleted != True:
        line_byte = arduino.readline()
        line_str = line_byte.decode("utf-8")

        if "XXX_end" in line_str and isReading == True :
            isReading = False
            ExtractionCompleted = True
            print("End of data sheet")

            with open(path + "_" + str(nbRecord) + ".csv", "w") as file:
                file.writelines(datafile)

        if "NEW RECORD" in line_str and isReading == True :
            with open(path + "_" + str(nbRecord) + ".csv", "w") as file:
                file.writelines(datafile)

            datafile = []
            line_str = ''
            nbRecord += 1

        if (isReading):
            if line_str != '':
                datafile.append(line_str[:-1])
                print("Add Data")

        if ("XXX_beginning" in line_str):
            isReading = True


if toDataManage:
    print(nbRecord)
    nbRecord = 1
    for i in range(1, nbRecord+1):
        if (device == 'sensitivePen'):
            dataSet = sp.SensitivePenDataSet(folderPath + fileName + "_" + str(i))
        elif (device == 'skateboardXXX3000'):
            dataSet = sk.SkateboardXXX3000DataSet(folderPath + fileName + "_" + str(i))
        else:
            print("No device matching")
            dataSet = dm.MovuinoDataSet(folderPath + fileName + "_"+str(i))

        dataSet.run()
        Te = dataSet.Te
        print(1/Te)

