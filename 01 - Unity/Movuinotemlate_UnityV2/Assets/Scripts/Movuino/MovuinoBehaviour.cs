using System.Collections;
using System.Collections.Generic;
using Device;
using UnityEngine;
using Movuino.Data;
using System.IO;
using System;
using System.Data;

/// <summary>
/// Namespace relative to movuino's scripts
/// </summary>
namespace Movuino
{
    /// <summary>
    /// Class that manage the movuino object in the scene
    /// </summary>
    /// <remarks>Handle OSC conncetion too</remarks>
    public class MovuinoBehaviour : MonoBehaviour
	{
        #region Attributs
        /// <summary>
        /// OSC connection
        /// </summary>
        public OSC oscManager;

        /// <summary>
        /// OSC adress that tbe movuino will read
        /// </summary>
        [SerializeField] private string _movuinoAdress;
        [SerializeField] private int _nbPointFilter;
        [SerializeField] private float _fcHighPass;

        [SerializeField] private bool _exportIntoFile;

        /// <summary>
        /// Path of the export data file
        /// </summary>
        [SerializeField] private string _folderPath;
        [SerializeField] private string _filename;

        private List<Vector3> _listMeanAcc;
        private List<Vector3> _listMeanGyro;
        private List<Vector3> _listMeanMag;
        private List<Vector3> _listMeanAngleAcc;


        private string _addressSensorData;

        private OSCMovuinoSensorBasicData _OSCmovuinoSensorData; //9axes data
        private DataSessionMovuinoExtended _movuinoExportData;


        public string movuinoAdress { get { return _movuinoAdress; } }

        #region Properties
        //OSC
        public OSCMovuinoSensorBasicData OSCmovuinoSensorData { get { return _OSCmovuinoSensorData; } }
        //Instant data
        public Vector3 instantAcceleration { get { return _OSCmovuinoSensorData.accelerometer; } }
        public Vector3 instantGyroscope { get { return _OSCmovuinoSensorData.gyroscope; } }
        public Vector3 instantMagnetometer { get { return _OSCmovuinoSensorData.magnetometer; } }

        public Vector3 instantEulerAngles; /*{ get { return _OSCmovuinoSensorData.eulerAngle; } }*/

        //Data for the duration of the frame
        public Vector3 accelerationRaw { get { return _accel; } }
        public Vector3 gyroscopeRaw { get { return (_gyr)*(float)(360/(2*3.14)); } }
        public Vector3 magnetometerRaw { get { return _mag; } }

        public Vector3 eulerRaw { get { return _euler; } }

        public Vector3 initMag { get { return _initMag; } }

        public Vector3 accelerationSmooth { get { return MovuinoDataProcessing.MovingMean(_accel, ref _listMeanAcc, _nbPointFilter); } }
        public Vector3 gyroscopeSmooth { get { return MovuinoDataProcessing.MovingMean(_gyr, ref _listMeanGyro, _nbPointFilter) * (float)(360 / (2 * 3.14)); } }
        public Vector3 gyroscopeHighPass { get { return _HPGyr * (float)(360 / (2 * 3.14)); } }
        public Vector3 magnetometerSmooth { get { return MovuinoDataProcessing.MovingMean(_mag, ref _listMeanMag, _nbPointFilter); } }

        //DeltaValues
        public Vector3 deltaAccel { get { return _accel - _prevAccel;  } }
        public Vector3 deltaGyr { get { return _gyr - _prevGyr;  } }
        public Vector3 deltaMag { get { return _mag - _prevMag;  } }
        public Vector3 deltaAngleAccel 
        { 
            get 
            {



                return _deltaAngleAccel;
            } 
        }

        //Angle obtained with != ways
        public Vector3 initAngleMag { get { return _initMagAngle; } }
        public Vector3 angleMagOrientation {  get { return _angleMagMethod; } }
        public Vector3 angleGyrOrientation {  get { return _angleGyrMethod; } }
        public Vector3 angleGyrOrientationHP {  get { return _angleGyrHP; } }
        public Vector3 angleAccelOrientationRaw {  get { return _angleAccelMethod; } }
        public Vector3 angleAccelOrientationSmooth {  get { return MovuinoDataProcessing.MovingMean(_angleAccelMethod, ref _listMeanAngleAcc, _nbPointFilter); } }

        public Vector3 angleEuler { get { return _euler - _initEulerAngle;  } }
        #endregion

        private float gravity;

        private Vector3 gravityReference;


        Vector3 _accel;
        Vector3 _gyr;
        Vector3 _mag;
        Vector3 _euler;

        Vector3 _prevAccel;
        Vector3 _prevGyr;
        Vector3 _prevHPGyr;
        Vector3 _prevMag;
        Vector3 _prevEuler;
        Vector3 _HPGyr;

        Vector3 _deltaAngleAccel;

        Vector3 _initAngle;
        Vector3 _initGyr;
        Vector3 _initAccel;
        Vector3 _initMag;
        Vector3 _initEulerAngle;

        Vector3 _angleGyrHP;
        Vector3 _angleMagMethod;
        Vector3 _angleGyrMethod;
        Vector3 _angleAccelMethod;
        Vector3 _angleEuler;
        Vector3 _initMagAngle;

        
        #endregion

        #region Methods

        #region Unity implemented Methos
        private void Awake()
        {
            Init();
            _addressSensorData = movuinoAdress + _OSCmovuinoSensorData.OSCAddress;
            _movuinoExportData = new DataSessionMovuinoExtended();
        }
        void Start()
        {
            oscManager.SetAddressHandler(movuinoAdress, _OSCmovuinoSensorData.ToOSCDataHandler);
            //oscManager.SetAllMessageHandler(OSCDataHandler.DebugAllMessage);
        }


        private void FixedUpdate()
        {
            UpdateMovuinoData();
            InitMovTransform();
            _movuinoExportData.StockData(Time.time, accelerationRaw, gyroscopeRaw, magnetometerRaw, angleGyrOrientation, angleAccelOrientationRaw);
        }

        private void OnDestroy()
        {
            if (_exportIntoFile == true) //We export the file t the end of the session if t
            {
                if (!Directory.Exists(_folderPath))
                {
                    Debug.Log(_folderPath + " has been created");
                    Directory.CreateDirectory(_folderPath);
                }
                DataManager.ToCSV(_movuinoExportData.DataTable, _folderPath + _filename);
            }
        }
        #endregion

        /// <summary>
        /// Initialise movuino's attributs
        /// </summary>
        public void Init()
		{
            _prevAccel = new Vector3(0, 0, 0);
            _prevGyr = new Vector3(0, 0, 0);
            _prevMag = new Vector3(0, 0, 0);
            _prevHPGyr = new Vector3(0, 0, 0);
            _prevEuler = new Vector3(0, 0, 0);
            _HPGyr = new Vector3(0, 0, 0);

            _accel = new Vector3(0, 0, 0);
            _gyr = new Vector3(0, 0, 0);
            _mag = new Vector3(0, 0, 0);
            _euler = new Vector3(0, 0, 0);

            _initAngle = this.gameObject.transform.eulerAngles;
            _deltaAngleAccel = new Vector3(0, 0, 0);

            _initGyr = new Vector3(0, 0, 0); ;
            _initAccel = new Vector3(0, 0, 0);
            _initMag = new Vector3(666, 666, 666);
            _initEulerAngle = new Vector3(666, 666, 666);

            _angleGyrMethod = _initAngle;
            _angleAccelMethod = _initAngle;
            _angleGyrHP= _initAngle;
            _angleMagMethod = new Vector3(0, 0, 0);
            _angleEuler = new Vector3(0, 0, 0);
            _initMagAngle = new Vector3(0, 0, 0);

            _listMeanAcc = new List<Vector3>();
            _listMeanGyro = new List<Vector3>();
            _listMeanMag = new List<Vector3>();
            _listMeanAngleAcc = new List<Vector3>();

            _OSCmovuinoSensorData = OSCDataHandler.CreateOSCDataHandler<OSCMovuinoSensorBasicData>();
        }


        


        public void UpdateMovuinoData()
        {
            if (_initMag == new Vector3(666, 666, 666)  && _mag != new Vector3(0, 0, 0))
            {
                _initMag = _mag;
                _initMagAngle = MovuinoDataProcessing.ComputeAngleAccel(_initMag);
                //print(_initEulerAngle);
            }

            _prevAccel = _accel;
            _prevGyr = _gyr;
            _prevMag = _mag;
            _prevEuler = _euler;

            _accel = instantAcceleration;
            _gyr = instantGyroscope;
            _mag = instantMagnetometer;
            _euler = instantEulerAngles;

            _prevHPGyr = _HPGyr;
            _HPGyr = MovuinoDataProcessing.HighPassFilter(_fcHighPass, Time.fixedDeltaTime, _prevHPGyr, _gyr, _prevGyr);
            

            _angleGyrHP = MovuinoDataProcessing.GetEulerIntegration(gyroscopeHighPass, _angleGyrHP, Time.fixedDeltaTime);
            _angleGyrMethod = MovuinoDataProcessing.GetEulerIntegration(gyroscopeRaw, _angleGyrMethod, Time.fixedDeltaTime);
            _angleMagMethod = MovuinoDataProcessing.ComputeAngleMagnetometer(magnetometerSmooth.normalized);
            _angleAccelMethod = MovuinoDataProcessing.ComputeAngleAccel(accelerationSmooth.normalized);
            _deltaAngleAccel = _angleAccelMethod - _deltaAngleAccel;

        }


        
        public void InitMovTransform()
        {

            if (Input.GetKeyDown(KeyCode.I))
            {
                _initAngle = _OSCmovuinoSensorData.magnetometer;
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                _initGyr = _OSCmovuinoSensorData.gyroscope;
                _initMag = _OSCmovuinoSensorData.magnetometer;
                _initAccel = _OSCmovuinoSensorData.accelerometer;
            }

        }


        #endregion



    }

}