using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class PlatformState
{
    public int stateID;
    public Color color;
}
public class DestructablePlatform : MonoBehaviour
{
    [SerializeField]
    public List<PlatformState> states;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Player")
        {
            

            if(states.Count >= 1)
            {
                states.RemoveAt(0);
                GetComponent<SpriteRenderer>().color = states[0].color;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
