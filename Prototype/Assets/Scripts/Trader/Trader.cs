using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class Trader : MonoBehaviour
{
    public List<UpgradeBase> availableUpgrades = new List<UpgradeBase>();
    public int activationCost = 5;
    public TMP_Text traderText;
    public bool isPayed = false;
    
    void Start()
    {
        
    }

    private void Update()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 3f);
        foreach (var hitCollider in hitColliders)
        {
           
            
            if (hitCollider.CompareTag("Player"))
            {
                traderText.gameObject.SetActive(true);
            
                if (Input.GetKeyDown(KeyCode.E))
                {
                    UpgradeUIController.Instance.HideUI();
                    GameManager2.Instance.JustPauseTheGame();
                  TraderUI.Instance.LoadItems(availableUpgrades.ToArray());
                  TraderUI.Instance.Show();
                }
            }
          

        }
        
    }


}
