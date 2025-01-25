using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economy : MonoBehaviour {

    public Stats stats;
    public float souls;

    private void Awake() {
        stats.ManageAddUpgrade(Buff.BaseSoulsPerSecond, 6);
        stats.ManageAddUpgrade(Buff.SoulsPerSecondPercentage, 50);
    }

    private void Update() {
        souls += stats.SoulsPerSecond * Time.deltaTime;
    }
}
