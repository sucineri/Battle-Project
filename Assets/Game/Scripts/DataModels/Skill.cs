﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill
{
    public string Name { get; set; }

    public Targeting SkillTargetArea { get; set; }

    public List<SkillEffect> Effects = new List<SkillEffect>();

    public int NumberOfTriggers { get; set; }

    public string PrefabPath { get; set; }

    public string EffectPrefabPath { get; set; }

    public int Rank { get; set; }

    public Const.EnmityTargetType EnmityType { get; set; }

    public int BaseEnmity { get; set; }

    public static Skill MeleeAttack()
    {
        var skill = new Skill();
        skill.Name = "Attack";
        skill.Effects.Add(SkillEffect.MeleeAttackEffect());
        skill.NumberOfTriggers = 1;
        skill.PrefabPath = "Skills/MeleeAttack";
        skill.EffectPrefabPath = "Effects/Explosion";
        skill.SkillTargetArea = Targeting.MeleeTargetArea();
        skill.Rank = 3;
        skill.BaseEnmity = 10;
        skill.EnmityType = Const.EnmityTargetType.Target;
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
        skill.SkillTargetArea = Targeting.MeleeTargetArea();
        skill.Rank = 4;
        skill.BaseEnmity = 15;
        skill.EnmityType = Const.EnmityTargetType.Target;
        return skill;
    }

    public static Skill Squash()
    {
        var skill = new Skill();
        skill.Name = "Squash";
        skill.Effects.Add(SkillEffect.SquashEffect());
        skill.NumberOfTriggers = 1;
        skill.PrefabPath = "Skills/Squash";
        skill.EffectPrefabPath = "Effects/Explosion";
        skill.SkillTargetArea = Targeting.MeleeTargetArea();
        skill.Rank = 5;
        skill.BaseEnmity = 30;
        skill.EnmityType = Const.EnmityTargetType.Target;
        return skill;
    }

    public static Skill DoubleCross()
    {
        var skill = new Skill();
        skill.Name = "Double Cross";
        skill.Effects.Add(SkillEffect.CrossSlashEffect());
        skill.Effects.Add(SkillEffect.CrossSlashEffect());
        skill.NumberOfTriggers = 1;
        skill.PrefabPath = "Skills/DoubleCross";
        skill.EffectPrefabPath = "Effects/Explosion";
        skill.SkillTargetArea = Targeting.MeleeTargetArea();
        skill.Rank = 5;
        skill.BaseEnmity = 30;
        skill.EnmityType = Const.EnmityTargetType.Target;
        return skill;
    }

    public static Skill ChainLightning()
    {
        var skill = new Skill();
        skill.Name = "Chain Lightning";
        skill.Effects.Add(SkillEffect.ChainLightning());
        skill.Effects.Add(SkillEffect.ChainLightningSecondary());
        skill.Effects.Add(SkillEffect.ChainLightningSecondary());
        skill.NumberOfTriggers = 1;
        skill.PrefabPath = "Skills/ChainLightning";
        skill.EffectPrefabPath = "Effects/Explosion";
        skill.SkillTargetArea = Targeting.MeleeTargetArea();
        skill.Rank = 5;
        skill.BaseEnmity = 40;
        skill.EnmityType = Const.EnmityTargetType.Target;
        return skill;
    }
}
