using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movuino
{

    public abstract class ObjectMovuino_visu: MonoBehaviour
    {
        private MovuinoDataSet _movuinoDataSet;
        private MovuinoBehaviour _movuinoBehaviour;

        [System.NonSerialized]
        public Vector3 graphData;

        public bool onlineMode 
        {
            get
            {
                if (movuinoBehaviour.enabled && !movuinoDataSet.enabled)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool offlineMode
        {
            get
            {
                if (movuinoDataSet.enabled && !movuinoBehaviour.enabled)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public MovuinoDataSet movuinoDataSet { get { return _movuinoDataSet; } }
        public MovuinoBehaviour movuinoBehaviour { get { return _movuinoBehaviour; } }

        public void Awake()
        {
            _movuinoDataSet = GetComponent<MovuinoDataSet>();
            _movuinoBehaviour = GetComponent<MovuinoBehaviour>();
            graphData = new Vector3(0, 0, 0);
        }

    }

}
