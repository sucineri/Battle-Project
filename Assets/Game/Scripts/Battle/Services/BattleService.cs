using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleService
{

    public List<BattleCharacter> GetCharactersAtPositions(List<BattleCharacter> characters, List<MapPosition> affectedPositions)
    {
        HashSet<BattleCharacter> affectedCharacters = new HashSet<BattleCharacter>();
        foreach (var position in affectedPositions)
        {
            var characterAtPosition = this.GetCharacterAtPosition(characters, position);
            if (characterAtPosition != null && !characterAtPosition.IsDead)
            {
                affectedCharacters.Add(characterAtPosition);
            }
        }
        return affectedCharacters.ToList();
    }

    public BattleCharacter GetCharacterAtPosition(List<BattleCharacter> characters, MapPosition targetPosition)
    {
        return characters.Find( x =>{
            return x.OccupiedMapPositions.Contains(targetPosition);
        });
    }

    public Queue<BattleActionResult> GetActionResults(Queue<BattleAction> actionQueue, Dictionary<MapPosition, Tile> map, List<BattleCharacter> characters)
    {
        var resultQueue = new Queue<BattleActionResult>();
        while (actionQueue.Count > 0)
        {
            var action = actionQueue.Dequeue();
            var actionResult = this.GetActionResult(action, map, characters);
            if (actionResult != null)
            {
                resultQueue.Enqueue(actionResult);
            }
        }
        return resultQueue;
    }

    private BattleActionResult GetActionResult(BattleAction action, Dictionary<MapPosition, Tile> map, List<BattleCharacter> characters)
    {
        switch (action.ActionType)
        {
            case Const.ActionType.Movement:
                return this.GetMovementActionResult(action, map, characters);
            case Const.ActionType.Skill:
                return this.GetSkillActionResult(action, map, characters);
            default:
                return null;
        }
    }

    private BattleActionResult GetMovementActionResult(BattleAction action, Dictionary<MapPosition, Tile> map, List<BattleCharacter> characters)
    {
        var actor = action.Actor;
        var moveTo = action.TargetPosition;

        var actionResult = new BattleActionResult();
        actionResult.type = Const.ActionType.Movement;
        actionResult.targetPosition = moveTo;
        actionResult.targetCharacter = actor;
        actionResult.actor = actor;

        var movementEffect = new BattleActionResult.EffectOnTarget();
        movementEffect.target = actor;
        movementEffect.positionChangeTo = moveTo;
        movementEffect.isSuccess = true;
        movementEffect.isCritical = false;

        var effectResult = new BattleActionResult.ActionEffectResult();
        effectResult.AddEffectOnTarget(movementEffect);

        actionResult.AddSkillEffectResult(effectResult);

        return actionResult;
    }
        
    private BattleActionResult GetSkillActionResult(BattleAction action, Dictionary<MapPosition, Tile> map, List<BattleCharacter> allCharacters)
    {
        var actor = action.Actor;
        var skill = action.SelectedSkill;

        var skillActionResult = new BattleActionResult();
        skillActionResult.type = Const.ActionType.Skill;
        skillActionResult.targetPosition = action.TargetPosition;
        skillActionResult.targetCharacter = this.GetCharacterAtPosition(allCharacters, action.TargetPosition);
        skillActionResult.actor = actor;
        skillActionResult.skill = skill;

        MapPosition prevTargetPosition = null;
        foreach (var effect in skill.Effects)
        {
            var affectedCharacters = this.GetTargets(actor, effect.EffectTarget, map, allCharacters, skillActionResult, ref prevTargetPosition);

            if (affectedCharacters.Count == 0)
            {
                continue;
            }

            var actionEffectResult = new BattleActionResult.ActionEffectResult();

            foreach (var affectedCharacter in affectedCharacters)
            {
                var resultOnTarget = this.GetEffectResultOnTarget(actor, affectedCharacter, effect, skill.EffectPrefabPath);
                actionEffectResult.AddEffectOnTarget(resultOnTarget);
            }
            skillActionResult.AddSkillEffectResult(actionEffectResult);
        }

        ServiceFactory.GetStatusEffectService().AddPostActionEffect(skillActionResult, actor);

        return skillActionResult;
    }

    private List<BattleCharacter> GetTargets(BattleCharacter actor, Targeting targeting, Dictionary<MapPosition, Tile> map, List<BattleCharacter> characters, BattleActionResult actionResult, ref MapPosition prevTargetPosition)
    {
        List<BattleCharacter> affectedCharacters = new List<BattleCharacter>();
        if (targeting.TargetSearchRule == Const.TargetSearchRule.Nearest)
        {
            if (prevTargetPosition != null)
            {
                var target = this.GetNearestTarget(actor, targeting, map, characters, actionResult, prevTargetPosition);
                if (target != null)
                {
                    prevTargetPosition = target.BasePosition;
                    affectedCharacters.Add(target);
                }
            }
        }
        else
        {
            if (prevTargetPosition == null)
            {
                prevTargetPosition = actionResult.targetPosition;
            }
            var affectedPositions = ServiceFactory.GetMapService().GeMapPositionsForPattern(targeting.Pattern, targeting.TargetGroup, actor.Team, map, prevTargetPosition);
            var potentialTargets = this.GetCharactersAtPositions(characters, affectedPositions);
            foreach (var potentialTarget in potentialTargets)
            {
                if (this.IsTargetValidForEffect(actor, potentialTarget, targeting))
                {
                    affectedCharacters.Add(potentialTarget);
                }
            }
        }

        return affectedCharacters;
    }

    private BattleCharacter GetNearestTarget(BattleCharacter actor, Targeting targeting, Dictionary<MapPosition, Tile> map, List<BattleCharacter> characters, BattleActionResult actionResult, MapPosition targetPosition)
    {
        var nearestDistance = int.MaxValue;
        BattleCharacter target = null;

        // TODO: another target rule that allows repeat?
        bool allowRepeat = false;

        foreach (var character in characters)
        {
            if (!allowRepeat && actionResult.IsCharacterAffected(character))
            {
                continue;
            }

            if (this.IsTargetValidForEffect(actor, character, targeting))
            {
                foreach (var characterOccupiedPosition in character.OccupiedMapPositions)
                {
                    var distance = characterOccupiedPosition.GetDistance(targetPosition);
                    if (distance < nearestDistance && distance > 0)
                    {
                        target = character;
                        nearestDistance = distance;
                    }
                }
            }
        }
        return target;
    }

    private bool IsTargetValidForEffect(BattleCharacter actor, BattleCharacter target, Targeting effect)
    {
        if (target.IsDead)
        {
            return false;
        }

        switch (effect.TargetGroup)
        {
            case Const.SkillTargetGroup.Ally:
                return actor.Team == target.Team;
            case Const.SkillTargetGroup.Opponent:
                return actor.Team != target.Team;
            case Const.SkillTargetGroup.Self:
                return actor == target;
            case Const.SkillTargetGroup.All:
                return true;
            default:
                return false;
        }
    }

    private BattleActionResult.EffectOnTarget GetEffectResultOnTarget(BattleCharacter actor, BattleCharacter target, SkillEffect effect, string effectPrefabPath)
    {
        var skillService = ServiceFactory.GetSkillService();

        // TODO: more effect types
        var effectOnTarget = new BattleActionResult.EffectOnTarget();
        effectOnTarget.target = target;

        var hpChange = 0d;

        var shouldHit = skillService.ShouldHit(actor, target, effect);
        if (shouldHit)
        {
            effectOnTarget.isSuccess = true;
            if (effect.HasStatusEffect)
            {
                var statusEffectService = ServiceFactory.GetStatusEffectService();
                effectOnTarget.statusEffectResult = statusEffectService.GetStatusEffectResult(target, effect.StatusEffects);
            }

            if (effect.HasDamageEffect)
            {
                var shouldCritical = skillService.ShouldCritical(actor, target, effect);
                effectOnTarget.isCritical = shouldCritical;
                effectOnTarget.hasDamageEffect = true;

                var damage = Math.Floor(skillService.CalculateDamage(actor, target, effect, shouldCritical));
                hpChange = -damage;
            }                

            effectOnTarget.isEmptyEffect = effect.IsEmptyEffect;
        }
        else
        {
            effectOnTarget.isSuccess = false;
        }

        effectOnTarget.hpChange = hpChange;
        effectOnTarget.skillEffect = effect;
        effectOnTarget.effectPrefabPath = effectPrefabPath;

        return effectOnTarget;
    }
}
