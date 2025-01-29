using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/CutSpear")]
public class CutSpear : SkillBase
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

        controllerRef.StartCoroutineSkill2();

        StartCooldown(caster.GetComponent<MonoBehaviour>());
    }
}
