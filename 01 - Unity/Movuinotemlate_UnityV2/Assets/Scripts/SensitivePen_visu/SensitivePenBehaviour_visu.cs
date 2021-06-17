using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movuino;
using Movuino.Data;
using Device;
using System.IO;


public class SensitivePenBehaviour_visu : ObjectMovuino_visu
{
    [SerializeField] private bool _exportIntoFile;

    /// <summary>
    /// Path of the export data file
    /// </summary>
    [SerializeField] private string _folderPath;
    [SerializeField] private string _filename;

    [SerializeField] private GameObject vertAngle;
    [SerializeField] private GameObject horizAngle;

    private DataMovuinoSensitivePen _movuinoExportData;

    Vector3 angle;
    Vector3 initAngle;

    private float startTime;
    private int i;
    private bool end;



    //Angles we wnt with sensitiv pen
    float theta;
    float psi;



    public void Start()
    {
        _movuinoExportData = new DataMovuinoSensitivePen();


        if (offlineMode)
        {
            //movuinoDataSet.Init(dataPath);
            print("Movuino offline mode");
            InvokeRepeating("Rotate", 2f, 0.03f);
        }
        else if (onlineMode)
        {
            print("Movuino online mode");
        }
        else if (movuinoBehaviour.enabled && movuinoDataSet.enabled)
        {
            print("Impossible to use both modes, please uncheck one");
        }

        initAngle = this.gameObject.transform.localEulerAngles;

    }
    public void FixedUpdate()
    {
        if (onlineMode)
        {
            theta = movuinoBehaviour.angleAccelOrientation.x-90;
            psi = -movuinoBehaviour.angleEuler.z;

            //Angle continuity for the pen :
            if (psi < -180 && psi >= -360)
            {
                psi += 360;
            }
            else if (psi > 180 && psi <=360)
            {
                psi -= 360;
            }

            graphData.x = theta;
            graphData.y = movuinoBehaviour.magnetometerSmooth.magnitude;
            graphData.z = psi;

            //angle = new Vector3(theta, psi, 0);            
            //this.gameObject.transform.Rotate(movuinoBehaviour.gyroscopeRaw * Time.deltaTime);
                                                   
            this.gameObject.transform.eulerAngles = new Vector3(theta, psi, 0);

            vertAngle.transform.eulerAngles = new Vector3(0, (theta+90),0);
            horizAngle.transform.eulerAngles = new Vector3(0, psi, 0);

            _movuinoExportData.StockData(Time.time, movuinoBehaviour.accelerationRaw, movuinoBehaviour.gyroscopeRaw, movuinoBehaviour.magnetometerRaw, theta, psi);
        }

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

    

    private void Rotate()
    {
        Vector3 deltaTheta = movuinoDataSet.GetVector("posAngX", "posAngY", "posAngZ", movuinoDataSet.i) - movuinoDataSet.GetVector("posAngX", "posAngY", "posAngZ", movuinoDataSet.i - 1);
        this.gameObject.transform.Rotate(deltaTheta);
        print(movuinoDataSet.time);
        movuinoDataSet.i++;
    }
}