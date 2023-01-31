using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomMeasuringTool : MonoBehaviour
{
    // Setting up cube objects for the inspector
    public GameObject cube1;
    public GameObject cube2;
    public LineRenderer line;
    public TextMeshPro textMesh;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // get and update the positions of the cube
        Vector3 cube1Position = cube1.transform.position;
        Vector3 cube2Position = cube2.transform.position;

        // using built-in method to measure the distance.
        float distance = Vector3.Distance(cube1Position, cube2Position);

        // draw a line between the cubes
        line = GetComponent<LineRenderer>();
        line.SetPosition(0, cube1Position);
        line.SetPosition(1, cube2Position);

        // display the distance in the TextMeshPro component
        textMesh = GetComponent<TextMeshPro>();
        textMesh.SetText(Math.Round(distance/9.8425, 2) + "m");
        Debug.Log(distance);


    }
}
