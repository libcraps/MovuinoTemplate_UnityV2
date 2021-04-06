using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovuinoTemplate
{
    /// <summary>
    /// Class that manage the movuino object in the scene
    /// </summary>
    /// <remarks>Handle OSC conncetion too</remarks>
    public class MovuinoBehaviour : MonoBehaviour
    {

        public OSC oscManager;
        private string addressSensorData;

        private MovuinoData movuino;
        [SerializeField]
        private string movuinoAdress;

        public OSCMovuinoSensorData OSCmovuinoSensorData { get { return movuino.OSCmovuinoSensorData; } }

        private void Awake()
        {
            movuino = new MovuinoData(movuinoAdress);
            addressSensorData = movuino.MovuinoAdress + OSCmovuinoSensorData.OSCAddress;
        }
        void Start()
        {
            oscManager.SetAddressHandler(movuino.MovuinoAdress, movuino.OSCmovuinoSensorData.ToOSCDataHandler);
            oscManager.SetAllMessageHandler(OSCDataHandler.DebugAllMessage);
        }


        private void FixedUpdate()
        {
            movuino.UpdateMovuinoData();
            //this.gameObject.transform.Rotate(movuino.InstantGyroscope * Time.fixedDeltaTime * (float)(360/(2*3.14)));
            movuino.InitMovTransform();
            this.gameObject.transform.Translate(new Vector3(movuino.AngleOrientation.y * (float)0.001, -movuino.AngleOrientation.x * (float)0.001, 0) );
            Debug.Log(movuino.AngleOrientation);
        }

        void GetGyroriantation(OSCMovuinoSensorData movuino)
        {
            this.gameObject.transform.Rotate(movuino.gyroscope * Time.fixedDeltaTime * (float)(360.0 / (2 * 3.14)));
        }
        void MoveObj(OSCMovuinoSensorData movuino)
        {
            this.gameObject.transform.Translate(movuino.accelerometer * Time.fixedDeltaTime);
        }

        void OrientObj(OSCMovuinoSensorData movuino)
        {
            //this.gameObject.transform.localEulerAngles = Angle;
        }


    }

}