using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SkillService
{
    public bool ShouldHit(BattleCharacter attacker, BattleCharacter defender, SkillEffect effect)
    {
        var accModifiers = this.GetStatModifier(effect.StatsModifiers, Const.BasicStats.Accuracy);
        var finalValue = this.CalculateModifiedValue(attacker.BaseCharacter.Accuracy, accModifiers);

        var hitChance = this.GetStatEffect(attacker.BaseCharacter.Accuracy, accModifiers, defender.BaseCharacter.Evasion); 

        return this.IsRandomCheckSuccess(hitChance);
    }

    public bool ShouldCritical(BattleCharacter attacker, BattleCharacter defender, SkillEffect effect)
    {
        var critModifiers = this.GetStatModifier(effect.StatsModifiers, Const.BasicStats.Critical);
        var finalValue = this.CalculateModifiedValue(attacker.BaseCharacter.Critical, critModifiers);

        // TODO: critical resistance?
        var hitChance = this.GetStatEffect(attacker.BaseCharacter.Critical, critModifiers, 0d); 
        return this.IsRandomCheckSuccess(finalValue.value);
    }

    public double GetStatEffect(BattleCharacter attacker, BattleCharacter defender, SkillEffect effect, bool shouldCritical)
    {
        var strModifier = this.GetStatModifier(effect.StatsModifiers, Const.BasicStats.Attack);
        var wisModifier = this.GetStatModifier(effect.StatsModifiers, Const.BasicStats.Wisdom);
        var mndModifier = this.GetStatModifier(effect.StatsModifiers, Const.BasicStats.Mind);

        var damage = 0d;
        if (strModifier.Count > 0)
        {
            damage += this.GetStatEffect(attacker.BaseCharacter.Attack, strModifier, defender.BaseCharacter.Defense);
        }

        if (wisModifier.Count > 0)
        {
            damage += this.GetStatEffect(attacker.BaseCharacter.Wisdom, wisModifier, defender.BaseCharacter.Mind);
        }

        if (mndModifier.Count > 0)
        {
            damage += this.GetStatEffect(attacker.BaseCharacter.Mind, mndModifier, 0d);
        }

        if (shouldCritical)
        {
            damage *= Const.CriticalDamageMultiplier;
        }

        damage = ApplyAffinityBonuses(damage, defender.BaseCharacter.Resistances, effect.Affinities);

        return Math.Floor(damage);
    }

    private double GetStatEffect(double attackerBaseValue, Dictionary<Const.ModifierType, double> attackerModifiers, double defenderBaseValue)
    {
        var finalAttackValue = this.CalculateModifiedValue(attackerBaseValue, attackerModifiers);
        if (finalAttackValue.isAbsolute)
        {
            return finalAttackValue.value;
        }
        return finalAttackValue.value - defenderBaseValue;
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

    private Dictionary<Const.ModifierType, double> GetStatModifier(List<StatModifier> bonues, Const.BasicStats stat)
    {
        var dict = new Dictionary<Const.ModifierType, double>();
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

    private ModifiedValue CalculateModifiedValue(double baseValue, Dictionary<Const.ModifierType, double> bonuses)
    {
        var value = baseValue;
        var finalValue = new ModifiedValue();
        if (bonuses.ContainsKey(Const.ModifierType.Absolute))
        {
            finalValue.value = bonuses[Const.ModifierType.Absolute];
            finalValue.isAbsolute = true;
        }
        else
        {
            if (bonuses.ContainsKey(Const.ModifierType.Multiply))
            {
                value *= bonuses[Const.ModifierType.Multiply];
            }

            if (bonuses.ContainsKey(Const.ModifierType.Addition))
            {
                value += bonuses[Const.ModifierType.Addition];
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

    private struct ModifiedValue
    {
        public double value;
        public bool isAbsolute;
    }
}
