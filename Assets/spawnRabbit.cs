using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using Meta;

public class spawnRabbit : MonoBehaviour {
    Vector3 vector3 = new Vector3 (1f, 1f, 1f) ;

    [DllImport("wrapper", EntryPoint = "detectPlane")]
    public unsafe static extern int detecPlane(float* points, int size);

    //void GetTrans()
    //{
    //    GameObject temp = GameObject.Find
    //    if()
    //}

    // Update is called once per frame
    void Start () {
        Debug.Log("init pose: " + transform.position);
        Debug.Log("init ori: " + transform.rotation);
        transform.Translate(vector3);

    }
}
