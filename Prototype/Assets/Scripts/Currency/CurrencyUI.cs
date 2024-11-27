using UnityEngine;
using TMPro;
public class CurrencyUI : MonoBehaviour
{
    public TMP_Text currencyText;
    public Purse purse;
    private void Start()
    {
        purse = FindObjectOfType<Purse>();
    }
    
    private void Update()
    {
        if(purse == null)
        {
            purse = FindObjectOfType<Purse>();
            return;
        }
        int funds = purse.GetFunds();
        currencyText.text = funds.ToString() + "$";
    }
}
