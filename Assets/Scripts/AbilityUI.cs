using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour {

    public Abilities ability;
    public Image image;
    public Image shadow;
    public TMP_Text cooldown;

    private void Update() {
        switch (ability) {
            case Abilities.skillQ:
                image.sprite = SkillManager.instance.skillQ.skillIcon;
                shadow.fillAmount = SkillManager.instance.skillQ.currentCooldown / SkillManager.instance.skillQ.cooldownTime;
                cooldown.text = SkillManager.instance.skillQ.currentCooldown <= 0 ? "" : SkillManager.instance.skillQ.currentCooldown.ToString("F1") + "s";
                break;
            case Abilities.skillW:
                image.sprite = SkillManager.instance.skillW.skillIcon;
                shadow.fillAmount = SkillManager.instance.skillW.currentCooldown / SkillManager.instance.skillW.cooldownTime;
                cooldown.text = SkillManager.instance.skillW.currentCooldown <= 0 ? "" : SkillManager.instance.skillW.currentCooldown.ToString("F1") + "s";
                break;
            case Abilities.skillE:
                image.sprite = SkillManager.instance.skillE.skillIcon;
                shadow.fillAmount = SkillManager.instance.skillE.currentCooldown / SkillManager.instance.skillE.cooldownTime;
                cooldown.text = SkillManager.instance.skillE.currentCooldown <= 0 ? "" : SkillManager.instance.skillE.currentCooldown.ToString("F1") + "s";
                break;
            case Abilities.skillR:
                image.sprite = SkillManager.instance.skillR.skillIcon;
                shadow.fillAmount = SkillManager.instance.skillR.currentCooldown / SkillManager.instance.skillR.cooldownTime;
                cooldown.text = SkillManager.instance.skillR.currentCooldown <= 0 ? "" : SkillManager.instance.skillR.currentCooldown.ToString("F1") + "s";
                break;
            default:
                break;
        }
    }
}

public enum Abilities {
    skillQ,
    skillW,
    skillE,
    skillR,
}