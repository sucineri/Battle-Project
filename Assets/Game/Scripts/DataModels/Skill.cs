using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill
{
    public string Name { get; set; }

    public List<SkillEffect> Effects = new List<SkillEffect>();

    public int NumberOfTriggers { get; set; }

    public Targetting SkillTarget { get; set; }

    public string PrefabPath { get; set; }

    public string EffectPrefabPath { get; set; }

    protected Skill()
    {
        // no public default constructor
    }

    public static Skill MeleeAttack()
    {
        var skill = new Skill();
        skill.Name = "Attack";
        skill.Effects.Add(SkillEffect.MeleeAttackEffect());
        skill.NumberOfTriggers = 1;
        skill.PrefabPath = "Skills/MeleeAttack";
        skill.EffectPrefabPath = "Effects/Explosion";
        skill.SkillTarget = Targetting.SingleOpponentTarget();
        return skill;
    }

    public static Skill CrossSlash()
    {
        var skill = new Skill();
        skill.Name = "Cross Slash";
        skill.Effects.Add(SkillEffect.CrossSlashEffect());
        skill.NumberOfTriggers = 1;
        skill.PrefabPath = "Skills/CrossSlash";
        skill.EffectPrefabPath = "Effects/Explosion";
        skill.SkillTarget = Targetting.CrossOpponentTarget();
        return skill;
    }
}
