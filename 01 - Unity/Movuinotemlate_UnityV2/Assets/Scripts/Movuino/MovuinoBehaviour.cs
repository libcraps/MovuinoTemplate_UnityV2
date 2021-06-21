using System.Collections.Generic;
using Device;
using UnityEngine;
using Movuino.Data;
using System.IO;

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

        private List<Vector3> _listMeanAcc;
        private List<Vector3> _listMeanGyro;
        private List<Vector3> _listMeanMag;

        private string _addressSensorData;

        private OSCMovuinoSensorBasicData _OSCmovuinoSensorData; //9axes data
        public string movuinoAdress { get { return _movuinoAdress; } }


        #region Properties
        //OSC
        public OSCMovuinoSensorBasicData OSCmovuinoSensorData { get { return _OSCmovuinoSensorData; } }
        //Instant data
        public Vector3 instantAcceleration { get { return _OSCmovuinoSensorData.accelerometer; } }
        public Vector3 instantGyroscope { get { return _OSCmovuinoSensorData.gyroscope; } }
        public Vector3 instantMagnetometer { get { return _OSCmovuinoSensorData.magnetometer; } }

        //Data for the duration of the frame
        public Vector3 accelerationRaw { get { return _accel; } }
        public Vector3 gyroscopeRaw { get { return (_gyr)*(float)(360/(2*3.14)); } }
        public Vector3 magnetometerRaw { get { return _mag; } }

        public Vector3 accelerationSmooth { get { return MovuinoDataProcessing.MovingMean(_accel, ref _listMeanAcc, _nbPointFilter); } }
        public Vector3 gyroscopeSmooth { get { return MovuinoDataProcessing.MovingMean(_gyr, ref _listMeanGyro, _nbPointFilter) * (float)(360 / (2 * 3.14)); } }
        public Vector3 gyroscopeHighPass { get { return _HPGyr * (float)(360 / (2 * 3.14)); } }
        public Vector3 magnetometerSmooth { get { return MovuinoDataProcessing.MovingMean(_mag, ref _listMeanMag, _nbPointFilter); } }

        //DeltaValues
        public Vector3 deltaAccel { get { return _accel - _prevAccel;  } }
        public Vector3 deltaGyr { get { return _gyr - _prevGyr;  } }
        public Vector3 deltaMag { get { return _mag - _prevMag;  } }


        //Angle obtained with != ways
        public Vector3 angleGyrOrientation {  get { return _angleGyrMethod; } }
        public Vector3 angleGyrOrientationHP {  get { return _angleGyrHP; } }
        public Vector3 angleAccelOrientation {  get { return _angleAccelMethod; } }
        public Vector3 angleEuler { get { return (_euler-_initEulerAngle) * 180 / Mathf.PI;  } }
        #endregion

        public struct Coordinates
        {
            public Vector3 xAxis;
            public Vector3 yAxis;
            public Vector3 zAxis;

            public Matrix4x4 rotationMatrix { get { return new Matrix4x4(new Vector4(xAxis.x, xAxis.y, xAxis.z, 0), new Vector4(yAxis.x, yAxis.y, yAxis.z, 0), new Vector4(zAxis.x, zAxis.y, zAxis.z, 0), new Vector4(0, 0, 0, 1));  } }

            public override string ToString() {
                return "  x  " + "  y  " + "  z  " + " \n " 
                    + xAxis.x + "   " + yAxis.x + "   " + zAxis.x + " \n " 
                    + xAxis.y + "   " + yAxis.y + "   " + zAxis.y + " \n " 
                    + xAxis.z + "   " + yAxis.z + "   " + zAxis.z;
            }

            public Coordinates(int i)
            {
                xAxis = new Vector3(666, 666, 666);
                yAxis = new Vector3(666, 666, 666);
                zAxis = new Vector3(666, 666, 666);
            }
        }


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

        Vector3 _initObjectAngle;
        Vector3 _initGyr;
        Vector3 _initAccel;
        Vector3 _initMag;
        Vector3 _initEulerAngle;

        Vector3 _angleGyrHP;
        Vector3 _angleMagMethod;
        Vector3 _angleGyrMethod;
        Vector3 _angleAccelMethod;
        Vector3 _initMagAngle;

        public Coordinates movuinoCoordinates;
        public Coordinates initmovuinoCoordinates;
        
        #endregion

        #region Methods

        #region Unity implemented Methos
        private void Awake()
        {
            Init();
            _addressSensorData = movuinoAdress + _OSCmovuinoSensorData.OSCAddress;

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
            
        }

        private void OnDestroy()
        {

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

            _initObjectAngle = this.gameObject.transform.eulerAngles;
            _deltaAngleAccel = new Vector3(0, 0, 0);

            _initGyr = new Vector3(666, 666, 666);
            _initAccel = new Vector3(666, 666, 666);
            _initMag = new Vector3(666, 666, 666);
            _initEulerAngle = new Vector3(0, 0, 0);

            _angleGyrMethod = new Vector3(0, 0, 0);
            _angleAccelMethod = new Vector3(0, 0, 0);
            _angleGyrHP= new Vector3(0, 0, 0);
            _angleMagMethod = new Vector3(0, 0, 0);

            _listMeanAcc = new List<Vector3>();
            _listMeanGyro = new List<Vector3>();
            _listMeanMag = new List<Vector3>();

            initmovuinoCoordinates = new Coordinates(0);

            _OSCmovuinoSensorData = OSCDataHandler.CreateOSCDataHandler<OSCMovuinoSensorBasicData>();
        }

        public void UpdateMovuinoData()
        {
            print(initmovuinoCoordinates.xAxis);
            if (_initMag == new Vector3(666, 666, 666) && _initAccel == new Vector3(666, 666, 666) && initmovuinoCoordinates.xAxis == new Vector3(666, 666, 666) && _mag != new Vector3(0, 0, 0) && _accel != new Vector3(0, 0, 0))
            {
                _initMag = _mag;
                _initAccel = _accel;

                Vector3 c = -_initAccel.normalized;
                Vector3 b = Vector3.Cross(c, _initMag.normalized).normalized;
                Vector3 a = Vector3.Cross(c, b).normalized;
                initmovuinoCoordinates.xAxis = b;
                initmovuinoCoordinates.yAxis = c;
                initmovuinoCoordinates.zAxis = a;

                _initEulerAngle = MovuinoDataProcessing.GetEulerAngle(initmovuinoCoordinates.rotationMatrix);
                print("OOOOk");
            }
            




            _prevAccel = _accel;
            _prevGyr = _gyr;
            _prevMag = _mag;
            _prevEuler = _euler;

            _accel = instantAcceleration;
            _gyr = instantGyroscope;
            _mag = instantMagnetometer;
            

            _prevHPGyr = _HPGyr;
            _HPGyr = MovuinoDataProcessing.HighPassFilter(_fcHighPass, Time.fixedDeltaTime, _prevHPGyr, _gyr, _prevGyr);
            

            _angleGyrHP = MovuinoDataProcessing.GetEulerIntegration(gyroscopeHighPass, _angleGyrHP, Time.fixedDeltaTime);
            _angleGyrMethod = MovuinoDataProcessing.GetEulerIntegration(gyroscopeRaw, _angleGyrMethod, Time.fixedDeltaTime);
            _angleMagMethod = MovuinoDataProcessing.ComputeAngleMagnetometer(magnetometerSmooth.normalized);
            _angleAccelMethod = MovuinoDataProcessing.ComputeAngleAccel(accelerationSmooth.normalized);
            _deltaAngleAccel = _angleAccelMethod - _deltaAngleAccel;

            // --- Getting orientation matrix -----
            Vector3 z = -accelerationSmooth.normalized;
            Vector3 y = Vector3.Cross(z, magnetometerSmooth.normalized).normalized;
            Vector3 x = Vector3.Cross(z, y).normalized;

            movuinoCoordinates.xAxis = y;
            movuinoCoordinates.yAxis = z;
            movuinoCoordinates.zAxis = x;

            print(movuinoCoordinates.rotationMatrix);
            _euler = MovuinoDataProcessing.GetEulerAngle(movuinoCoordinates.rotationMatrix);
            //-------------------------------------

        }



        public void InitMovTransform()
        {

            if (Input.GetKeyDown(KeyCode.I))
            {
                _initObjectAngle = _OSCmovuinoSensorData.magnetometer;
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