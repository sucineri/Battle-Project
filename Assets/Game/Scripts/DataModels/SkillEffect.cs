using UnityEngine;
using System.Collections;

public class SkillEffect
{
    public BasicStats StatsModifiers { get; set; }

    public Targeting EffectTarget { get; set; }

    public static SkillEffect MeleeAttackEffect()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 1d, 0d, 0d, 0d);
        skillEffect.EffectTarget = Targeting.SingleOpponentTarget();
        return skillEffect;
    }

    public static SkillEffect CrossSlashEffect()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 2d, 0d, 0d, 0d);
        skillEffect.EffectTarget = Targeting.CrossOpponentTarget();
        return skillEffect;
    }

    public static SkillEffect SquashEffect()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 0.3d, 0d, 0d, 0d);
        skillEffect.EffectTarget = Targeting.SquashTarget();
        return skillEffect;
    }

    public static SkillEffect ChainLightning()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 2d, 0d, 0d, 0d);
        skillEffect.EffectTarget = Targeting.ChainLightning();
        return skillEffect;
    }

    public static SkillEffect ChainLightningSecondary()
    {
        var skillEffect = new SkillEffect();
        skillEffect.StatsModifiers = new BasicStats(0d, 0d, 1d, 0d, 0d, 0d);
        skillEffect.EffectTarget = Targeting.ChainLightningSecondary();
        return skillEffect;
    }
}
