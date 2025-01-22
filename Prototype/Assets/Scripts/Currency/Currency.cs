using UnityEngine;

public class Currency : ICollectible
{
    public int value;
    public string currencyName;
    public Sprite currencySprite;
    public AudioSource collectSound;
    

    public Currency(int value, string currencyName, Sprite currencySprite)
    {
        this.value = value;
        this.currencyName = currencyName;
        this.currencySprite = currencySprite;
        isCollected = false;
    }

    public override ICollectible CollectItem()
    {
        isCollected = true;
        collectSound.Play();
        return this;
        
    }
}
