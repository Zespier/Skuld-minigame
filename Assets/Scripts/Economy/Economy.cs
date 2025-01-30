using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Economy : MonoBehaviour {

    public Stats stats;
    public double souls;
    public TMP_Text soulsPerSecond;
    public TMP_Text totalSouls;
    public List<EconomyUpgrade> upgradesSoulsPerSecond = new List<EconomyUpgrade>();
    public List<EconomyUpgrade> upgradesSoulsPerCollection = new List<EconomyUpgrade>();
    public List<EconomyUpgrade> upgradesSoulsPercentage = new List<EconomyUpgrade>();
    public List<UIForLevelingThingys> UIpanelsSoulsPerSecond = new List<UIForLevelingThingys>();
    public List<UIForLevelingThingys> UIpanelsSoulsPerCollection = new List<UIForLevelingThingys>();
    public List<UIForLevelingThingys> UIpanelsSoulsPercentage = new List<UIForLevelingThingys>();
    public List<string> economyLetterss = new List<string>() { "", "K", "M", "B", "T", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az" };

    public static Economy instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void Start() {

        for (int i = 0; i < UIpanelsSoulsPerSecond.Count; i++) {
            UIpanelsSoulsPerSecond[i].economyUpgrade = upgradesSoulsPerSecond[i];
            UIpanelsSoulsPerSecond[i].SetUpPanel();
        } 
        for (int i = 0; i < UIpanelsSoulsPerCollection.Count; i++) {
            UIpanelsSoulsPerCollection[i].economyUpgrade = upgradesSoulsPerCollection[i];
            UIpanelsSoulsPerCollection[i].SetUpPanel();
        }
        for (int i = 0; i < UIpanelsSoulsPercentage.Count; i++) {
            UIpanelsSoulsPercentage[i].economyUpgrade = upgradesSoulsPercentage[i];
            UIpanelsSoulsPercentage[i].SetUpPanel();
        }
    }

    private void Update() {
        souls += stats.SoulsPerSecond * Time.deltaTime;
        soulsPerSecond.text = $"{TransformIntoEconomyLetter(stats.SoulsPerSecond)} Almas/s";
        totalSouls.text = $"{TransformIntoEconomyLetter(souls)}";

        if (Input.GetKeyDown(KeyCode.U)) {
            AddUpgrade(upgradesSoulsPerCollection[0]);
        }
    }

    public void AddSoulCollectable() {
        souls += stats.SoulsPerCollection;
    }

    public void AddUpgrade(EconomyUpgrade economyUpgrade) {
        stats.ManageAddUpgrade(economyUpgrade.buffType, economyUpgrade.amount);
    }

    public string TransformIntoEconomyLetter(double amount) {
        double originalAmount = amount;
        int ceroAmount = 0;
        do {
            amount /= 10;
            ceroAmount++;
        } while (amount > 10);

        int stepsIntoTheEconomyLetters = (ceroAmount / 3);

        return (originalAmount / Mathf.Pow(10, stepsIntoTheEconomyLetters * 3)).ToString("F2") + economyLetterss[stepsIntoTheEconomyLetters];
    }
}
