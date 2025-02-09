using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stats : MonoBehaviour {

    public UpgradeHolder upgradeHolder;

    public List<double> baseSoulsPerSeconds;
    public List<double> soulsPerSecondPercentages;
    public List<double> flatSoulsPerSeconds;

    public List<double> baseSoulsPerCollections;
    public List<double> soulsPerCollectionPercentages;
    public List<double> flatSoulsPerCollections;

    //public List<float> baseSpeeds;
    //public List<float> speedPercentages;
    //public List<float> flatSpeeds;

    public double baseSoulsPerSecond = 10;
    public virtual double SoulsPerSecond => (baseSoulsPerSecond + BaseSoulsPerSeconds) * (SoulsPerSecondPercentages / 100f) + FlatSoulsPerSeconds;
    public virtual double BaseSoulsPerSeconds => GetAllBuffs(Buff.BaseSoulsPerSecond);
    public virtual double SoulsPerSecondPercentages => GetAllBuffs(Buff.SoulsPerSecondPercentage);
    public virtual double FlatSoulsPerSeconds => GetAllBuffs(Buff.FlatSoulsPerSecond);

    public double baseSoulsPerCollection = 100;
    public virtual double SoulsPerCollection => (baseSoulsPerCollection + BaseSoulsPerCollections) * (SoulsPerCollectionPercentages / 100f) + FlatSoulsPerCollections;
    public virtual double BaseSoulsPerCollections => GetAllBuffs(Buff.BaseSoulsPerCollection);
    public virtual double SoulsPerCollectionPercentages => GetAllBuffs(Buff.SoulsPerCollectionPercentage);
    public virtual double FlatSoulsPerCollections => GetAllBuffs(Buff.FlatSoulsPerCollection);

    //No defense, too hard to manage

    //public double baseSpeed = 2;
    //public virtual double Speed => (baseSpeed + BaseSpeeds) * (SpeedPercentages / 100f) + FlatSpeeds;
    //public virtual double BaseSpeeds => GetAllBuffs(Buff.BaseSpeed);
    //public virtual double SpeedPercentages => GetAllBuffs(Buff.SpeedPercentage);
    //public virtual double FlatSpeeds => GetAllBuffs(Buff.FlatSpeed);

    //private List<(Buff buff, double amount, Coroutine coroutine)> buffCoroutines = new List<(Buff buff, double amount, Coroutine coroutine)>();

    #region Upgrade Management

    public virtual void ManageAddUpgrade(Buff buffType, double amount) {
        if (upgradeHolder == null) {
            Debug.LogError("No upgrade holder assigned");
            return;
        }

        upgradeHolder.AddUpgrade(buffType, amount, this);
    }
    public virtual void ManageAddUpgrade(Buff buff, double amount, float time) {
        ManageAddUpgrade(buff, amount);
        //buffCoroutines.Add((buff, amount, StartCoroutine(C_WaitToRemoveUpgrade(buff, amount, time))));
    }

    public virtual void ManageRemoveUpgrade(Buff buffType, double amount) {
        if (upgradeHolder == null) {
            Debug.LogError("No upgrade holder assigned");
            return;
        }

        upgradeHolder.RemoveUpgrade(buffType, amount);
    }

    #endregion

    #region Buffs

    public virtual double GetAllBuffs(Buff buffType) {

        switch (buffType) {

            case Buff.BaseSoulsPerSecond:
                if (baseSoulsPerSeconds == null || baseSoulsPerSeconds.Count <= 0) {
                    return 0;
                }
                return baseSoulsPerSeconds.Sum();

            case Buff.SoulsPerSecondPercentage:
                if (soulsPerSecondPercentages == null || soulsPerSecondPercentages.Count <= 0) {
                    return 100f; //Is in percentage
                }
                return soulsPerSecondPercentages.Sum() + 100f;

            case Buff.FlatSoulsPerSecond:
                if (flatSoulsPerSeconds == null || flatSoulsPerSeconds.Count <= 0) {
                    return 0;
                }
                return flatSoulsPerSeconds.Sum();

            case Buff.BaseSoulsPerCollection:
                if (baseSoulsPerCollections == null || baseSoulsPerCollections.Count <= 0) {
                    return 0;
                }
                return baseSoulsPerCollections.Sum();

            case Buff.SoulsPerCollectionPercentage:
                if (soulsPerCollectionPercentages == null || soulsPerCollectionPercentages.Count <= 0) {
                    return 100;
                }
                return soulsPerCollectionPercentages.Sum() + 100f;

            case Buff.FlatSoulsPerCollection:
                if (flatSoulsPerCollections == null || flatSoulsPerCollections.Count <= 0) {
                    return 0;
                }
                return flatSoulsPerCollections.Sum();

            //case Buff.BaseSpeed:
            //    if (baseSpeeds == null || baseSpeeds.Count <= 0) {
            //        return 0;
            //    }
            //    return baseSpeeds.Sum();

            //case Buff.SpeedPercentage:
            //    if (speedPercentages == null || speedPercentages.Count <= 0) {
            //        return 100;
            //    }
            //    return speedPercentages.Sum() + 100f;

            //case Buff.FlatSpeed:
            //    if (flatSpeeds == null || flatSpeeds.Count <= 0) {
            //        return 0;
            //    }
            //    return flatSpeeds.Sum();

            default:
                Debug.LogError("Buff not defined");
                return 0;

        }

    }

    public virtual void AddBuff(Buff buffType, double amount) {

        switch (buffType) {

            case Buff.BaseSoulsPerSecond:
                baseSoulsPerSeconds.Add(amount);
                break;

            case Buff.SoulsPerSecondPercentage:
                soulsPerSecondPercentages.Add(amount);
                break;

            case Buff.FlatSoulsPerSecond:
                flatSoulsPerSeconds.Add(amount);
                break;

            case Buff.BaseSoulsPerCollection:
                baseSoulsPerCollections.Add(amount);
                break;

            case Buff.SoulsPerCollectionPercentage:
                soulsPerCollectionPercentages.Add(amount);
                break;

            case Buff.FlatSoulsPerCollection:
                flatSoulsPerCollections.Add(amount);
                break;

            //case Buff.BaseSpeed:
            //    baseSpeeds.Add(amount);
            //    break;

            //case Buff.SpeedPercentage:
            //    speedPercentages.Add(amount);
            //    break;

            //case Buff.FlatSpeed:
            //    flatSpeeds.Add(amount);
            //    break;

            default:
                Debug.LogError("Buff not defined");
                break;
        }
    }

    public virtual void RemoveBuff(Buff buffType, double amount) {

        switch (buffType) {

            case Buff.BaseSoulsPerSecond:
                baseSoulsPerSeconds.Remove(amount);
                break;

            case Buff.SoulsPerSecondPercentage:
                soulsPerSecondPercentages.Remove(amount);
                break;

            case Buff.FlatSoulsPerSecond:
                flatSoulsPerSeconds.Remove(amount);
                break;

            case Buff.BaseSoulsPerCollection:
                baseSoulsPerCollections.Remove(amount);
                break;

            case Buff.SoulsPerCollectionPercentage:
                soulsPerCollectionPercentages.Remove(amount);
                break;

            case Buff.FlatSoulsPerCollection:
                flatSoulsPerCollections.Remove(amount);
                break;

            //case Buff.BaseSpeed:
            //    baseSpeeds.Remove(amount);
            //    break;

            //case Buff.SpeedPercentage:
            //    speedPercentages.Remove(amount);
            //    break;

            //case Buff.FlatSpeed:
            //    flatSpeeds.Remove(amount);
            //    break;

            default:
                Debug.LogError("Buff not defined");
                break;
        }
    }

    #endregion

    #region Timers

    private IEnumerator C_WaitToRemoveUpgrade(Buff buff, float amount, float time) {

        float timer = time;

        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }

        ManageRemoveUpgrade(buff, amount);
    }

    public void RemoveAllBuffs() {
        for (int i = 0; i < upgradeHolder.upgrades.Count; i++) {
            upgradeHolder.RemoveUpgrade(upgradeHolder.upgrades[i].upgradeType, upgradeHolder.upgrades[i].amount);
        }
    }

    #endregion
}

public enum Buff {
    BaseSoulsPerSecond,
    SoulsPerSecondPercentage,
    FlatSoulsPerSecond,

    BaseSoulsPerCollection,
    SoulsPerCollectionPercentage,
    FlatSoulsPerCollection,

    //BaseSpeed,
    //SpeedPercentage,
    //FlatSpeed,
}