import serial
import dataSet.SensitivePenDataSet as sp
import dataSet.SkateboardXXX3000DataSet as sk
import dataSet.GlobalDataSet as gds
import dataSet.MovuinoDataSet as dm
import os
import numpy as np
import matplotlib.pyplot as plt
from scipy import signal


#folderPath = "..\\Data\\Movuino-heel_50HZ_smooth15\\"

############ SETTINGS #############

device = 'sensitivePen'  # devices available : skateboardXXX3000 / sensitivePen / globalDataSet

folderPath = "..\\_data\\mov_template\\"
fileName = "record"

serialPort = 'COM5'

toDataManage = True
toExtract = False

filter = 75

###################################

nb_files = 0
file_start = 1
nbRecord = 13
path = folderPath + fileName

# --------- Data Extraction from Movuino ----------
if toExtract:

    isReading = False
    ExtractionCompleted = False
    arduino = serial.Serial(serialPort, baudrate=115200, timeout=1.)
    line_byte = ''
    line_str = ''
    datafile = []
    nbRecord = 13

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
    #nbRecord = 1
    for i in range(file_start, file_start+nbRecord+1):
        if (device == 'sensitivePen'):
            dataSet = sp.SensitivePenDataSet(folderPath + fileName + "_" + str(i), filter)
        elif (device == 'skateboardXXX3000'):
            dataSet = sk.SkateboardXXX3000DataSet(folderPath + fileName + "_" + str(i), filter)
        elif (device == 'globalDataSet'):
            dataSet = gds.GlobalDataSet(folderPath + fileName + "_" + str(i), filter)
        else:
            print("No device matching")

        dataSet.run()
        Te = dataSet.Te
        print(1/Te)

