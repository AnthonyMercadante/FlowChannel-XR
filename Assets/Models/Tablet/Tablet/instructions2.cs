using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class instructions2 : MonoBehaviour
{
    private int actualQ;

    private string q1 = "Measure b the width of the channel.";
    private string q2 = "Measure the distances between the stations.";
    private string q3 = "Measure the distance between the brink and the adjacent station.";
    private string q4 = "Measure the run of the channel - the distance from the pivot point of the channel to the location of the measurement of the rise of the channel.";
    private string q5 = "Set the rise (fall) at 10 mm.";
    private string q6 = "Start the pump.";
    private string q7 = "Adjust the discharge control valve so that the flow rate is the maximum possible. Check that air is not entrained at the entrance to the suction pipe.";
    private string q8 = "Measure the depth of flow at each station.";
    private string q9 = "Measure hw the head on the weir. (The point of the V-notch is level with the underside of the stilling basin).";
    private string q10 = "Force the flow to change from tranquil to rapid by lowering the sluice gate.";
    private string q11 = "Induce a hydraulic jump by raising the barrier at the brink.";
    //private string q12 = "When conditions stablize, measure du and dd the depths of flow immediately before and after the hydraulic jump.";

    public TMP_Text currentQ;

    public TMP_Text question;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Button")
        {
            string[] temp = currentQ.text.Split('/');
            //Debug.Log(temp[0]);

            switch (Int32.Parse(temp[0]))
            {
                case 12:
                    question.text = q11;
                    currentQ.text = "11/12";
                    break;
                case 11:
                    question.text = q10;
                    currentQ.text = "10/12";
                    break;
                case 10:
                    question.text = q9;
                    currentQ.text = "9/12";
                    break;
                case 9:
                    question.text = q8;
                    currentQ.text = "8/12";
                    break;
                case 8:
                    question.text = q7;
                    currentQ.text = "7/12";
                    break;
                case 7:
                    question.text = q6;
                    currentQ.text = "6/12";
                    break;
                case 6:
                    question.text = q5;
                    currentQ.text = "5/12";
                    break;
                case 5:
                    question.text = q4;
                    currentQ.text = "4/12";
                    break;
                case 4:
                    question.text = q3;
                    currentQ.text = "3/12";
                    break;
                case 3:
                    question.text = q2;
                    currentQ.text = "2/12";
                    break;
                case 2:
                    question.text = q1;
                    currentQ.text = "1/12";
                    break;
            }


        }
    }
}


