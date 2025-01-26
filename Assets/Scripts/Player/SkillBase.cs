using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : ScriptableObject {
    [Header("Skill Settings")]
    public string skillName;
    public float cooldownTime;
    public Sprite skillIcon;

    [HideInInspector] public float currentCooldown;

    [HideInInspector] public bool isOnCooldown;

    /// <summary>
    /// Ejecuta la habilidad.
    /// </summary>
    /// <param name="caster">El GameObject que lanza la habilidad.</param>
    public abstract void Execute(GameObject caster);

    /// <summary>
    /// Inicia el cooldown.
    /// </summary>
    public void StartCooldown(MonoBehaviour owner) {
        if (isOnCooldown) return;
        isOnCooldown = true;
        owner.StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine() {
        currentCooldown = cooldownTime;
        while (currentCooldown > 0) {
            currentCooldown -= Time.deltaTime;
            yield return null;
        }
        currentCooldown = 0;
        isOnCooldown = false;
    }
}

