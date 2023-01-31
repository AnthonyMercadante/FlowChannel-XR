using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back : MonoBehaviour
{
    public GameObject actualScreen;
    public GameObject nextScreen;


    public void startUp()
    {
        actualScreen.SetActive(false);

        nextScreen.SetActive(true);
    }
}
