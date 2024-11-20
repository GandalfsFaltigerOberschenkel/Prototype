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
            Vector2 targetDir = target.position - transform.position;
            Vector3 moveDir = new Vector3(0,targetDir.y,-10);
            transform.Translate(moveDir*Time.deltaTime);

        }
    }
}
