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
    float timer = 0;
    public float bufferTime = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = states[0].sprite;
        timer = bufferTime;
    }

    // Update is called once per frame
    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {

            if (timer <= 0)
            {
                timer = bufferTime;


                if (states.Count >= 1)
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
}
