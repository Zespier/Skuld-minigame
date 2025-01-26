using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/BoomSpear")]
public class BoomSpear : SkillBase
{
    private PlayerController controllerRef;

    public override void Execute(GameObject caster)
    {

        if (isOnCooldown)
        {
            Debug.Log($"{skillName} está en cooldown.");
            return;
        }

        caster.TryGetComponent<PlayerController>(out controllerRef);

        if(controllerRef.IsInIdleSide) return;

        controllerRef.StartCoroutineSkill3();

        Debug.Log($"{skillName} lanzada!");

        // Iniciar cooldown
        StartCooldown(caster.GetComponent<MonoBehaviour>());
    }
}
