using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class rear_update2 : MonoBehaviour
{
    public TMP_Text text;
    public GameObject rear;


    // Update is called once per frame
    void Update()
    {
        if (rear.transform.position.y >= 12.5)
        {
            text.color = Color.green;
            text.text = "Totaly Up";
        }
        else
        {
            text.color = Color.red;
            text.text = "Not Totaly Up";
        }
    }
}
