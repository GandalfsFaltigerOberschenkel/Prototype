using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DebugCheckpoints : MonoBehaviour
{
    public List<Vector2> points;

    private void Start()
    {
        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("DebugCheckpoint");
        foreach (GameObject checkpoint in checkpoints)
        {
            points.Add(checkpoint.transform.position);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("Checkpoint 1");
            transform.position = points[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Debug.Log("Checkpoint 2");
            transform.position = points[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Debug.Log("Checkpoint 3");
            transform.position = points[2];
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Debug.Log("Checkpoint 4");
            transform.position = points[3];
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Debug.Log("Checkpoint 5");
            transform.position = points[4];
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("Checkpoint 6");
            transform.position = points[5];
        }
    }
}
