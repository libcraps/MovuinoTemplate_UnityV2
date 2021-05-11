using System.Collections;
using System.Collections.Generic;
using Movuino;
using UnityEngine;
using UnityEngine.UI;


public class WindowGraph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private int _yMax;
    [SerializeField] private int nbDot;

    [SerializeField] private MovuinoBehaviour movuinoBehaviour;

    private float _graphHeight;
    private float _xSpace;

    List<float> listeX;
    List<float> listeY;
    List<float> listeZ;

    List<float> liste;

    int i = 0;

    private void Awake()
    {
        //CreateCircle(new Vector2(200, 200));
        _xSpace = graphContainer.sizeDelta.x / nbDot;
        _graphHeight = graphContainer.sizeDelta.y;
        liste = new List<float>();

        GameObject curve = new GameObject("curve");
        curve.transform.SetParent(graphContainer, false);

    }
    private void Update()
    {
        //ShowGraph(liste, yMax, nbDot);
        float theta = (float)(i * 0.5);
        liste.Add(Mathf.Cos(theta) * 30);
        //liste.Add(movuinoBehaviour.gyroscope.x);
        i++;
        GameObject curve = GameObject.Find("curve");
        RefreshGraph(liste, curve);

    }

    private void RefreshGraph(List<float> listCurve, GameObject curveParent)
    {
        GameObject[] graph_points = GameObject.FindGameObjectsWithTag("Graph_dot");
        GameObject[] graph_lines = GameObject.FindGameObjectsWithTag("Graph_connection");

        if (graph_points.Length >0)
        {
            GameObject lastDot = null;
            int i = 0;


            foreach (var dot in graph_points)
            {
                float xpos = i* _xSpace;
                float ypos = (listCurve[i] / _yMax) * _graphHeight;
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
            while (i < listCurve.Count)
            {
                float xpos = i * _xSpace;
                float ypos = (listCurve[i] / _yMax) * _graphHeight;
                GameObject circleGameObject = CreateCircle(new Vector2(xpos, ypos), curveParent);
                if (lastDot)
                {
                    Vector2 dotPosA = lastDot.GetComponent<RectTransform>().anchoredPosition;
                    Vector2 dotPosB = circleGameObject.GetComponent<RectTransform>().anchoredPosition;
                    CreateDotConnection(dotPosA, dotPosB, curveParent);
                }

                lastDot = circleGameObject;
                //If listCurve.Count > Nbdot we remove objects
                if (i >= nbDot)
                {
                    listCurve.RemoveAt(0);
                    Destroy(graph_points[0]);
                    Destroy(graph_lines[0]);
                }
                i++;

            }
        }
        else
        {
            print("ok");
            ShowGraph(liste, _yMax, nbDot, curveParent);
        }

    }

    private void ShowGraph(List<float> listValue, float yMax, int nbDot, GameObject curve)
    {
        float _graphHeigth = graphContainer.sizeDelta.y;
        float _xSpace = graphContainer.sizeDelta.x / nbDot;

        GameObject lastGameObject = null;

        for (int i = 0; i < listValue.Count; i++)
        {
            float yPos = (listValue[i] / yMax) * _graphHeigth / 2;
            float xPos = i * _xSpace;

            GameObject dot = CreateCircle(new Vector2(xPos, yPos),curve);
            if (lastGameObject != null)
            {
                Vector2 dotPosA = lastGameObject.GetComponent<RectTransform>().anchoredPosition;
                Vector2 dotPosB = dot.GetComponent<RectTransform>().anchoredPosition;
                CreateDotConnection(dotPosA, dotPosB, curve);
            }
            lastGameObject = dot;
        }

    }

    private GameObject CreateCircle(Vector2 anchoredPosition, GameObject curveParent)
    {
        GameObject go = new GameObject("dot", typeof(Image));
        go.tag = "Graph_dot";
        RectTransform rt = go.GetComponent<RectTransform>();

        go.transform.SetParent(curveParent.transform, false);
        go.GetComponent<Image>().sprite = circleSprite;
        rt.anchoredPosition = anchoredPosition;
        rt.sizeDelta = new Vector2(11, 11);
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(0, 0.5f);

        return go;
    }

    private void CreateDotConnection(Vector2 dotPosA, Vector2 dotPosB, GameObject curveParent)
    {
        GameObject go = new GameObject("dotConnection", typeof(Image));
        go.tag = "Graph_connection";
        RectTransform rt = go.GetComponent<RectTransform>();
        go.GetComponent<Image>().color = new Color(250, 250, 250, .8f);
        go.transform.SetParent(curveParent.transform, false);

        Vector2 vectDots = dotPosB - dotPosA;
        Vector2 dir = vectDots.normalized;
        float dist = vectDots.magnitude;

        rt.sizeDelta = new Vector2(dist, 5);
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(0, 0.5f);
        rt.anchoredPosition = dotPosA + dir * dist / 2;
        go.transform.localEulerAngles = new Vector3(0,0,Vector2.SignedAngle(Vector2.right, dir));
    }

    private void RefreshDotConnection(Vector2 dotPosA, Vector2 dotPosB, GameObject line)
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


}

[System.Serializable]
public class Curve : MonoBehaviour
{
    [SerializeField] private Material material;

    GameObject[] graph_points;
    GameObject[] graph_lines;

    List<float> valueList;
}
