using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;
[System.Serializable]
public class UpgradeUI
{
    public UpgradeBase upgrade;
    public TMP_Text upgradeName;
    public TMP_Text upgradeDescription;
    public TMP_Text upgradeCost;
    public GameObject upgradeUI;
    public Sprite upgradeSprite;
    public int cost;
    public bool unlocked = false;
    
}
public class TraderUI : MonoBehaviour
{

    public static TraderUI Instance;
    public TMP_Text balanceText;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public UpgradeUI[] upgradeUIs;
    public TMP_Text traderName;
    public TMP_Text traderMessage;
    public GameObject traderUI;
    public void Hide()
    {
        traderUI.SetActive(false);
        GameManager2.Instance.JustPauseTheGame();
    }
    public void SetSelectedUpgrage(int index)
    {
        UpgradeUI upgradeItemUI = upgradeUIs[index];
        upgradeItemUI.upgradeName.text = upgradeItemUI.upgrade.name;
        upgradeItemUI.upgradeDescription.text = upgradeItemUI.upgrade.description;
    }
    public void Show()
    {
        traderUI.SetActive(true);
    }
    public void LoadItems(UpgradeBase[] upgrades)
    {
        balanceText.text = FindAnyObjectByType<Purse>().GetFunds().ToString();
        upgrades = upgrades.OrderBy(upgrade => upgrade.id).ToArray();
        for(int i = 0; i < upgradeUIs.Length; i++)
        {
            upgradeUIs[i].upgrade = upgrades[i];
            upgradeUIs[i].upgradeName.text = upgrades[i].name;
            upgradeUIs[i].upgradeDescription.text = upgrades[i].description;
            upgradeUIs[i].upgradeCost.text = upgrades[i].activationCost.ToString();
            upgradeUIs[i].cost = upgrades[i].activationCost;
            if (upgradeUIs[i].unlocked)
            {
                upgradeUIs[i].upgradeUI.SetActive(false);
            }
            else
            {
                upgradeUIs[i].upgradeUI.SetActive(true);
            }
            
        }
    }
    public void UnlockItem(int index)
    {
        UpgradeUI upgradeItemUI = upgradeUIs[index];
        if (FindAnyObjectByType<Purse>().GetFunds() >= upgradeItemUI.cost)
        {
            FindAnyObjectByType<Purse>().SubCurrency(upgradeItemUI.cost);
            upgradeItemUI.upgradeUI.SetActive(false);
            upgradeItemUI.unlocked = true;

            

            UpgradeBase upgrade = GameManager2.Instance.spawnedChar.GetComponent<PlayerController>().upgrades.FirstOrDefault(upgrade => upgrade.id == upgradeUIs[index].upgrade.id);
            upgrade.unlocked = true;
        }
    }

    public void SetTraderName(string name)
    {
        traderName.text = name;
    }
    public void SetTraderMessage(string message)
    {
        traderMessage.text = message;
    }
}
