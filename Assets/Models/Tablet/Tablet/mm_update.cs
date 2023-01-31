using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class mm_update : MonoBehaviour
{
    public GameObject arti;

    public TMP_Text _mm;

    private void Update()
    {
        if(arti.transform.eulerAngles.z >= 359.78f && arti.transform.eulerAngles.z <= 360.21f)
        {
            double temp = (arti.transform.eulerAngles.z-360) * 5 *10;
            if (temp < 1 && temp > -1)
            {
                _mm.text = "0 mm";
            }
            else
            {
                _mm.text = temp.ToString("#") + " mm";
            }
            
        }
        else
        {
            double temp = arti.transform.eulerAngles.z * 5 *10;
            if (temp < 1 && temp > -1)
            {
                _mm.text = "0 mm";
            }
            else
            {
                _mm.text = temp.ToString("#") + " mm";
            }
        }
        
    }
}
