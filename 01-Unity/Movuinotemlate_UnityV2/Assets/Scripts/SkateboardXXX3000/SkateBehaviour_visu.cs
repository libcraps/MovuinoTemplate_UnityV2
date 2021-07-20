using System;
using System.IO;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Movuino;
using UnityEngine;

namespace Skateboard
{

    public class SkateBehaviour_visu : ObjectMovuino_visu
    {
        private float startTime;
        private int i;
        private bool end;

        private Vector3 initAngle;

        public Vector3 angleGyr { get { return movuinoDataSet.GetVector("thetaGyrx", "thetaGyry", "thetaGyrz", movuinoDataSet.i); } }

        private void Start()
        {
            //movuinoDataSet.showColumns();
            print(movuinoDataSet);
            InvokeRepeating("Rotate", 2f, ((movuinoDataSet.GetValue("time", 1)- movuinoDataSet.GetValue("time", 0))));
            initAngle = movuinoDataSet.GetVector("thetaGyrx", "thetaGyry", "thetaGyrz", 0);

        }

        private void FixedUpdate()
        {
            graphData = angleGyr;
        }

        private void Rotate()
        {
            this.gameObject.transform.localEulerAngles = new Vector3(angleGyr.y, 0, 0);
            movuinoDataSet.i++;
        }

    }

}

/*
void Start()
{
    movuinoDataSet = new MovuinoDataSet(dataPath);
    i = 0;
    startTime = Time.time;
    end = false;
    //this.gameObject.transform.Rotate(new Vector3(5, 0, 0));



}
private void FixedUpdate()
{

    //if (i < rawData.Count && Time.time - startTime >= rawData[i][0] * 0.001 && Time.time - startTime <= rawData[i + 1][0] * 0.001)
    if (i < rawData.Count - 1)
    {
        //Vector3 velocity = new Vector3(-rawData[i][13] * (float)0.5, -rawData[i][14] * (float)0.5, -rawData[i][15] * (float)0.5);
        //this.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(rawData[i][1], rawData[i][3], rawData[i][2]), ForceMode.Acceleration);
        float deltaRotX = (rawData[i + 1][10] - rawData[i][10]) * (float)(360 / 0.25);
        float deltaRotY = (rawData[i + 1][11] - rawData[i][11]) * (float)(360 / 0.25);
        float deltaRotZ = (rawData[i + 1][12] - rawData[i][12]) * (float)(360 / 0.25);
        this.gameObject.transform.Rotate(new Vector3(deltaRotX, deltaRotY, deltaRotZ));

        //this.gameObject.transform.Translate(velocity);
        //this.gameObject.transform.Rotate(new Vector3(rawData[i][4] * (float)(360 / 0.25), rawData[i][5] * (float)(360 / 0.25), rawData[i][6] * (float)(360 / 0.25)));
        i += 1;
    }
    */