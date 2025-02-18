using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class PlatformState
{
    public int stateID;
    public Sprite sprite;
}
public class DestructablePlatform : MonoBehaviour
{
    [SerializeField]
    public List<PlatformState> states;
    public SpriteRenderer spriteRenderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = states[0].sprite;
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Player")
        {
            

            if(states.Count >= 1)
            {
                spriteRenderer.sprite = states[0].sprite;
                states.RemoveAt(0);
                
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
