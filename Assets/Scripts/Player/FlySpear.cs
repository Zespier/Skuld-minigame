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
            return;
        }

        caster.TryGetComponent<PlayerController>(out controllerRef);

        controllerRef.StartCoroutineSkill1();

        StartCooldown(caster.GetComponent<MonoBehaviour>());
    }
}
