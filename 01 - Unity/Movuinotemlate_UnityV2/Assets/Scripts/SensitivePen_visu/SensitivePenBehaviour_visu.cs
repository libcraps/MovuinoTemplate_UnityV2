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
            theta = 90-movuinoBehaviour.angleAccelOrientation.x;
            psi = -movuinoBehaviour.angleEuler.z;

            angle = GetEulerAngle()*180/Mathf.PI;

            graphData.x = angle.x;
            graphData.y = angle.y;
            graphData.z = angle.z;

            //angle = new Vector3(theta, psi, 0);            //this.gameObject.transform.Rotate(movuinoBehaviour.gyroscopeRaw * Time.deltaTime);
                                                   //this.gameObject.transform.eulerAngles = new Vector3(-angle.x, angle.z, angle.y);

            vertAngle.transform.eulerAngles = new Vector3(0, theta,0);
            horizAngle.transform.eulerAngles = new Vector3(0, angle.z, 0);

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

    public Vector3 GetEulerAngle()
    {
        float a00;
        float a10;
        float a20;
        float a01;
        float a02;
        float a11;
        float a22;
        float a12;
        float a21;

        float theta;
        float psi;
        float phi;


        a00 = movuinoBehaviour.movuinoCoordinates.xAxis.x;
        a10 = movuinoBehaviour.movuinoCoordinates.xAxis.y;
        a20 = movuinoBehaviour.movuinoCoordinates.xAxis.z;
        a01 = movuinoBehaviour.movuinoCoordinates.yAxis.x;
        a11 = movuinoBehaviour.movuinoCoordinates.yAxis.y;
        a21 = movuinoBehaviour.movuinoCoordinates.yAxis.z;
        a02 = movuinoBehaviour.movuinoCoordinates.zAxis.x;
        a12 = movuinoBehaviour.movuinoCoordinates.zAxis.y;
        a22 = movuinoBehaviour.movuinoCoordinates.zAxis.z;

        float sy = Mathf.Sqrt(a00 * a00 + a10 * a10);
        bool singuler = sy < 0.00001;

        if (!singuler)
        {
            phi = Mathf.Atan2(a21, a22);
            theta = Mathf.Atan2(-a20, sy);
            psi = Mathf.Atan2(a10, a00);
        }
        else
        {
            
            phi = Mathf.Atan2(a21, a22);
            theta = Mathf.Atan2(a20, sy);
            psi = 0;
        }

        print("theta  : " + theta + " / " + " psi : " + psi);

        return new Vector3(phi, theta, psi);
    }

    private void Rotate()
    {
        Vector3 deltaTheta = movuinoDataSet.GetVector("posAngX", "posAngY", "posAngZ", movuinoDataSet.i) - movuinoDataSet.GetVector("posAngX", "posAngY", "posAngZ", movuinoDataSet.i - 1);
        this.gameObject.transform.Rotate(deltaTheta);
        print(movuinoDataSet.time);
        movuinoDataSet.i++;
    }
}