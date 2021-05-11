﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Graph
{

    [System.Serializable]
    public class Curve : MonoBehaviour
    {
        [SerializeField] private Material material;
        [SerializeField] private Sprite sprite;
        [SerializeField] private RectTransform graphContainer;

        public int yMax;
        public int nbDot;

        public static int index;

        private float _graphHeight;
        private float _xSpace;

        public List<GameObject> graph_points;
        public List<GameObject> graph_lines;

        public List<float> valueList;
        public List<float> listMean;

        public Color curveColor;
        int i;
        private void Awake()
        {
            graph_points = new List<GameObject>();
            graph_lines = new List<GameObject>();
            valueList = new List<float>();
            listMean = new List<float>();
            curveColor = Random.ColorHSV();
        }

        public void Init(Sprite sprite, RectTransform graphContainer, int yMax, int nbDot)
        {
            index++;
            this.sprite = sprite;
            this.graphContainer = graphContainer;
            this.yMax = yMax;
            this.nbDot = nbDot;
            this.gameObject.transform.SetParent(this.graphContainer, false);

            _xSpace = graphContainer.sizeDelta.x / nbDot;
            _graphHeight = graphContainer.sizeDelta.y;
        }

        public void RefreshCurve()
        {
            if (graph_points != null)
            {
                GameObject lastDot = null;
                int i = 0;
                foreach (var dot in graph_points)
                {
                    float xpos = i * _xSpace;
                    float ypos = (valueList[i] / yMax) * _graphHeight;
                    dot.GetComponent<RectTransform>().anchoredPosition = new Vector2(xpos, ypos);

                    if (lastDot)
                    {
                        Vector2 dotPosA = lastDot.GetComponent<RectTransform>().anchoredPosition;
                        Vector2 dotPosB = dot.GetComponent<RectTransform>().anchoredPosition;
                        RefreshDotConnection(dotPosA, dotPosB, graph_lines[i - 1]);
                    }
                    lastDot = dot;
                    i++;
                }

                //i == prev listCurve.Count
                while (i < valueList.Count)
                {
                    float xpos = i * _xSpace;
                    float ypos = (valueList[i] / yMax) * _graphHeight;
                    GameObject circleGameObject = CreateDot(new Vector2(xpos, ypos));
                    if (lastDot)
                    {
                        Vector2 dotPosA = lastDot.GetComponent<RectTransform>().anchoredPosition;
                        Vector2 dotPosB = circleGameObject.GetComponent<RectTransform>().anchoredPosition;
                        CreateDotConnection(dotPosA, dotPosB);
                    }

                    lastDot = circleGameObject;
                    //If listCurve.Count > Nbdot we remove objects
                    if (i >= nbDot)
                    {
                        valueList.RemoveAt(0);
                        Destroy(graph_points[0]);
                        graph_points.RemoveAt(0);
                        Destroy(graph_lines[0]);
                        graph_lines.RemoveAt(0);
                    }
                    i++;

                }
            }
            else
            {
                print("ok");
                ShowCurve(valueList, yMax, nbDot);
            }

        }

        public GameObject CreateDot(Vector2 anchoredPosition)
        {
            GameObject go = new GameObject("dot", typeof(Image));
            graph_points.Add(go);
            go.tag = "Graph_dot";
            RectTransform rt = go.GetComponent<RectTransform>();

            go.transform.SetParent(this.gameObject.transform, false);
            go.GetComponent<Image>().sprite = sprite;
            go.GetComponent<Image>().color = curveColor;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = new Vector2(8, 8);
            rt.anchorMin = new Vector2(0, 0.5f);
            rt.anchorMax = new Vector2(0, 0.5f);

            return go;
        }

        public void CreateDotConnection(Vector2 dotPosA, Vector2 dotPosB)
        {
            GameObject go = new GameObject("dotConnection", typeof(Image));
            graph_lines.Add(go);
            go.tag = "Graph_connection";
            RectTransform rt = go.GetComponent<RectTransform>();
            go.GetComponent<Image>().color = curveColor;
            go.transform.SetParent(this.gameObject.transform, false);

            Vector2 vectDots = dotPosB - dotPosA;
            Vector2 dir = vectDots.normalized;
            float dist = vectDots.magnitude;

            rt.sizeDelta = new Vector2(dist, 8);
            rt.anchorMin = new Vector2(0, 0.5f);
            rt.anchorMax = new Vector2(0, 0.5f);
            rt.anchoredPosition = dotPosA + dir * dist / 2;
            go.transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, dir));
        }

        public void RefreshDotConnection(Vector2 dotPosA, Vector2 dotPosB, GameObject line)
        {
            RectTransform rt = line.GetComponent<RectTransform>();
            Vector2 vectDots = dotPosB - dotPosA;
            Vector2 dir = vectDots.normalized;
            float dist = vectDots.magnitude;
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.sizeDelta = new Vector2(dist, 3f);
            rt.anchoredPosition = dotPosA + dir * dist * .5f;
            rt.localRotation = Quaternion.Euler(new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, dir)));
        }

        public void ShowCurve(List<float> listValue, float yMax, int nbDot)
        {
            GameObject lastGameObject = null;

            for (int i = 0; i < listValue.Count; i++)
            {
                float yPos = (listValue[i] / yMax) * _graphHeight / 2;
                float xPos = i * _xSpace;

                GameObject dot = CreateDot(new Vector2(xPos, yPos));
                if (lastGameObject != null)
                {
                    Vector2 dotPosA = lastGameObject.GetComponent<RectTransform>().anchoredPosition;
                    Vector2 dotPosB = dot.GetComponent<RectTransform>().anchoredPosition;
                    CreateDotConnection(dotPosA, dotPosB);
                }
                lastGameObject = dot;
            }

        }





    }
}