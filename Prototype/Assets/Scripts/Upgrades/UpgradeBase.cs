using System.Collections;
using UnityEngine;

public class UpgradeBase : MonoBehaviour
{
    public int id;
    public string name;
    public string description;
    public float multiplier;
    public PlayerController playerController;
    public bool unlocked = false;
    public float duration;
    public float cooldown;
    bool isActivated = false;
    bool isOnCooldown = false;
    [System.Obsolete]
    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
    }
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }
    public virtual void ApplyUpgrade()
    {
        Debug.Log("Applying upgrade: " + name);
    }
    public virtual void DeapplyUpgrade()
    {
        Debug.Log("Deapplying upgrade: " + name);
    }
    public IEnumerator DeactivateUpgrade()
    {
        yield return new WaitForSeconds(duration);
        DeapplyUpgrade();
        isOnCooldown = true;
        StartCoroutine(StartCooldown());
    }
    public IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
    public void ActivateUpgrade()
    {
        if (!isActivated && !isOnCooldown)
        {
            isActivated = true;
            ApplyUpgrade();
            StartCoroutine(DeactivateUpgrade());
        }
    }
}
