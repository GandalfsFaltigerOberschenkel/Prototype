using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UpgradeUIController : MonoBehaviour
{

    public static UpgradeUIController Instance;
    public UpgradeUI[] upgradeUIs;
    public List<UpgradeBase> playerUpgrades;
    public GameObject content;
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
    private void Start()
    {
        Initialize();
    }
    void Initialize()
    {
        PlayerController p = FindObjectOfType<PlayerController>();
        GetPlayerUpgradeRefrence(p);
        
    }
    public void HideUI()
    {
        content.SetActive(false);
    }
    public void ShowUI()
    {
        content.SetActive(true);
    }
    void GetPlayerUpgradeRefrence(PlayerController playerController)
    {
        playerUpgrades = playerController.upgrades;
        for (int i = 0; i < upgradeUIs.Length; i++)
        {
            upgradeUIs[i].upgrade = playerUpgrades[i];
            upgradeUIs[i].InitializeUpgradeUI();
        }
    }
  
}

