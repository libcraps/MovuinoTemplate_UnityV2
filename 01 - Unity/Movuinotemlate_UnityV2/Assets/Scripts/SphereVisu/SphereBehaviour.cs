﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movuino;

public class SphereBehaviour : MonoBehaviour
{
    MovuinoBehaviour movuinoBehaviour;
    Vector3 angle;

    public void Awake()
    {
        angle = new Vector3();
        movuinoBehaviour = GetComponent<MovuinoBehaviour>();
        
    }

    public void FixedUpdate()
    {
        angle = movuinoBehaviour.angleAccelOrientation;
        this.gameObject.transform.Rotate(movuinoBehaviour.gyroscope * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.G))
        {
            this.gameObject.transform.eulerAngles = GameObject.Find("OrbitCamera").transform.eulerAngles;
        }
    }


#if UNITY_EDITOR
    public void Update()
    {
        GetMouse();
    }
    public void GetMouse()
    {
        float x = Input.GetAxis("Mouse X")*3;
        float y = Input.GetAxis("Mouse Y")*3;
        this.gameObject.transform.Rotate(new Vector3(0,-y,-x));
    }


#endif
}