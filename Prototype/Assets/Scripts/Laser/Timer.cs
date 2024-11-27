using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public event System.Action timerEnded;
    public float time = 5f; // Zeit in Sekunden
    public bool isRunning = false;
    public float onTime = 5f;
    public float offTime = 5f;
    public bool alwaysOn = false;
    public bool startOnAwake = false;
    public bool isRepeating = false;
    public float offset = 1f;

    private void Awake()
    {
        if (startOnAwake)
        {
            StartCoroutine(StartTimer());
        }
    }

    public IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(offset);
        if (alwaysOn)
        {
            while (true)
            {
                timerEnded?.Invoke();
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            if (isRepeating)
            {
                while (true)
                {
                    timerEnded?.Invoke();
                    yield return new WaitForSeconds(onTime);
                    timerEnded?.Invoke();
                    yield return new WaitForSeconds(offTime);
                }
            }
            else
            {
                timerEnded?.Invoke();
                yield return new WaitForSeconds(time);
                timerEnded?.Invoke();
            }
        }
    }
}
