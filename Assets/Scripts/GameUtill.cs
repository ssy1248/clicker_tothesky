using System;
using Unity.VisualScripting;
using UnityEngine;
using BigNumber;

public class GameUtill : MonoBehaviour
{
    private int tc_cal;

    private int day;
    private int hour;
    private int minute;
    private int second;
    
    private void CacluateNumber()
    {
        //구구단을 입력
       // for (int num1 = 1; num1 < 10; num1++)
        {
           // for (int num2 = 1; num2 < 10; num2++)
            {
               // Debug.Log(message:$"{num1} X {num2} = {num1 * num2}");
            }
        }
    }

    private string SetFriendlyTime(int totalSeconds)
    {
        tc_cal = totalSeconds;
        day = tc_cal / 86400;
        hour = (tc_cal-(day*86400)) / 3600;
        minute = (tc_cal-(day*86400+hour*3600)) / 60;
        second= tc_cal-(day*86400+hour*3600+minute*60);
        return(day.ToString()+"D"+hour.ToString()+"h"+minute.ToString()+"m"+second.ToString()+"s");
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    private void Start()
    {
        Debug.Log(message: "Start");
        Debug.Log(message: SetFriendlyTime(totalSeconds: 1234234));
    }
    
    // Update is called once per frame
 
}
