using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class started : MonoBehaviour
{

    public GameObject firstTrigger;
    public GameObject Main;


    public void startUp()
    {
        firstTrigger.SetActive(false);

        Main.SetActive(true);
    }
}
