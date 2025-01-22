using UnityEngine;

public class RopeCoinCollector : MonoBehaviour
{
    private Purse purse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        purse = GetComponent<Purse>();
        if (purse == null)
        {
            Debug.LogError("RopeCoinCollector: Missing Purse component.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CollectCoin(GameObject coin)
    {
        if (purse != null)
        {
            purse.AddCurrency(coin.GetComponent<Collider2D>());
            
            
        }
    }
}
