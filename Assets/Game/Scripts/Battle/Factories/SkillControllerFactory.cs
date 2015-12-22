using UnityEngine;
using System.Collections;

public class SkillControllerFactory
{
    public static SkillController CreateSkillController(Skill skill)
    {
        var prefab = Resources.Load(skill.PrefabPath) as GameObject;
        var go = GameObject.Instantiate(prefab) as GameObject;
        var controller = go.GetComponent<SkillController>();
        return controller;
    }
}
