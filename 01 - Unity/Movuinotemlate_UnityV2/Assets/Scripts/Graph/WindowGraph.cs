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
            curveList[1].curveColor = new Color(0, 255, 0);
            curveList[2].curveColor = new Color(0, 0, 255);
        }
        private void Update()
        {
            //ShowGraph(liste, yMax, nbDot);
            float theta = (float)(i * 0.5);
            for (int k = 0; k < nbCurve; k++)
            {
                curveList[k].valueList.Add(Mathf.Cos(theta + k * Mathf.PI / 2) * 30);
                curveList[k].RefreshCurve();
            }

            i++;

            //liste.Add(movuinoBehaviour.gyroscope.x);


            //RefreshCurve(curve);

        }

    }



}