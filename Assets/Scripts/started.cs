using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class started : MonoBehaviour
{

    public GameObject firstTrigger;
    public GameObject rotationControl;

    
    public TMP_Text _mm;

    public void startUp()
    {
        firstTrigger.SetActive(false);

        rotationControl.SetActive(true);
        _mm.text="no";
    }
}
