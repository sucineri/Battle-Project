using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleService
{

    public List<BattleCharacter> GetAffectdCharacters(List<BattleCharacter> characters, List<MapPosition> affectedPositions)
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
        
    public Queue<BattleActionOutcome> ProcessActionQueue(Queue<BattleAction> actionQueue, Dictionary<MapPosition, Tile> map, List<BattleCharacter> characters)
    {
        var outcomeQueue = new Queue<BattleActionOutcome>();
        while (actionQueue.Count > 0)
        {
            var action = actionQueue.Dequeue();
            var outcome = this.ProcessAction(action, map, characters);
            if (outcome != null)
            {
                outcomeQueue.Enqueue(outcome);
            }
        }
        return outcomeQueue;
    }

    private BattleActionOutcome ProcessAction(BattleAction action, Dictionary<MapPosition, Tile> map, List<BattleCharacter> characters)
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

    private BattleActionOutcome ProcessMovementAction(BattleAction action, Dictionary<MapPosition, Tile> map, List<BattleCharacter> characters)
    {
        var actor = action.Actor;
        var moveTo = action.TargetPosition;

        Debug.LogWarning(actor.Name + " moves to " + moveTo.ToString());

        var newOccupiedPositions = ServiceFactory.GetMapService().GeMapPositionsForPattern(actor.BaseCharacter.Shape, map, moveTo);

        // update character position
        actor.OccupiedMapPositions = newOccupiedPositions;

        var outcome = new BattleActionOutcome();
        outcome.type = Const.ActionType.Movement;
        outcome.targetPosition = moveTo;
        outcome.targeteCharacter = actor;

        var movementOutcome = new BattleActionOutcome.OutcomePerTarget();
        movementOutcome.target = actor;
        movementOutcome.positionChangeTo = moveTo;

        outcome.actorOutcome = movementOutcome;

        return outcome;
    }
        
    private BattleActionOutcome ProcessSkillAction(BattleAction action, Dictionary<MapPosition, Tile> map, List<BattleCharacter> characters)
    {
        Debug.LogWarning(action.Actor.Name + " uses " + action.SelectedSkill.Name);

        var actor = action.Actor;
        var skill = action.SelectedSkill;
        var affectedPositions = ServiceFactory.GetMapService().GeMapPositionsForPattern(skill.SkillTarget.Pattern, map, action.TargetPosition);
        var affectedCharacters = this.GetAffectdCharacters(characters, affectedPositions);

        var outcome = new BattleActionOutcome();
        outcome.type = Const.ActionType.Skill;
        outcome.targetPosition = action.TargetPosition;
        outcome.targeteCharacter = this.GetCharacterAtPosition(characters, action.TargetPosition);
        outcome.actor = actor;

        for (int i = 0; i < skill.NumberOfTriggers; ++i)
        {
            var triggerOutcome = new BattleActionOutcome.OutcomePerTrigger();
            foreach (var affectedCharacter in affectedCharacters)
            {
                var targetOutcome = this.ApplyEffects(actor, affectedCharacter, skill);
                triggerOutcome.AddTargetOutcome(targetOutcome);
            }
            outcome.AddTriggerOutcome(triggerOutcome);
        }

        ServiceFactory.GetTurnOrderService().AddActionTurnOrderWeight(action);

        return outcome;
    }

    private BattleActionOutcome.OutcomePerTarget ApplyEffects(BattleCharacter actor, BattleCharacter target, Skill skill)
    {
        // TODO: more effect types
        var effects = skill.Effects;
        var outcome = new BattleActionOutcome.OutcomePerTarget();
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
