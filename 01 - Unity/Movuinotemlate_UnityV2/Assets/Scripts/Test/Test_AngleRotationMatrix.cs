using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movuino
{

    public class Test_AngleRotationMatrix : ObjectMovuino_visu
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            this.transform.localPosition = movuinoBehaviour.magnetometerSmooth.normalized/2;
        }
    }

}