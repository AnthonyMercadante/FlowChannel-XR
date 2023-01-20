using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testeo : MonoBehaviour
{
    public float waitTime = 1f;
    private float timer1 = 1f;
    private float timer2 = 2f;
    private float timer3 = 9f;

    public Transform bottomPoint;
    public Transform bigPoint1;
    public Transform bigPoint2;
    public Transform smallPoint;
    public Transform box1;
    public Transform box2;

    public GameObject firstWater;
    public GameObject waterA;
    public GameObject waterB;
    public GameObject smallwater;
    public GameObject waterbox1;
    public GameObject waterbox2;

    private bool oneTimeRun = true;


    public void machineON()
    {
  
        

        if (oneTimeRun)
        {
            
            Debug.Log(timer1);

            if (timer1 <= 0f)
            {
                
                GameObject bottomSpawn = Instantiate(firstWater);
                bottomSpawn.transform.position = bottomPoint.position;

                timer1 = 99999f;

            }

            if (timer2 <= 0f)
            {

                GameObject spawn1 = Instantiate(waterA);
                spawn1.transform.position = bigPoint1.position;
                GameObject spawn2 = Instantiate(waterB);
                spawn2.transform.position = bigPoint2.position;

                timer2 = 99999f;
            }

            if (timer3 <= 0f)
            {
                GameObject boxspawn = Instantiate(waterbox1);
                boxspawn.transform.position = box1.position;

                GameObject spawn3 = Instantiate(smallwater);
                spawn3.transform.position = smallPoint.position;

                GameObject boxspawn2 = Instantiate(waterbox2);
                boxspawn2.transform.position = box2.position;

                timer3 = 99999f;
                oneTimeRun = false;
            }


            StartCoroutine(Wait());
            

            IEnumerator Wait()
            {
                timer1 -= 1f;
                timer2 -= 1f;
                timer3 -= 1f;
                Debug.Log("aLgo");
                yield return new WaitForSeconds(waitTime);
                machineON();
            }
        }

        
    }


}
