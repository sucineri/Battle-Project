using System;
using System.Collections;
using System.Collections.Generic;

public class SkillService
{
    public bool ShouldHit(BattleCharacter attacker, BattleCharacter defender, SkillEffect effect)
    {
        var accBonuses = this.GetBonuesForStat(effect.StatsBonues, Const.BasicStats.Accuracy);
        var finalValue = this.CalculateFinalStatValue(attacker.BaseCharacter.Accuracy, accBonuses);

        var hitChance = 0d;
        if (finalValue.isAbsolute)
        {
            hitChance = finalValue.value;
        }
        else
        {
            hitChance = finalValue.value - defender.BaseCharacter.Evasion;
        }
        return this.IsRandomCheckSuccess(hitChance);
    }

    public bool ShouldCritical(BattleCharacter attacker, BattleCharacter defender, SkillEffect effect)
    {
        var critBonuses = this.GetBonuesForStat(effect.StatsBonues, Const.BasicStats.Critical);
        var finalValue = this.CalculateFinalStatValue(attacker.BaseCharacter.Critical, critBonuses);
        return this.IsRandomCheckSuccess(finalValue.value);
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

    private Dictionary<Const.StatBonusType, double> GetBonuesForStat(List<StatBonus> bonues, Const.BasicStats stat)
    {
        var dict = new Dictionary<Const.StatBonusType, double>();
        foreach (var bonus in bonues)
        {       
            if (bonus.Stat == stat)
            {
                var type = bonus.Type;  
                if(dict.ContainsKey(type))
                {
                    dict[type] += bonus.Magnitude;
                }
                else
                {
                    dict.Add(type, bonus.Magnitude);
                }
            }
        }
        return dict;
    }

    private FinalStatValue CalculateFinalStatValue(double baseValue, Dictionary<Const.StatBonusType, double> bonuses)
    {
        var value = baseValue;
        var finalValue = new FinalStatValue();
        if (bonuses.ContainsKey(Const.StatBonusType.Absolute))
        {
            finalValue.value = bonuses[Const.StatBonusType.Absolute];
            finalValue.isAbsolute = true;
        }
        else
        {
            if (bonuses.ContainsKey(Const.StatBonusType.Multiply))
            {
                value *= bonuses[Const.StatBonusType.Multiply];
            }

            if (bonuses.ContainsKey(Const.StatBonusType.Addition))
            {
                value += bonuses[Const.StatBonusType.Addition];
            }

            finalValue.value = value;
            finalValue.isAbsolute = false;
        }
        return finalValue;
    }

    private struct FinalStatValue
    {
        public double value;
        public bool isAbsolute;
    }
}
