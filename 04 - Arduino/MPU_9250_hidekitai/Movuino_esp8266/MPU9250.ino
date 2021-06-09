void print9axesDataMPU(MPU9250 IMU){
  // display the data
  Serial.print(IMU.getAccX(),6);
  Serial.print(" ");
  Serial.print(IMU.getAccY(),6);
  Serial.print(" ");
  Serial.print(IMU.getAccZ(),6);
  Serial.print(" ");
  Serial.print(IMU.getGyroX(),6);
  Serial.print(" ");
  Serial.print(IMU.getGyroY(),6);
  Serial.print(" ");
  Serial.print(IMU.getGyroZ(),6);
  Serial.print(" ");
  Serial.print(IMU.getMagX(),6);
  Serial.print(" ");
  Serial.print(IMU.getMagY(),6);
  Serial.print(" ");
  Serial.println(IMU.getMagZ(),6);
}

void printTempDataMPU(MPU9250 IMU){
    //Serial.println(IMU.getTemperature_C(),6);
}

void get9axesDataMPU(MPU9250 IMU, float ax, float ay, float az, float gx, float gy, float gz, float mx, float my, float mz){
    //Accel
    ax = IMU.getAccX();
    ay = IMU.getAccY();
    az = IMU.getAccZ();
    //Gyro
    gx = IMU.getGyroX();
    gy = IMU.getGyroY();
    gz = IMU.getGyroZ();
    //Mag
    mx = IMU.getMagX();
    my = IMU.getMagY();
    mz = IMU.getMagZ();
}


void getTempDataMPU(MPU9250 IMU){
  
}
