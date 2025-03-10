using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public event System.Action timerEnded;
    public event System.Action timerAboutToEnd;
    public float onTime = 5f;        // Active phase duration
    public float offTime = 5f;       // Inactive phase duration
    public bool isRunning = false;
    public bool alwaysOn = false;
    public bool startOnAwake = false;
    public bool isRepeating = false;
    public float offset = 1f;        // Initial delay
    public float aboutToEndTime = 1f;

    private void Start()
    {
        if (startOnAwake) StartCoroutine(TimerCycle());
    }

    public void StartTimer()
    {
        if (!isRunning)
        {
            StartCoroutine(RunTimer());
        }
    }

    public void StopTimer()
    {
        StopAllCoroutines();
        isRunning = false;
    }
    private IEnumerator RunTimer()
    {
        timerEnded?.Invoke();
        isRunning = true;
        yield return new WaitForSeconds(onTime);
        isRunning = false;
        timerEnded?.Invoke();
    }

    private IEnumerator TimerCycle()
    {
        yield return new WaitForSeconds(offset);

        if (alwaysOn)
        {
            timerEnded?.Invoke();
            yield break;
        }

        do
        {
            // Active phase (e.g., laser ON)
            yield return new WaitForSeconds(onTime - aboutToEndTime);
            timerAboutToEnd?.Invoke(); // Warn before phase ends
            yield return new WaitForSeconds(aboutToEndTime);
            timerEnded?.Invoke();     // End active phase

            // Inactive phase (e.g., laser OFF)
            yield return new WaitForSeconds(offTime);

            // If repeating, the cycle restarts here
        }
        while (isRepeating);

        isRunning = false;
    }
}