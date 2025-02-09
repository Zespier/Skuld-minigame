using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EconomyUpgrade", menuName = "EconomyUpgrade")]
public class EconomyUpgrade : ScriptableObject {

    public Buff buffType;
    public Sprite icon;
    public double amount;
    public string Name;
    public string description;
    public double price;

}
