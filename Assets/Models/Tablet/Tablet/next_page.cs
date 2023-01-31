using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Threading;
using UnityEngine;

public class next_page : MonoBehaviour
{

    public float deadTime = 10.0f;
    private bool _deadTimeActive = false;

    public GameObject currentScreen;
    public GameObject nextScreen;
    private GameObject left; 
    private GameObject right;
    private GameObject touchPen; 

    public void startUp()
    {
        left = GameObject.Find("Finger Collider L");
        right = GameObject.Find("Finger Collider R");
        touchPen = GameObject.Find("Pen trigger");

        currentScreen.SetActive(false);
        nextScreen.SetActive(true);

        left.SetActive(false);
        right.SetActive(false);
        touchPen.SetActive(false);

        Invoke(nameof(reactivate),1.0f);
    }

    public void reactivate()
    {
        left.SetActive(true);
        right.SetActive(true);
        touchPen.SetActive(true);
    }

    /*IEnumerator WaitForDeadTime()
    {
        _deadTimeActive = true;
        yield return new WaitForSecondsRealtime(deadTime);
        Debug.Log("hmm");
        _deadTimeActive = false;
    }*/
}
