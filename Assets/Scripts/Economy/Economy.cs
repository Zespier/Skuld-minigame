using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Economy : MonoBehaviour {

    public Stats stats;
    public float souls;
    public TMP_Text soulsPerSecond;
    public TMP_Text totalSouls;
    public List<EconomyUpgrade> upgrades = new List<EconomyUpgrade>();
    public List<char> economyLetterss = new List<char>() { ' ', 'K', 'M', 'B', 'T', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k' };

    private void Start() {
        stats.ManageAddUpgrade(Buff.BaseSoulsPerSecond, 6);
        stats.ManageAddUpgrade(Buff.SoulsPerSecondPercentage, 50);
    }

    private void Update() {
        souls += stats.SoulsPerSecond * Time.deltaTime;
        soulsPerSecond.text = $"{TransformIntoEconomyLetter(stats.SoulsPerSecond)} Soul/s";
        totalSouls.text = $"{TransformIntoEconomyLetter(souls)}";

        if (Input.GetKeyDown(KeyCode.U)) {
            stats.ManageAddUpgrade(upgrades[0].buffType, upgrades[0].amount);
        }
    }

    private string TransformIntoEconomyLetter(float amount) {
        float originalAmount = amount;
        int ceroAmount = 0;
        do {
            amount /= 10;
            ceroAmount++;
        } while (amount > 10);

        int stepsIntoTheEconomyLetters = (ceroAmount / 3);

        return (originalAmount / Mathf.Pow(10, stepsIntoTheEconomyLetters * 3)).ToString("F2") + economyLetterss[stepsIntoTheEconomyLetters];
    }
}
