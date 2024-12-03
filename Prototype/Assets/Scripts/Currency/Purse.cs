using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Purse : MonoBehaviour
{
    public List<ICollectible> currency = new List<ICollectible>();
    public List<ICollectible> collectibles = new List<ICollectible>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AddCurrency(collision);
    }
    public void AddCurrency(Collider2D collision)
    {
        Currency collectible = collision.GetComponent<Currency>();
        if (collectible != null)
        {
            if (collectible.isCollected == true)
            {
                return;
            }
            collectible.CollectItem();
            if (collectible is Currency)
            {
                currency.Add(collectible);
            }
            else
            {
                collectibles.Add(collectible);
            }
            collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            collision.gameObject.GetComponent<CircleCollider2D>().enabled = false;

        }
    }

    public int GetFunds()
    {
        int funds = 0;
        foreach (ICollectible item in currency)
        {
            if (item is Currency)
            {
                Currency currency = (Currency)item;
                funds += currency.value;
            }
        }
        return funds;
    }
}


