using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class rear_update : MonoBehaviour
{
    public TMP_Text text;
    public GameObject rear;


    // Update is called once per frame
    void Update()
    {
        if(rear.transform.position.y <= 15.9)
        {
            text.color = Color.green;
            text.text = "Totaly Closed";
        }
        else
        {
            text.color = Color.red;
            text.text = "Not Totaly Closed";
        }
    }
}
