using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class UIForLevelingThingys : MonoBehaviour {

    [HideInInspector] public EconomyUpgrade economyUpgrade;
    public TMP_Text Name;
    public TMP_Text description;
    public TMP_Text amount;
    public TMP_Text price;
    public TMP_Text levelText;
    public int level;

    public void SetUpPanel() {
        Name.text = economyUpgrade.Name;
        description.text = economyUpgrade.description;
        amount.text = $"+{Economy.instance.TransformIntoEconomyLetter(economyUpgrade.amount)}";
        price.text = Economy.instance.TransformIntoEconomyLetter(GetCurrentPrice());
    }

    public void OnClick() {
        Economy.instance.AddUpgrade(economyUpgrade);
        level++;
        SetUpPanel();
    }

    public float GetCurrentPrice() {
        float price = economyUpgrade.price;

        for (int i = 0; i < level; i++) {
            price *= 1.15f;
        }

        return price;
    }
}
