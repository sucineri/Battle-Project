﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillEffect
{
    // Skill effect's affinities
    public SkillEffectAffinities Affinities { get; set; }

    // Targeting information
    public Targeting EffectTarget { get; set; }

    // How emity is calculated when the skill effect is applied
    public Const.EnmityTargetType EnmityType { get; set; }

    // Base enmity generated by the skill effect
    public int BaseEnmity { get; set; }

    // Modifiers used to calculate skill effects 
    public List<StatModifier> StatsModifiers { get; set; }

    public List<StatusEffect> StatusEffects { get; set; }

    public Const.SkillEffectType EffectType { get; set; }

    public bool HasDamageEffect { get { return this.Affinities != null; } }

    public bool HasStatusEffect { get { return this.StatusEffects.Count > 0; } }

    public bool IsEmptyEffect { get { return this.EffectType == Const.SkillEffectType.None; } }

    public SkillEffect()
    {
        StatsModifiers = new List<StatModifier>();
        StatusEffects = new List<StatusEffect>();
    }

    public bool HasStatModifier(Const.Stats stat)
    {
        var mod = this.StatsModifiers.Find(x =>{
            return stat == x.Stat;
        });
        return mod != null;
    }

    private void AddStatModifier(Const.Stats stat, double magnitude, Const.ModifierType bonusType)
    {
        this.StatsModifiers.Add(new StatModifier(stat, magnitude, bonusType));
    }

    private void AddStatusEffect(StatusEffect effect)
    {
        this.StatusEffects.Add(effect);
    }

    public static SkillEffect MeleeAttackEffect()
    {
        var skillEffect = new SkillEffect();

        skillEffect.EffectTarget = Targeting.SingleOpponentTarget();

        var affinities = new SkillEffectAffinities();
        affinities.SetAffinity(Const.Affinities.Physical, 1d);
        skillEffect.Affinities = affinities;

        skillEffect.BaseEnmity = 10;
        skillEffect.EnmityType = Const.EnmityTargetType.Target;

        skillEffect.EffectType = Const.SkillEffectType.Attack;

        skillEffect.AddStatModifier(Const.Stats.Attack, 1d, Const.ModifierType.Multiply);

        skillEffect.AddStatusEffect(StatusEffect.Blind());
        return skillEffect;
    }

    public static SkillEffect CrossSlashEffect()
    {
        var skillEffect = new SkillEffect();

        skillEffect.EffectTarget = Targeting.CrossOpponentTarget();

        var affinities = new SkillEffectAffinities();
        affinities.SetAffinity(Const.Affinities.Physical, 1d);
        skillEffect.Affinities = affinities;

        skillEffect.BaseEnmity = 15;
        skillEffect.EnmityType = Const.EnmityTargetType.Target;

        skillEffect.EffectType = Const.SkillEffectType.Attack;

        skillEffect.AddStatModifier(Const.Stats.Attack, 2d, Const.ModifierType.Multiply);
        return skillEffect;
    }

    public static SkillEffect SquashEffect()
    {
        var skillEffect = new SkillEffect();

        skillEffect.EffectTarget = Targeting.SquashTarget();

        var affinities = new SkillEffectAffinities();
        affinities.SetAffinity(Const.Affinities.Physical, 1d);
        skillEffect.Affinities = affinities;

        skillEffect.BaseEnmity = 30;
        skillEffect.EnmityType = Const.EnmityTargetType.Target;

        skillEffect.EffectType = Const.SkillEffectType.Attack;

        skillEffect.AddStatModifier(Const.Stats.Attack, 0.5d, Const.ModifierType.Multiply);
        skillEffect.AddStatusEffect(StatusEffect.Poison());
        return skillEffect;
    }

    public static SkillEffect ChainLightning()
    {
        var skillEffect = new SkillEffect();

        skillEffect.EffectTarget = Targeting.ChainLightning();

        var affinities = new SkillEffectAffinities();
        affinities.SetAffinity(Const.Affinities.Lightning, 1d);
        skillEffect.Affinities = affinities;

        skillEffect.BaseEnmity = 40;
        skillEffect.EnmityType = Const.EnmityTargetType.Target;

        skillEffect.EffectType = Const.SkillEffectType.Attack;

        skillEffect.AddStatModifier(Const.Stats.Accuracy, 1d, Const.ModifierType.Absolute);
        skillEffect.AddStatModifier(Const.Stats.Wisdom, 1.5d, Const.ModifierType.Multiply);
        return skillEffect;
    }

    public static SkillEffect ChainLightningSecondary()
    {
        var skillEffect = new SkillEffect();

        skillEffect.EffectTarget = Targeting.ChainLightningSecondary();

        var affinities = new SkillEffectAffinities();
        affinities.SetAffinity(Const.Affinities.Lightning, 1d);
        skillEffect.Affinities = affinities;

        skillEffect.BaseEnmity = 20;
        skillEffect.EnmityType = Const.EnmityTargetType.Target;

        skillEffect.EffectType = Const.SkillEffectType.Attack;

        skillEffect.AddStatModifier(Const.Stats.Accuracy, 1d, Const.ModifierType.Absolute);
        skillEffect.AddStatModifier(Const.Stats.Wisdom, 1d, Const.ModifierType.Multiply);
        return skillEffect;
    }

    public static SkillEffect MinorHealEffect()
    {
        var skillEffect = new SkillEffect();

        skillEffect.EffectTarget = Targeting.SingleTarget();

        var affinities = new SkillEffectAffinities();
        affinities.SetAffinity(Const.Affinities.Healing, 1d);
        skillEffect.Affinities = affinities;

        skillEffect.BaseEnmity = 40;
        skillEffect.EnmityType = Const.EnmityTargetType.All;

        skillEffect.EffectType = Const.SkillEffectType.Heal;

        skillEffect.AddStatModifier(Const.Stats.Accuracy, 1d, Const.ModifierType.Absolute);
        skillEffect.AddStatModifier(Const.Stats.Mind, 3d, Const.ModifierType.Multiply);

        skillEffect.AddStatusEffect(StatusEffect.PoisonResistanceUp());
        return skillEffect;
    }

    public static SkillEffect WaitEffect()
    {
        var skillEffect = new SkillEffect();

        skillEffect.EffectTarget = Targeting.SingleTarget();
        skillEffect.BaseEnmity = 0;
        skillEffect.EnmityType = Const.EnmityTargetType.Target;

        skillEffect.EffectType = Const.SkillEffectType.None;
        skillEffect.AddStatModifier(Const.Stats.Accuracy, 1d, Const.ModifierType.Absolute);

        return skillEffect;

    }
}
