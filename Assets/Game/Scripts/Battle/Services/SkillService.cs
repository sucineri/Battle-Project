using System;
using System.Collections;

public class SkillService
{
    public bool ShouldHit(BattleCharacter attacker, BattleCharacter defender, SkillEffect effect)
    {
        var hitChance = attacker.BaseCharacter.Accuracy - defender.BaseCharacter.Evasion;
        return this.IsRandomCheckSuccess(hitChance);
    }

    public bool ShouldCritical(BattleCharacter attacker, BattleCharacter defender, SkillEffect effect)
    {
        var criticalChance = attacker.BaseCharacter.Critical;
        return this.IsRandomCheckSuccess(criticalChance);
    }

    public double CalculateDamage(BattleCharacter attacker, BattleCharacter defender, SkillEffect effect, bool shouldCritical)
    {
        // TODO: Real damage logic, Resistance
        var strMod = effect.StatsModifiers.GetStats(Const.BasicStats.Attack);
        var wisMod = effect.StatsModifiers.GetStats(Const.BasicStats.Wisdom);
        var mndMod = effect.StatsModifiers.GetStats(Const.BasicStats.Mind);

        var damage = 0d;
        if (strMod != 0d)
        {
            damage += attacker.BaseCharacter.Attack * strMod - defender.BaseCharacter.Defense;
        }

        if (wisMod != 0d)
        {
            damage += attacker.BaseCharacter.Wisdom * wisMod - defender.BaseCharacter.Mind;
        }

        if (mndMod != 0d)
        {
            damage += attacker.BaseCharacter.Mind * mndMod;
        }

        if (shouldCritical)
        {
            damage *= Const.CriticalDamageMultiplier;
        }

        damage = ApplyAffinityBonuses(damage, defender.BaseCharacter.Resistances, effect.Affinities);

        return Math.Floor(damage);
    }

    private bool IsRandomCheckSuccess(double chance)
    {
        // range of random.NextDouble() is [0, 0.99999999999999978];
        var random = new Random();
        return random.NextDouble() >= (1d - chance);
    }

    private double ApplyAffinityBonuses(double baseDamage, Affinity resistances, Affinity effectAffinities)
    {
        if (resistances == null || effectAffinities == null)
        {
            return baseDamage;
        }

        var nonZeroAffinities = effectAffinities.GetNonZeroAffinities();

        if (nonZeroAffinities.Count == 0)
        {
            return baseDamage;
        }

        var modifiedDamage = 0d;

        foreach (var kv in nonZeroAffinities)
        {
            var resistanceEffect = 1d - resistances.GetAffinity(kv.Key);
            modifiedDamage += baseDamage * resistanceEffect * kv.Value;
        }
        return modifiedDamage;
    }
}
