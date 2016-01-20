using UnityEngine;
using System.Collections;

public class SkillEffect
{
    public BasicStats StatsModifiers { get; set; }

    public EffectTarget EffectTarget { get; set; }

    public static SkillEffect MeleeAttackEffect()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 1d, 0d, 0d, 0d);
        skillEffect.EffectTarget = EffectTarget.SingleOpponentTarget();
        return skillEffect;
    }

    public static SkillEffect CrossSlashEffect()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 2d, 0d, 0d, 0d);
        skillEffect.EffectTarget = EffectTarget.CrossOpponentTarget();
        return skillEffect;
    }

    public static SkillEffect SquashEffect()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 0.3d, 0d, 0d, 0d);
        skillEffect.EffectTarget = EffectTarget.SquashTarget();
        return skillEffect;
    }

    public static SkillEffect ChainLightning()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 2d, 0d, 0d, 0d);
        skillEffect.EffectTarget = EffectTarget.ChainLightning();
        return skillEffect;
    }

    public static SkillEffect ChainLightningSecondary()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 1d, 0d, 0d, 0d);
        skillEffect.EffectTarget = EffectTarget.ChainLightningSecondary();
        return skillEffect;
    }
}
