using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;


public class UpgradeUI : MonoBehaviour
{
    public UpgradeBase upgrade;
    bool isCooldown = false;
    public bool isUnlocked = false;
    
   
 
    public Slider upgradeProgress;
    public Image sliderBackground;
    
  
    private void Start()
    {
        if (upgrade != null)
        {
            InitializeUpgradeUI();
        }
        isUnlocked = false;
        sliderBackground.enabled = true;
    }
    private void Update()
    {
        if (upgrade.unlocked)
        {
            isUnlocked = true;
            sliderBackground.enabled = false;
        }
    }
   
    public void InitializeUpgradeUI()
    {
     
        upgradeProgress.maxValue = upgrade.duration;
        upgradeProgress.value = 0f;
      
    }

    public void OnActivateButtonClicked()
    {
        if (upgrade != null && upgrade.unlocked && !isCooldown)
        {
            isCooldown = true;
            upgrade.ActivateUpgrade();
            StartCoroutine(UpdateUpgradeProgress());
        }
    }

    private IEnumerator UpdateUpgradeProgress()
    {
        float elapsedTime = 0f;
        while (elapsedTime < upgrade.duration)
        {
            elapsedTime += Time.deltaTime;
            upgradeProgress.value = elapsedTime;
            yield return null;
        }
        upgradeProgress.value = upgrade.duration;

        // Start cooldown
        upgradeProgress.maxValue = upgrade.cooldown;
        upgradeProgress.value = upgrade.cooldown;
        elapsedTime = upgrade.cooldown;

        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            upgradeProgress.value = elapsedTime;
            yield return null;
        }
        upgradeProgress.value = 0f;
        upgradeProgress.maxValue = upgrade.duration;
        isCooldown = false;
    }
}

