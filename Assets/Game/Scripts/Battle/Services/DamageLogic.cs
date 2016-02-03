using System;
using System.Collections;

public class DamageLogic
{
    public static double CalculateDamage(BattleCharacter attacker, BattleCharacter defender, SkillEffect effect)
    {
        // TODO: Real damage logic, Resistance
        var strMod = effect.StatsModifiers.GetStats(Const.BasicStats.Attack);
        var wisMod = effect.StatsModifiers.GetStats(Const.BasicStats.Wisdom);
        var mndMod = effect.StatsModifiers.GetStats(Const.BasicStats.Mind);

        var damage = 0d;
        if (strMod != 0d)
        {
            damage += Math.Floor(attacker.BaseCharacter.Attack * strMod) - defender.BaseCharacter.Defense;
        }

        if (wisMod != 0d)
        {
            damage += Math.Floor(attacker.BaseCharacter.Wisdom * wisMod) - defender.BaseCharacter.Mind;
        }

        if (mndMod != 0d)
        {
            damage += Math.Floor(attacker.BaseCharacter.Mind * mndMod);
        }

        damage = ApplyAffinityBonuses(damage, defender.BaseCharacter.Resistances, effect.Affinities);

        return damage;
    }

    private static double ApplyAffinityBonuses(double baseDamage, Affinity resistances, Affinity effectAffinities)
    {
        if (resistances == null || effectAffinities == null)
        {
            return baseDamage;
        }

        var damage = baseDamage;
        // TODO: Confirm affinity spec
        foreach (var kv in effectAffinities.GetNonZeroAffinities())
        {
            var resistance = resistances.GetAffinity(kv.Key);
            var percentage = 1d - resistance;
            damage *= percentage;
        }
        return damage;
    }
}
