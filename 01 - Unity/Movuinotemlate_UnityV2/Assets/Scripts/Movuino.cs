using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovuinoTemplate
{
    /// <summary>
    /// Class that manage the movuino object in the scene
    /// </summary>
    /// <remarks>Handle OSC conncetion too</remarks>
    public class Movuino : MonoBehaviour
    {
        public string movuinoAdress;
        public OSC oscManager;

        private string addressSensorData;

        public OSCMovuinoSensorData movuinoSensorData; //9axes data


        private void Awake()
        {
            movuinoSensorData = OSCDataHandler.CreateOSCDataHandler<OSCMovuinoSensorData>();
            addressSensorData = movuinoAdress + movuinoSensorData.OSCAddress;
        }
        void Start()
        {
            oscManager.SetAddressHandler(movuinoAdress, movuinoSensorData.ToOSCDataHandler);
            oscManager.SetAllMessageHandler(OSCDataHandler.DebugAllMessage);
        }

        void RotateObj(OSCMovuinoSensorData movuino)
        {
            this.gameObject.GetComponent<Rigidbody>().AddForce(movuino.gyroscope);
        }

        private void FixedUpdate()
        {
            RotateObj(movuinoSensorData);
            
        }

        private void testEulerLive(Vector3 prevGyr)
        {/*
            Y = [y0]
            for i in range(len(T) - 1):
                pas = T[i + 1] - T[i]
                yt1 = A[i] * pas * 0.001 + Y[i]
                Y.append(yt1)

            return Y
        */
            }



    }

}