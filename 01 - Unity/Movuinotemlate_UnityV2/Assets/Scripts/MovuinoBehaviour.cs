using System.Collections;
using System.Collections.Generic;
using Device;
using UnityEngine;
using Movuino.Data;
using System.IO;
using System;
using System.Data;

namespace Movuino
{
    /// <summary>
    /// Class that manage the movuino object in the scene
    /// </summary>
    /// <remarks>Handle OSC conncetion too</remarks>
    public class MovuinoBehaviour : MonoBehaviour
	{
        public OSC oscManager;

        [SerializeField] private string _movuinoAdress;
        [SerializeField] private int nbPointFilter;

        [SerializeField] private bool _exportIntoFile;

        private string _filePath;

        public List<float> listMeanX;
        public List<float> listMeanY;
        public List<float> listMeanZ;

        private string _addressSensorData;

        public OSCMovuinoSensorData OSCmovuinoSensorData; //9axes data
        public DataSessionMovuinoExtended movuinoExportData;

        public string movuinoAdress { get { return _movuinoAdress; } }

        //Instant data
        public Vector3 instantAcceleration { get { return OSCmovuinoSensorData.accelerometer; } }
        public Vector3 instantGyroscope { get { return OSCmovuinoSensorData.gyroscope; } }
        public Vector3 instantMagnetometer { get { return OSCmovuinoSensorData.magnetometer; } }

        //Data for the duration of the frame
        public Vector3 acceleration { get { return _accel; } }
        public Vector3 gyroscope { get { return (_gyr-_initGyr)*(float)(360/(2*3.14)); } }
        public Vector3 magnetometer { get { return _mag; } }

        //DeltaValues
        public Vector3 deltaAccel { get { return _accel - _prevAccel;  } }
        public Vector3 deltaGyr { get { return _gyr - _prevGyr;  } }
        public Vector3 deltaMag { get { return _mag - _prevMag;  } }

        //Angle obtained with != ways
        public Vector3 angleMagOrientation {  get { return GetAngleMag(); } }
        public Vector3 angleGyrOrientation {  get { return _angleGyrMethod; } }
        public Vector3 angleAccelOrientation {  get { return _angleAccelMethod; } }


        public float gravity;

        public Vector3 gravityReference;


        Vector3 _accel;
        Vector3 _gyr;
        Vector3 _mag;

        Vector3 _prevAccel;
        Vector3 _prevGyr;
        Vector3 _prevMag;

        Vector3 _initAngle;
        Vector3 _initGyr;
        Vector3 _initAccel;
        Vector3 _initMag;

        Vector3 _angleMagMethod;
        Vector3 _angleGyrMethod;
        Vector3 _angleAccelMethod;

        private void Awake()
        {
            Init();
            _addressSensorData = movuinoAdress + OSCmovuinoSensorData.OSCAddress;
            _filePath = ".\\_data\\";
            movuinoExportData = new DataSessionMovuinoExtended();
        }
        void Start()
        {
            oscManager.SetAddressHandler(movuinoAdress, OSCmovuinoSensorData.ToOSCDataHandler);
            //oscManager.SetAllMessageHandler(OSCDataHandler.DebugAllMessage);
        }


        private void FixedUpdate()
        {
            UpdateMovuinoData();
            InitMovTransform();
            movuinoExportData.StockData(Time.realtimeSinceStartup, acceleration, gyroscope, magnetometer, angleGyrOrientation, angleAccelOrientation);
        }

        private void OnDestroy()
        {
            if (_exportIntoFile == true) //We export the file t the end of the session if t
            {
                if (!Directory.Exists(_filePath))
                {
                    Debug.Log(_filePath + " has been created");
                    Directory.CreateDirectory(_filePath);
                }
                DataManager.ToCSV(movuinoExportData.DataTable, _filePath + "test.csv");
            }
        }

        private Vector3 ComputeAngle(Vector3 U)
        {
            Vector3 angle;

            Vector2 Uxy = new Vector2(U.x, U.y);
            Vector2 Uyz = new Vector2(U.y, U.z);
            Vector2 Uzx = new Vector2(U.z, U.x);

            float alpha; //z angle (real)
            float beta; //x angle (real)
            float gamma; //y angle (real)

            alpha = Mathf.Acos((U.x) / (Uxy.magnitude));
            beta = Mathf.Acos((U.y) / (Uyz.magnitude));
            gamma = Mathf.Acos((U.z) / (Uzx.magnitude));

            angle = new Vector3(beta, alpha, gamma)*360/(2*Mathf.PI);
            return angle;
        }
        public void Init()
		{
            _prevAccel = new Vector3(0, 0, 0);
            _prevGyr = new Vector3(0, 0, 0);
            _prevMag = new Vector3(0, 0, 0);

            _accel = new Vector3(0, 0, 0);
            _gyr = new Vector3(0, 0, 0);
            _mag = new Vector3(0, 0, 0);

            _angleGyrMethod = new Vector3(0, 0, 0);
            _angleMagMethod = new Vector3(0, 0, 0);
            _angleMagMethod = new Vector3(0, 0, 0);

            _initAngle = new Vector3(0, 0, 0);
            _initGyr = new Vector3(0, 0, 0);
            _initAccel = new Vector3(0, 0, 0);
            _initMag = new Vector3(0, 0, 0);

            listMeanX = new List<float>();
            listMeanY = new List<float>();
            listMeanZ = new List<float>();

            OSCmovuinoSensorData = OSCDataHandler.CreateOSCDataHandler<OSCMovuinoSensorData>();
        }


        Vector3 GetAngleMag()
        {
            _angleMagMethod = OSCmovuinoSensorData.magnetometer - _initAngle;
            return _angleMagMethod;
        }

        void GetAngleGyrEulerIntegratino()
        {
            _angleGyrMethod.x += gyroscope.x * Time.deltaTime;
            _angleGyrMethod.y += gyroscope.y * Time.deltaTime;
            _angleGyrMethod.z += gyroscope.z * Time.deltaTime;
        }


        public void UpdateMovuinoData()
        {
            _prevAccel = _accel;
            _prevGyr = _gyr;
            _prevMag = _mag;

            GetAngleGyrEulerIntegratino();
            _angleMagMethod = GetAngleMag();
            _angleAccelMethod = ComputeAngle(instantAcceleration.normalized);

            _accel = instantAcceleration;
            _gyr = instantGyroscope;
            _mag = instantMagnetometer;
        }

        public void InitMovTransform()
        {

            if (Input.GetKeyDown(KeyCode.I))
            {
                _initAngle = OSCmovuinoSensorData.magnetometer;
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                _initGyr = OSCmovuinoSensorData.gyroscope;
                _initMag = OSCmovuinoSensorData.magnetometer;
                _initAccel = OSCmovuinoSensorData.accelerometer;
            }

        }


        /// <summary>
        /// Filtered incoming data
        /// </summary>
        /// <param name="rawDat"></param>
        /// <param name="listMean"></param>
        /// <returns></returns>
        public float MovingMean(float rawDat, ref List<float> listMean)
        {
            float meanDat = 0;
            listMean.Add(rawDat);

            if (listMean.Count - nbPointFilter > 0)
            {
                // remove oldest data if N unchanged (i=0 removed)
                // remove from 0 to rawdat.length - N + 1 if new N < old N
                for (int i = 0; i < listMean.Count - nbPointFilter + 1; i++)
                {
                    listMean.RemoveAt(0);
                }
            }

            foreach (float number in listMean)
            {
                meanDat += number;
            }
            meanDat /= listMean.Count;
            return meanDat;
        }



    }

}