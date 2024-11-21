using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 10f;

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
            Vector2 targetDir = target.position - transform.position;
            Vector3 moveDir = new Vector3(0,targetDir.y*followSpeed,0);
            transform.Translate(moveDir*Time.deltaTime);

        }
    }
}
