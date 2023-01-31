using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOWN : MonoBehaviour
{
    private bool on = false;
    public Vector3 velocity = new Vector3(0f, 0f, -0.01f);
    public Vector3 valveVel = new Vector3(0f, -50f, 0f);
    public GameObject arti;
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
            if (arti.transform.eulerAngles.z >= 359.8f && arti.transform.eulerAngles.z <= 360.2f)
            {
                arti.transform.Rotate(velocity * Time.deltaTime);
                valve.transform.Rotate(valveVel * Time.deltaTime);
            }
            if (arti.transform.eulerAngles.z >= -0.2f && arti.transform.eulerAngles.z <= 0.21f)
            {
                arti.transform.Rotate(velocity * Time.deltaTime);
                valve.transform.Rotate(valveVel * Time.deltaTime);
            }
        }
    }
}
