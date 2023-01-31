using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LineDistance : MonoBehaviour
{
    public Transform startPosition; // position of the line's starting point
    public Transform endPosition; // position of the line's ending point
    public GameObject line; // the line that represents the distance

    private Vector3 startVector; // the starting position's vector
    private Vector3 endVector; // the ending position's vector
    private float distance; // distance between start and end

    void Start()
    {
        // set the start and end vectors
        startVector = startPosition.position;
        endVector = endPosition.position;
    }

    void Update()
    {
        // update the distance variable
        distance = Vector3.Distance(startVector, endVector);

        // Draw the line
        line.transform.position = startVector;
        line.transform.LookAt(endVector);
        line.transform.localScale = new Vector3(1, 1, distance);
    }

    // return the distance between start and end points
    public float GetDistance()
    {
        return distance;
    }
}
