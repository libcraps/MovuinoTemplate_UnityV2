using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movuino;


public class SensitivePenBehaviour_visu : ObjectMovuino_visu
{
    Vector3 angle;
    Vector3 initAngle;

    private float startTime;
    private int i;
    private bool end;

    public void Start()
    {
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
            angle = movuinoBehaviour.angleAccelOrientationSmooth;
            //this.gameObject.transform.Rotate(movuinoBehaviour.gyroscopeRaw * Time.deltaTime);
            this.gameObject.transform.eulerAngles = new Vector3(-angle.x, angle.z, angle.y) ;
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