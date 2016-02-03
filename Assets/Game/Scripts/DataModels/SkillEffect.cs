using UnityEngine;
using System.Collections;

public class SkillEffect
{
    public BasicStats StatsModifiers { get; set; }

    public Targeting EffectTarget { get; set; }

    public Const.EnmityTargetType EnmityType { get; set; }

    public int BaseEnmity { get; set; }

    public static SkillEffect MeleeAttackEffect()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 1d, 0d, 0d, 0d, 0d);
        skillEffect.EffectTarget = Targeting.SingleOpponentTarget();
        skillEffect.BaseEnmity = 10;
        skillEffect.EnmityType = Const.EnmityTargetType.Target;
        return skillEffect;
    }

    public static SkillEffect CrossSlashEffect()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 2d, 0d, 0d, 0d, 0d);
        skillEffect.EffectTarget = Targeting.CrossOpponentTarget();
        skillEffect.BaseEnmity = 15;
        skillEffect.EnmityType = Const.EnmityTargetType.Target;
        return skillEffect;
    }

    public static SkillEffect SquashEffect()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 0.5d, 0d, 0d, 0d, 0d);
        skillEffect.EffectTarget = Targeting.SquashTarget();
        skillEffect.BaseEnmity = 30;
        skillEffect.EnmityType = Const.EnmityTargetType.Target;
        return skillEffect;
    }

    public static SkillEffect ChainLightning()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 2d, 0d, 0d, 0d, 0d);
        skillEffect.EffectTarget = Targeting.ChainLightning();
        skillEffect.BaseEnmity = 40;
        skillEffect.EnmityType = Const.EnmityTargetType.Target;
        return skillEffect;
    }

    public static SkillEffect ChainLightningSecondary()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 1d, 0d, 0d, 0d, 0d);
        skillEffect.EffectTarget = Targeting.ChainLightningSecondary();
        skillEffect.BaseEnmity = 20;
        skillEffect.EnmityType = Const.EnmityTargetType.Target;
        return skillEffect;
    }
}
