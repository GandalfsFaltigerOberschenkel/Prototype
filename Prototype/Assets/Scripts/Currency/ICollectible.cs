using UnityEngine;

public class ICollectible : MonoBehaviour
{
    protected string name = "Collectible";
    public bool isCollected = false;
    public virtual ICollectible CollectItem()
    {
        isCollected = true;
        return this;
    }
}
