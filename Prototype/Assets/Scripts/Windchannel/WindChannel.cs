using UnityEngine;

public class WindChannel : MonoBehaviour
{
    public float upliftForce = 5.0f;
    public string windChannelName = "WindChannel";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * upliftForce);
      
        }
    }
}
