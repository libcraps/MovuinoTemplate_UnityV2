void print9axesDataMPU(MPU9250 IMU){
  // display the data
  Serial.print(IMU.getAccX(),2);
  Serial.print(" ");
  Serial.print(IMU.getAccY(),2);
  Serial.print(" ");
  Serial.print(IMU.getAccZ(),2);
  Serial.print(" ");
  Serial.print(IMU.getGyroX(),2);
  Serial.print(" ");
  Serial.print(IMU.getGyroY(),2);
  Serial.print(" ");
  Serial.print(IMU.getGyroZ(),2);
  Serial.print(" ");
  Serial.print(IMU.getMagX(),2);
  Serial.print(" ");
  Serial.print(IMU.getMagY(),2);
  Serial.print(" ");
  Serial.print(IMU.getMagZ(),2);
  Serial.print(" ");
  Serial.print(IMU.getEulerX(),2);
  Serial.print(" ");
  Serial.print(IMU.getEulerY(),2);
  Serial.print(" ");
  Serial.println(IMU.getEulerZ(),2);
}

void printTempDataMPU(MPU9250 IMU){
    //Serial.println(IMU.getTemperature_C(),6);
}

void get9axesDataMPU(MPU9250 IMU, float ax, float ay, float az, float gx, float gy, float gz, float mx, float my, float mz, float y, float r, float p){
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
    //Yaw Pitch Roll
    ex = IMU.getEulerX();
    ey = IMU .getEulerY();
    ez = IMU.getEulerZ();
}


void getTempDataMPU(MPU9250 IMU){
  
}
