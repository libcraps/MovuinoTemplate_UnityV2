using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movuino
{
    public class Test_AngleRotationMatrix : ObjectMovuino_visu
    {
        [SerializeField] private GameObject x;
        [SerializeField] private GameObject y;
        [SerializeField] private GameObject z;



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //this.transform.localPosition = movuinoBehaviour.magnetometerSmooth.normalized/2;
            x.transform.localPosition = movuinoBehaviour.movuinoCoordinates.xAxis.normalized / 2;
            y.transform.localPosition = movuinoBehaviour.movuinoCoordinates.yAxis.normalized / 2;
            z.transform.localPosition = movuinoBehaviour.movuinoCoordinates.zAxis.normalized / 2;

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
                theta = Mathf.Atan2(a21, a22);
                psi = Mathf.Atan2(a10, a00);
            }
            else
            {
                theta = Mathf.Atan2(a20, sy);
                psi = 0;
            }

            print("theta  : " + theta + " / "+ " psi : " + psi);
        }
    }

}