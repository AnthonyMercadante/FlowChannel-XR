using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rear_down2 : MonoBehaviour
{
    private bool on = false;
    private Vector3 valveVel = new Vector3(0f, 0f, 50f);
    public GameObject rear;
    public GameObject valve;

    public void start()
    {
        on = true;
    }

    public void stop()
    {
        on = false;
    }

    private void Update()
    {
        if (on)
        {
            if (rear.transform.position.y <= 12.6f && rear.transform.position.y >= 11.5f)
            {
                rear.transform.position += new Vector3(0, -0.5f * Time.deltaTime, 0);
                valve.transform.Rotate(valveVel * Time.deltaTime * 5);
            }
        }
    }
}
