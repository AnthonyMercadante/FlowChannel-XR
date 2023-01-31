using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LineRendererController : MonoBehaviour
{

    // Reference to the Line Renderer component
    private LineRenderer lineRenderer;

    // Start and end positions of the line
    private Vector3 startPos;
    private Vector3 endPos;

    void Start()
    {
        // Get reference to the Line Renderer component
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // Update the start and end positions for the line
        startPos = transform.position;
        endPos = transform.position + transform.forward * 10f;

        // Set the positions of the Line Renderer
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // Calculate the distance between the start and end points
        float distance = Vector3.Distance(startPos, endPos);
        Debug.Log(distance);

        // Calculate the angle between the start and end points
        float angle = Vector3.Angle(startPos, endPos);
    }

}
