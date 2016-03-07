using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SkillService
{
    private Random _random = new Random();

    public bool ShouldHit(BattleCharacter attacker, BattleCharacter defender, SkillEffect effect)
    {
        var modifiedStat = attacker.GetStatWithModifiers(Const.Stats.Accuracy, effect.StatsModifiers);
        var hitChance = modifiedStat.Value;
        var evaChance = modifiedStat.IsAbsolute ? 0d :defender.GetStat(Const.Stats.Evasion);

        return this.IsRandomCheckSuccess(hitChance - evaChance);
    }

    public bool ShouldCritical(BattleCharacter attacker, BattleCharacter defender, SkillEffect effect)
    {
        var critChance = attacker.GetStatWithModifiers(Const.Stats.Critical, effect.StatsModifiers);
        return this.IsRandomCheckSuccess(critChance.Value);
    }

    public double CalculateDamage(BattleCharacter attacker, BattleCharacter defender, SkillEffect effect, bool shouldCritical)
    {
        var damage = 0d;
        if (effect.HasStatModifier(Const.Stats.Attack))
        {
            var modifiedAtk = attacker.GetStatWithModifiers(Const.Stats.Attack, effect.StatsModifiers);
            damage += modifiedAtk.Value - defender.GetStat(Const.Stats.Defense);
        }

        if (effect.HasStatModifier(Const.Stats.Wisdom))
        {
            var modifiedWis = attacker.GetStatWithModifiers(Const.Stats.Wisdom, effect.StatsModifiers);
            damage += modifiedWis.Value - defender.GetStat(Const.Stats.Mind);
        }

        if (effect.HasStatModifier(Const.Stats.Mind))
        {
            damage += attacker.GetStatWithModifiers(Const.Stats.Mind, effect.StatsModifiers).Value;
        }

        if (shouldCritical)
        {
            damage *= Const.CriticalDamageMultiplier;
        }

        // damage value needs to be positive or zero at this point so a low atk doesn't heal a high def without going thru affinities 
        damage = Math.Max(0d, damage);

        damage = ApplyAffinityBonuses(damage, defender, effect.Affinities);

        return Math.Floor(damage);
    }
        
    public MapPosition SelectDefaultTargetForSkill(BattleCharacter actor, Skill selectedSkill, List<MapPosition> skillRadius, List<BattleCharacter> characters, Dictionary<MapPosition, Tile> map)
    {
        // TODO: better select default target logic. Save last target maybe

        var targetTeam = this.GetPreferredSkillTargetTeam(selectedSkill.Effects[0], actor.Team);

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
        return _random.NextDouble() >= (1d - chance);
    }

    private double ApplyAffinityBonuses(double baseDamage, BattleCharacter defender, SkillEffectAffinities effectAffinities)
    {
        var nonZeroAffinities = effectAffinities.GetNonZeroAffinities();

        if (nonZeroAffinities.Count == 0)
        {
            return baseDamage;
        }

        var modifiedDamage = 0d;

        foreach (var kv in nonZeroAffinities)
        {
            var resistanceEffect = 1d - defender.GetAffinityResistance(kv.Key);
            modifiedDamage += baseDamage * resistanceEffect * kv.Value;
        }
        return modifiedDamage;
    }

    private Dictionary<Const.ModifierType, double> GetStatModifier(List<StatModifier> modifiers, Const.Stats stat)
    {
        var dict = new Dictionary<Const.ModifierType, double>();
        foreach (var bonus in modifiers)
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

    private Const.Team GetPreferredSkillTargetTeam(SkillEffect skillEffect, Const.Team actorTeam)
    {
        switch (skillEffect.EffectType)
        {
            case Const.SkillEffectType.Attack:
            case Const.SkillEffectType.Debuff:
                return this.GetOpponentTeam(actorTeam);
            case Const.SkillEffectType.Buff:
            case Const.SkillEffectType.Heal:
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
}  
