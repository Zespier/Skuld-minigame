using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Economy : MonoBehaviour {

    public Stats stats;
    public double souls;
    public TMP_Text soulsPerSecond;
    public TMP_Text totalSouls;
    public List<EconomyUpgrade> upgrades = new List<EconomyUpgrade>();
    public List<UIForLevelingThingys> UIpanels = new List<UIForLevelingThingys>();
    public List<string> economyLetterss = new List<string>() { "", "K", "M", "B", "T", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az" };

    public static Economy instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void Start() {

        for (int i = 0; i < UIpanels.Count; i++) {
            UIpanels[i].economyUpgrade = upgrades[i];
            UIpanels[i].SetUpPanel();
        }
    }

    private void Update() {
        souls += stats.SoulsPerSecond * Time.deltaTime;
        soulsPerSecond.text = $"{TransformIntoEconomyLetter(stats.SoulsPerSecond)} Soul/s";
        totalSouls.text = $"{TransformIntoEconomyLetter(souls)}";

        if (Input.GetKeyDown(KeyCode.U)) {
            AddUpgrade(upgrades[0]);
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
