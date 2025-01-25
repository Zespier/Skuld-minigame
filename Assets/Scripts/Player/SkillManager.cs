using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("Habilidades activas")]
    public SkillBase skillQ;
    public SkillBase skillW;
    public SkillBase skillE;
    public SkillBase skillR;

    private void Update()
    {
        // Detectar teclas y lanzar habilidades
        if (Input.GetKeyDown(KeyCode.Q)) ExecuteSkill(skillQ);
        if (Input.GetKeyDown(KeyCode.W)) ExecuteSkill(skillW);
        if (Input.GetKeyDown(KeyCode.E)) ExecuteSkill(skillE);
        if (Input.GetKeyDown(KeyCode.R)) ExecuteSkill(skillR);
    }

    private void ExecuteSkill(SkillBase skill)
    {
        if (skill != null)
        {
            skill.Execute(gameObject);
        }
    }
}
