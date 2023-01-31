using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class trigger : MonoBehaviour
{
    public float deadTime = 1.0f;
    private bool _deadTimeActive = false;

    public UnityEvent onPress, onRelease;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Button" && !_deadTimeActive)
        {
            onPress?.Invoke();
            //Debug.Log("ON");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Button" && !_deadTimeActive)
        {
            onRelease?.Invoke();
            //Debug.Log("OFF");
        }
    }

    /*IEnumerator WaitForDeadTime()
    {
        _deadTimeActive = true;
        yield return new WaitForSeconds(deadTime);
        _deadTimeActive = false;
    }*/
}
