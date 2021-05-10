using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private int yMax;
    [SerializeField] private int nbDot;

    [SerializeField]

    List<float> liste;

    private void Awake()
    {
        //CreateCircle(new Vector2(200, 200));
        liste = new List<float>();
        for (int i = 0; i<100; i++)
        {
            float theta = (float)(i*0.5);
            liste.Add(Mathf.Cos(theta)*30);
        }
        ShowGraph(liste, yMax, nbDot);

    }
    private void Update()
    {
        //ShowGraph(liste, yMax, nbDot);
    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject go = new GameObject("dot", typeof(Image));
        go.tag = "Graph_dot";
        RectTransform rt = go.GetComponent<RectTransform>();

        go.transform.SetParent(graphContainer, false);
        go.GetComponent<Image>().sprite = circleSprite;
        rt.anchoredPosition = anchoredPosition;
        rt.sizeDelta = new Vector2(11, 11);
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(0, 0.5f);

        return go;
    }

    private void ShowGraph(List<float> listValue, float yMax, int nbDot)
    {
        float graphHeigth = graphContainer.sizeDelta.y;
        float xSpace = graphContainer.sizeDelta.x/nbDot;

        GameObject lastGameObject = null;

        for (int i = 0; i < nbDot; i++)
        {
            float yPos = (listValue[i]/yMax) * graphHeigth/2;
            float xPos = i * xSpace;

            GameObject dot = CreateCircle(new Vector2(xPos, yPos));
            if (lastGameObject != null)
            {
                Vector2 dotPosA = lastGameObject.GetComponent<RectTransform>().anchoredPosition; 
                Vector2 dotPosB = dot.GetComponent<RectTransform>().anchoredPosition;
                CreateDotConnection(dotPosA, dotPosB);
            }
            lastGameObject = dot;
        }
         
    }

    private void CreateDotConnection(Vector2 dotPosA, Vector2 dotPosB)
    {
        GameObject go = new GameObject("dotConnection", typeof(Image));
        go.tag = "Graph_connecti";
        RectTransform rt = go.GetComponent<RectTransform>();
        go.GetComponent<Image>().color = new Color(250, 250, 250, .8f);
        go.transform.SetParent(graphContainer, false);

        Vector2 vectDots = dotPosB - dotPosA;
        Vector2 dir = vectDots.normalized;
        float dist = vectDots.magnitude;

        rt.sizeDelta = new Vector2(dist, 5);
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(0, 0.5f);
        rt.anchoredPosition = dotPosA + dir * dist / 2;
        go.transform.localEulerAngles = new Vector3(0,0,Vector2.SignedAngle(Vector2.right, dir));
    }
}
