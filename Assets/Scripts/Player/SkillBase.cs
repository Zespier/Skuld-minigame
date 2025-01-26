using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    [Header("Skill Settings")]
    public string skillName;
    public float cooldownTime;
    public Sprite skillIcon;

    protected bool isOnCooldown;

    /// <summary>
    /// Ejecuta la habilidad.
    /// </summary>
    /// <param name="caster">El GameObject que lanza la habilidad.</param>
    public abstract void Execute(GameObject caster);

    /// <summary>
    /// Inicia el cooldown.
    /// </summary>
    public void StartCooldown(MonoBehaviour owner)
    {
        if (isOnCooldown) return;
        isOnCooldown = true;
        owner.StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}

