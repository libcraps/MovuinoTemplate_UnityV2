﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Movuino
{
    /// <summary>
    /// Static class that contains usefull methods for process data of the movuino
    /// </summary>
    public static class MovuinoDataProcessing

    {
        /// <summary>
        /// Integrate with Euler methods incoming data
        /// </summary>
        /// <param name="vectorInstDerivate">Derivate data</param>
        /// <param name="vectorIntegrate">Integrate data</param>
        /// <param name="dt">delta time (sampling period)</param>
        /// <returns>Integrate a time t vector</returns>
        public static Vector3 GetEulerIntegration(Vector3 vectorInstDerivate, Vector3 vectorIntegrate, float dt)
        {
            vectorIntegrate.x += vectorInstDerivate.x * dt;
            vectorIntegrate.y += vectorInstDerivate.y * dt;
            vectorIntegrate.z += vectorInstDerivate.z * dt;
            return vectorIntegrate;
        }


        /// <summary>
        /// return the angle Vector between gravity and incoming acceleration 
        /// </summary>
        /// <param name="U">Acceleration</param>
        /// <returns></returns>
        public static Vector3 ComputeAngleAccel(Vector3 U)
        {
            Vector3 angle;

            float alpha; //z angle
            float beta; //x angle
            float gamma; //y angle

            alpha = Mathf.Acos(U.x);
            beta = Mathf.Acos(U.y);
            gamma = Mathf.Acos(U.z);

            angle = new Vector3(alpha, beta, gamma) * 360 / (2 * Mathf.PI);
            //print(angle + " ---- " + U);
            return angle;
        }

        /// <summary>
        /// return the angle Vector between Bterrestre and the movuino
        /// </summary>
        /// <param name="U">Bmov</param>
        /// <returns></returns>
        public static Vector3 ComputeAngleMagnetometer(Vector3 U)
        {
            Vector3 angle;

            float alpha; //z angle
            float beta; //x angle
            float gamma; //y angle
            /*
            alpha = Mathf.Atan(U.x / U.y);
            beta = Mathf.Atan(U.y / U.z);
            gamma = Mathf.Atan(U.x / U.z);
            */

            alpha = Mathf.Acos(U.x);
            beta = Mathf.Acos(U.y);
            gamma = Mathf.Atan(U.x / U.z);
            angle = new Vector3(beta, gamma, alpha) * 360 / (2 * Mathf.PI);
            //print(angle + " ---- " + U);
            return angle;
        }


        /// <summary>
        /// Filtered incoming data, BP filter
        /// </summary>
        /// <param name="rawDat">Incoming data</param>
        /// <param name="listMean"></param>
        /// <returns></returns>
        public static float MovingMean(float rawDat, ref List<float> listMean, int nbPointFilter)
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
        public static Vector3 MovingMean(Vector3 rawDat, ref List<Vector3> listMean, int nbPointFilter)
        {
            Vector3 meanDat = new Vector3(0, 0, 0);
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
            foreach (Vector3 vector in listMean)
            {
                meanDat += vector;
            }
            meanDat /= listMean.Count;
            return meanDat;
        }

        /// <summary>
        /// HP filter
        /// </summary>
        /// <param name="fc">Cut frequency</param>
        /// <param name="Te">Sampling period</param>
        /// <param name="sn_last">Last HP value</param>
        /// <param name="en">Current entry value</param>
        /// <param name="en_last">Previous entry value</param>
        /// <returns>HP value</returns>
        public static float HighPassFilter(float fc, float Te, float sn_last, float en, float en_last)
        {
            float tau = 1 / (2 * Mathf.PI * fc);
            float sn = sn_last * (1 - Te / tau) + en - en_last;
            return sn;
        }

        /// <summary>
        /// HP filter
        /// </summary>
        /// <param name="fc">Cut frequency</param>
        /// <param name="Te">Sampling period</param>
        /// <param name="sn_last">Last HP value</param>
        /// <param name="en">Current entry value</param>
        /// <param name="en_last">Previous entry value</param>
        /// <returns>HP value</returns>
        public static Vector3 HighPassFilter(float fc, float Te, Vector3 sn_last, Vector3 en, Vector3 en_last)
        {
            Vector3 sn;
            float gx = HighPassFilter(fc, Te, sn_last.x, en.x, en_last.x);
            float gy = HighPassFilter(fc, Te, sn_last.y, en.y, en_last.y);
            float gz = HighPassFilter(fc, Te, sn_last.z, en.z, en_last.z);
            sn = new Vector3(gx, gy, gz);

            return sn;
        }

        public static Vector3 GetEulerAngle()
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

            print("theta  : " + theta + " / " + " psi : " + psi);
        }
    }
}

