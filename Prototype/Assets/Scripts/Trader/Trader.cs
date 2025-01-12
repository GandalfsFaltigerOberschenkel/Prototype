using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class Trader : MonoBehaviour
{
    public List<UpgradeBase> availableUpgrades = new List<UpgradeBase>();
    public int activationCost = 5;
    public TMP_Text dialogText;
    public bool isPayed = false;

    void Start()
    {
        
        dialogText.text = "Hello! I'm a trader. I can offer you the "+ availableUpgrades[0].name +" for: " + activationCost+ " Currcuit Boards".ToLower();
        dialogText.enabled = false;
    }
    private void Update()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 3f);
        dialogText.enabled = false;
        foreach (var hitCollider in hitColliders)
        {
           
            Debug.Log("Collider gefunden: " + hitCollider.name);
            if (hitCollider.CompareTag("Player"))
            {
                Debug.Log("Spieler gefunden");
                dialogText.enabled = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (isPayed)
                    {
                        dialogText.text = "You already bought an upgrade!".ToLower();
                    }
                    else
                    {
                        if (FindAnyObjectByType<Purse>().GetFunds() >= activationCost)
                        {
                            FindAnyObjectByType<Purse>().SubCurrency(activationCost);
                            dialogText.text = "Thank you for your purchase!".ToLower();
                            isPayed = true;
                            foreach (var upgrade in availableUpgrades)
                            {
                                upgrade.playerController = FindObjectOfType<PlayerController>();
                                List<UpgradeBase> playerUpgrades = upgrade.playerController.upgrades;
                                UpgradeBase current = playerUpgrades.FirstOrDefault(x => x.id == upgrade.id);
                                if (current != null)
                                {
                                    current.unlocked = true;
                                }
                                else
                                {
                                    //Not found
                                    Debug.Log("Upgrade not found");
                                }
                            }
                        }
                        else
                        {
                            dialogText.text = "You don't have enough Currcuit Boards!".ToLower();
                        }
                    }
                }
            }
          

        }
        
    }


}
