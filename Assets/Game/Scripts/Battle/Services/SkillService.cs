using System;
using System.Linq;
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
        
    public MapPosition SelectDefaultTargetForSkill(BattleCharacter actor, Skill selectedSkill, List<MapPosition> skillRadius, List<BattleCharacter> characters, Dictionary<MapPosition, Tile> map)
    {
        // TODO: better select default target logic. Save last target maybe

        var targetTeam = this.GetPreferredSkillTargetTeam(selectedSkill, actor.Team);

        foreach (var character in characters.FindAll( x => x.Team == targetTeam))
        {
            var position = this.GetPositionToHitCharacterWithSkill(selectedSkill, skillRadius, character, actor.Team, map);
            if (position != null)
            {
                return position;
            }
        }
        return null;
    }

    /// <summary>
    /// Gets a position to cast a skill on so that the skill can hit the target character
    /// </summary>
    /// <returns>The position to hit character with skill.</returns>
    /// <param name="skill">Skill.</param>
    /// <param name="skillRadius">Skill radius.</param>
    /// <param name="target">Target.</param>
    /// <param name="sourceTeam">Source team.</param>
    /// <param name="map">The full map.</param>
    private MapPosition GetPositionToHitCharacterWithSkill(Skill skill, List<MapPosition> skillRadius, BattleCharacter target, Const.Team sourceTeam, Dictionary<MapPosition, Tile> map)
    {
        foreach (var mapPosition in skillRadius)
        {
            var targeting = skill.Effects[0].EffectTarget;
            var affectedPositions = ServiceFactory.GetMapService().GeMapPositionsForPattern(targeting.Pattern, targeting.TargetGroup, sourceTeam, map, mapPosition);
            if (affectedPositions.Intersect(target.OccupiedMapPositions).Count() > 0)
            {
                return mapPosition;
            }
        }
        return null;
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

    private Const.Team GetPreferredSkillTargetTeam(Skill skill, Const.Team actorTeam)
    {
        switch (skill.SkillType)
        {
            case Const.SkillType.Attack:
            case Const.SkillType.Debuff:
                return this.GetOpponentTeam(actorTeam);
            case Const.SkillType.Buff:
            case Const.SkillType.Heal:
                return actorTeam;
            default:
                return this.GetOpponentTeam(actorTeam);
        }
    }

    private Const.Team GetOpponentTeam(Const.Team actorTeam)
    {
        if (actorTeam == Const.Team.Player)
        {
            return Const.Team.Enemy;
        }
        return Const.Team.Player;
    }

    private struct FinalStatValue
    {
        public double value;
        public bool isAbsolute;
    }
}
