using System.Collections;
using System.Collections.Generic;
using Movuino;
using UnityEngine;
using UnityEngine.UI;


namespace Graph
{
    public class WindowGraph : MonoBehaviour
    {
        [SerializeField] private Sprite circleSprite;
        [SerializeField] private RectTransform graphContainer;
        [SerializeField] private Curve curvePrefab;
        [SerializeField] private int _yMax;
        [SerializeField] private int nbDot;

        [SerializeField] private MovuinoBehaviour movuinoBehaviour;


        int nbCurve;
        List<Curve> curveList;

        List<float> liste;

        int i = 0;

        private GameObject rawDataText;
        private Text angleX;
        private Text angleY;
        private Text angleZ;



        private void Awake()
        {
            //CreateCircle(new Vector2(200, 200));
            liste = new List<float>();
            curveList = new List<Curve>();
            nbCurve = 3;

            for (int k = 0; k < nbCurve; k++)
            {
                GameObject go = Instantiate(curvePrefab.gameObject, graphContainer);
                go.GetComponent<Curve>().Init(circleSprite, graphContainer, _yMax, nbDot);
                go.name = go.name + "_" + Curve.index;
                curveList.Add(go.GetComponent<Curve>());
            }

            curveList[0].curveColor = new Color(255, 0, 0);
            curveList[1].curveColor = new Color(0, 200, 0);
            curveList[2].curveColor = new Color(0, 0, 255);

            rawDataText = this.gameObject.transform.Find("RawDataTexte").gameObject;
            angleX = rawDataText.transform.Find("AngleX").GetComponent<Text>();
            angleY = rawDataText.transform.Find("AngleY").GetComponent<Text>();
            angleZ = rawDataText.transform.Find("AngleZ").GetComponent<Text>();
        }
        private void Update()
        {
            float theta = (float)(i * 0.5);
            liste.Add(Mathf.Cos(theta) * 30);
            curveList[0].valueList.Add(movuinoBehaviour.MovingMean(movuinoBehaviour.angleAccelOrientation.x, ref movuinoBehaviour.listMeanX));
            curveList[1].valueList.Add(movuinoBehaviour.MovingMean(movuinoBehaviour.angleAccelOrientation.y, ref movuinoBehaviour.listMeanY));
            curveList[2].valueList.Add(movuinoBehaviour.MovingMean(movuinoBehaviour.angleAccelOrientation.z, ref movuinoBehaviour.listMeanZ));

            angleX.text = "Angle X : " + (int)movuinoBehaviour.angleAccelOrientation.x;
            angleY.text = "Angle Y : " + (int)movuinoBehaviour.angleAccelOrientation.y;
            angleZ.text = "Angle Z : " + (int)movuinoBehaviour.angleAccelOrientation.z;
            for (int k =0; k<nbCurve; k++)
            {
                curveList[k].RefreshCurve();
            }
            i++;

            //liste.Add(movuinoBehaviour.gyroscope.x);
            //RefreshCurve(curve);

        }

    }



}