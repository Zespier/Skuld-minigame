using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/FlySpear")]
public class FlySpear : SkillBase
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

        controllerRef.UpSkillImpulse();


        Debug.Log($"{skillName} lanzada!");

        // Iniciar cooldown
        StartCooldown(caster.GetComponent<MonoBehaviour>());
    }
}
