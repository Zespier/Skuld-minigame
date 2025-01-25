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

    private void Start() {
        stats.ManageAddUpgrade(Buff.BaseSoulsPerSecond, 6);
        stats.ManageAddUpgrade(Buff.SoulsPerSecondPercentage, 50);
    }

    private void Update() {
        souls += stats.SoulsPerSecond * Time.deltaTime;
        soulsPerSecond.text = $"{stats.SoulsPerSecond:F0} Soul/s";
        totalSouls.text = $"{souls:F0} S";

        if (Input.GetKeyDown(KeyCode.U)) {
            stats.ManageAddUpgrade(upgrades[0].buffType, upgrades[0].amount);
        }
    }
}
