using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] private Transform hoursPivot, minutesPivot, secondsPivot;

    const float HoursToDegrees = -30f, MinutesToDegrees = -6f, SecondsToDegrees = -6f;

    private void Update()
    {
        TimeSpan time = DateTime.Now.TimeOfDay;
        
        hoursPivot.localRotation = Quaternion.Euler(0f, 0f, HoursToDegrees *     (float)time.TotalHours);
        minutesPivot.localRotation = Quaternion.Euler(0f, 0f, MinutesToDegrees * (float)time.TotalMinutes);
        secondsPivot.localRotation = Quaternion.Euler(0f, 0f, SecondsToDegrees * (float)time.TotalSeconds);
    }
}
