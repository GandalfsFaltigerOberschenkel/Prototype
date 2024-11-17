using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public void Start()
    {
        target = GameObject.Find("Player(Clone)").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            target = GameObject.Find("Player(Clone)").transform;
            return;
        }
        else
        {
            transform.position = target.position + new Vector3(0, 0, -10);
        }
    }
}
