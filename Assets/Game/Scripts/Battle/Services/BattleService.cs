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
        foreach (var character in characters)
        {
            if (character.OccupiedMapPositions.Contains(targetPosition))
            {
                return character;
            }
        }
        return null;
    }
        
    public Queue<BattleActionResult> ProcessActionQueue(Queue<BattleAction> actionQueue, Dictionary<MapPosition, Tile> map, List<BattleCharacter> characters)
    {
        var resultQueue = new Queue<BattleActionResult>();
        while (actionQueue.Count > 0)
        {
            var action = actionQueue.Dequeue();
            var actionResult = this.ProcessAction(action, map, characters);
            if (actionResult != null)
            {
                resultQueue.Enqueue(actionResult);
            }
        }
        return resultQueue;
    }

    private BattleActionResult ProcessAction(BattleAction action, Dictionary<MapPosition, Tile> map, List<BattleCharacter> characters)
    {
        switch (action.ActionType)
        {
            case Const.ActionType.Movement:
                return this.ProcessMovementAction(action, map, characters);
            case Const.ActionType.Skill:
                return this.ProcessSkillAction(action, map, characters);
            default:
                return null;
        }
    }

    private BattleActionResult ProcessMovementAction(BattleAction action, Dictionary<MapPosition, Tile> map, List<BattleCharacter> characters)
    {
        var actor = action.Actor;
        var moveTo = action.TargetPosition;

        Debug.LogWarning(actor.Name + " moves to " + moveTo.ToString());

        var mapService = ServiceFactory.GetMapService();
        var unOccupiedPositions = mapService.GetUnoccupiedTiles(characters, map);
        var newOccupiedPositions = mapService.RequestPositions(actor.BaseCharacter.PatternShape.Shape, map, moveTo, unOccupiedPositions);

        // update character position
        actor.OccupiedMapPositions = newOccupiedPositions;

        var actionResult = new BattleActionResult();
        actionResult.type = Const.ActionType.Movement;
        actionResult.targetPosition = moveTo;
        actionResult.targetCharacter = actor;
        actionResult.actor = actor;

        var movementEffect = new BattleActionResult.EffectOnTarget();
        movementEffect.target = actor;
        movementEffect.positionChangeTo = moveTo;

        var effectResult = new BattleActionResult.ActionEffectResult();
        effectResult.AddEffectOnTarget(movementEffect);

        actionResult.AddSkillEffectResult(effectResult);

        return actionResult;
    }
        
    private BattleActionResult ProcessSkillAction(BattleAction action, Dictionary<MapPosition, Tile> map, List<BattleCharacter> allCharacters)
    {
        Debug.LogWarning(action.Actor.Name + " uses " + action.SelectedSkill.Name);

        var actor = action.Actor;
        var skill = action.SelectedSkill;

        var skillActionResult = new BattleActionResult();
        skillActionResult.type = Const.ActionType.Skill;
        skillActionResult.targetPosition = action.TargetPosition;
        skillActionResult.targetCharacter = this.GetCharacterAtPosition(allCharacters, action.TargetPosition);
        skillActionResult.actor = actor;
        skillActionResult.skill = skill;

        // For keeping track of affect characters to apply enmity
        HashSet<BattleCharacter> uniqueAffectedOpponents = new HashSet<BattleCharacter>();

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
                var resultOnTarget = this.ApplyEffects(actor, affectedCharacter, skill);
                actionEffectResult.AddEffectOnTarget(resultOnTarget);
            }
            skillActionResult.AddSkillEffectResult(actionEffectResult);

            foreach (var character in affectedCharacters)
            {
                if (character.Team != actor.Team)
                {
                    uniqueAffectedOpponents.Add(character);
                }
            }
        }
            
        ServiceFactory.GetTurnOrderService().ApplySkillCooldownToCharacter(actor, skill);

        ServiceFactory.GetEnmityService().ApplyEnmityForSkill(actor, skill, uniqueAffectedOpponents.ToList(), allCharacters);

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
            var affectedPositions = ServiceFactory.GetMapService().GeMapPositionsForPattern(targeting.Pattern, map, prevTargetPosition);
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
            default:
                return false;
        }
    }

    private BattleActionResult.EffectOnTarget ApplyEffects(BattleCharacter actor, BattleCharacter target, Skill skill)
    {
        // TODO: more effect types
        var effects = skill.Effects;
        var outcome = new BattleActionResult.EffectOnTarget();
        outcome.target = target;
        foreach (var effect in effects)
        {
            var damage = Math.Floor(DamageLogic.GetNormalAttackDamage(actor, target, effect));
            outcome.hpChange -= damage;
        }
        outcome.effectPrefabPath = skill.EffectPrefabPath;

        // Deduct Hp
        target.CurrentHp = Math.Min(target.MaxHp, Math.Max(0d, target.CurrentHp + outcome.hpChange));

        Debug.LogWarning(target.Name + " takes " + outcome.hpChange + " damage");
        Debug.LogWarning(target.Name + " remaining hp " + target.CurrentHp);

        return outcome;
    }
}
