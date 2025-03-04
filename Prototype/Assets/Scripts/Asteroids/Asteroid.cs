using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 1.0f;

    private void Start()
    {
        Destroy(this.gameObject, 30);

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }
}
