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

    [SerializeField] private float offlineSampleRate;

    private DataMovuinoSensitivePen _movuinoExportData;

    Vector3 angle;
    Vector3 initAngle;

    private float startTime;
    private float prevTime;
    private int i { get { return movuinoDataSet.i; } }
    private bool end;

    //Angles we wnt with sensitiv pen
    float theta;
    float psi;

    float initPsi;

    


    public void Start()
    {
        _movuinoExportData = new DataMovuinoSensitivePen();

        if (offlineMode)
        {
            //movuinoDataSet.Init(dataPath);
            print("Movuino offline mode");
            //InvokeRepeating("AnimatePen", 1f, offlineSampleRate);
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

            initPsi = GetPenOrientation(movuinoBehaviour.initmovuinoCoordinates.rotationMatrix);

            theta = movuinoBehaviour.angleAccelOrientation.x-90;

            if (Mathf.Abs(theta) > 80)
            {
                psi = 0;
            }
            else
            {
                psi = GetPenOrientation(movuinoBehaviour.movuinoCoordinates.rotationMatrix) - initPsi;
            }

            //Angle continuity for the pen :
            MovuinoDataProcessing.AngleRange(ref psi);

            graphData.x = psi;
            graphData.y = movuinoBehaviour.magnetometerSmooth.magnitude;
            graphData.z = theta;

            //graphData = movuinoBehaviour.movuinoCoordinates.xAxis;
            print("psi : " + psi + " theta : " + theta);
            //angle = new Vector3(theta, psi, 0);            
            //this.gameObject.transform.Rotate(movuinoBehaviour.gyroscopeRaw * Time.deltaTime);

            this.gameObject.transform.eulerAngles = new Vector3(theta, psi, 0);

            vertAngle.transform.eulerAngles = new Vector3(0, (theta+90),0);
            horizAngle.transform.eulerAngles = new Vector3(0, psi, 0);

            _movuinoExportData.StockData(Time.time, movuinoBehaviour.accelerationRaw, movuinoBehaviour.gyroscopeRaw, movuinoBehaviour.magnetometerRaw, theta, psi);
        } 
        else if (offlineMode)
        {


            if (Time.time - prevTime > offlineSampleRate)
            {

                float a00 = movuinoDataSet.GetValue("a00", i);
                float a10 = movuinoDataSet.GetValue("a10", i);
                float a20 = movuinoDataSet.GetValue("a20", i);
                float a01 = movuinoDataSet.GetValue("a01", i);
                float a11 = movuinoDataSet.GetValue("a11", i);
                float a21 = movuinoDataSet.GetValue("a21", i);
                float a02 = movuinoDataSet.GetValue("a02", i);
                float a12 = movuinoDataSet.GetValue("a12", i);
                float a22 = movuinoDataSet.GetValue("a22", i);

                float theta;
                float psi;
 

                psi = Mathf.Atan2(a01, a00);
                theta = 0;

                graphData = new Vector3(0, 0, 0); //TODO

                vertAngle.transform.eulerAngles = new Vector3(0, (theta + 90), 0);
                horizAngle.transform.eulerAngles = new Vector3(0, psi, 0);


                prevTime = Time.time;
                print(prevTime);
                movuinoDataSet.i++;
            }
        }

    }

    private void OnDestroy()
    {
        if (onlineMode && _exportIntoFile) //We export the file t the end of the session if t
        {
            if (!Directory.Exists(_folderPath))
            {
                Debug.Log(_folderPath + " has been created");
                Directory.CreateDirectory(_folderPath);
            }
            DataManager.ToCSV(_movuinoExportData.DataTable, _folderPath + _filename);
        }
    }
    private float GetPenOrientation(Matrix4x4 coord)
    {
        float a00 = coord.m00;
        float a01 = coord.m01;
        print(coord);
        print(a01);

        float psi = Mathf.Atan2(a01, a00)*180/Mathf.PI;
        
        return psi;
    }


}