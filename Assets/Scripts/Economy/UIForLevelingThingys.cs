using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIForLevelingThingys : MonoBehaviour {

    [HideInInspector] public EconomyUpgrade economyUpgrade;
    public Image icon;
    public TMP_Text Name;
    public TMP_Text description;
    public TMP_Text levelText;
    public TMP_Text amount;
    public TMP_Text price;
    public int level;

    public void SetUpPanel() {
        icon.sprite = economyUpgrade.icon;
        Name.text = economyUpgrade.Name;
        description.text = economyUpgrade.description;
        levelText.text = $"LV.{level}";
        amount.text = $"+{Economy.instance.TransformIntoEconomyLetter(economyUpgrade.amount)}";
        price.text = Economy.instance.TransformIntoEconomyLetter(GetCurrentPrice());
    }

    public void OnClick() {
        if (Economy.instance.souls >= GetCurrentPrice()) {
            Economy.instance.souls -= GetCurrentPrice();

            Economy.instance.AddUpgrade(economyUpgrade);
            level++;
            SetUpPanel();
        }
    }

    public float GetCurrentPrice() {
        float price = economyUpgrade.price;

        for (int i = 0; i < level; i++) {
            price *= 1.15f;
        }

        return price;
    }
}
