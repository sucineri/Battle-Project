﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BattleService
{

	public List<BattleCharacter> GetAffectdCharacters(Dictionary<BattleCharacter, MapPosition> characters, List<MapPosition> affectedPositions)
	{
		List<BattleCharacter> affectedCharacters = new List<BattleCharacter> ();
		foreach (var position in affectedPositions) {
			var characterAtPosition = this.GetCharacterAtPosition (characters, position);
			if (characterAtPosition != null && !characterAtPosition.IsDead) {
				affectedCharacters.Add (characterAtPosition);
			}
		}
		return affectedCharacters;
	}

	public BattleCharacter GetCharacterAtPosition(Dictionary<BattleCharacter, MapPosition> characters, MapPosition targetPosition)
	{
		foreach (var kv in characters) {
			if (kv.Value.Equals (targetPosition)) {
				return kv.Key;
			}
		}
		return null;
	}

	//TODO : i don't like this mapsize thing
	public Queue<BattleActionOutcome> ProcessActionQueue(Queue<BattleAction> actionQueue, Dictionary<MapPosition, Tile> map, Dictionary<BattleCharacter, MapPosition> characters, Vector2 mapSize)
	{
		var outcomeQueue = new Queue<BattleActionOutcome> ();
		while (actionQueue.Count > 0) {
			var action = actionQueue.Dequeue ();
			var outcome = this.ProcessAction (action, map, characters, mapSize);
			if (outcome != null) {
				outcomeQueue.Enqueue (outcome);
			}
		}
		return outcomeQueue;
	}

	private BattleActionOutcome ProcessAction(BattleAction action, Dictionary<MapPosition, Tile> map, Dictionary<BattleCharacter, MapPosition> characters, Vector2 mapSize)
	{
		switch (action.ActionType) 
		{
			case Const.ActionType.Movement:
				return this.ProcessMovementAction (action, map, characters);
			case Const.ActionType.Skill:
				return this.ProcessSkillAction (action, map, characters, mapSize);
			default:
				return null;
		}
	}

	private BattleActionOutcome ProcessMovementAction(BattleAction action, Dictionary<MapPosition, Tile> map, Dictionary<BattleCharacter, MapPosition> characters)
	{
		var actor = action.Actor;
		var moveTo = action.TargetPosition;

		Debug.LogWarning(actor.Name + " moves to " + moveTo.ToString());

		// update character position
		characters[actor] = moveTo;

		var outcome = new BattleActionOutcome ();
		outcome.type = Const.ActionType.Movement;
		outcome.targetPosition = moveTo;
		outcome.targeteCharacter = actor;

		var movementOutcome = new BattleActionOutcome.OutcomePerTarget ();
		movementOutcome.target = actor;
		movementOutcome.positionChangeTo = moveTo;

		outcome.actorOutcome = movementOutcome;

		return outcome;
	}

	// TODO: i don't like this map size thing
	private BattleActionOutcome ProcessSkillAction(BattleAction action, Dictionary<MapPosition, Tile> map, Dictionary<BattleCharacter, MapPosition> characters, Vector2 mapSize)
	{
		Debug.LogWarning(action.Actor.Name + " uses " + action.SelectedSkill.Name);

		var actor = action.Actor;
		var skill = action.SelectedSkill;
		var affectedPositions = ServiceFactory.GetMapService ().GeAffectedMapPositions (skill.SkillTarget.Pattern, map, action.TargetPosition, mapSize);
		var affectedCharacters = this.GetAffectdCharacters (characters, affectedPositions);

		var outcome = new BattleActionOutcome ();
		outcome.type = Const.ActionType.Skill;
		outcome.targetPosition = action.TargetPosition;
		outcome.targeteCharacter = this.GetCharacterAtPosition (characters, action.TargetPosition);
		outcome.actor = actor;

		for (int i = 0; i < skill.NumberOfTriggers; ++i) {
			var triggerOutcome = new BattleActionOutcome.OutcomePerTrigger ();
			foreach (var affectedCharacter in affectedCharacters) {
				var targetOutcome = this.ApplyEffects (actor, affectedCharacter, skill);
				triggerOutcome.AddTargetOutcome (targetOutcome);
			}
			outcome.AddTriggerOutcome (triggerOutcome);
		}

		ServiceFactory.GetTurnOrderService().AddActionTurnOrderWeight (action);

		return outcome;
	}

	private BattleActionOutcome.OutcomePerTarget ApplyEffects(BattleCharacter actor, BattleCharacter target, Skill skill)
	{
		// TODO: more effect types
		var effects = skill.Effects;
		var outcome = new BattleActionOutcome.OutcomePerTarget();
		outcome.target = target;
		foreach (var effect in effects) {
			var damage = Math.Floor(DamageLogic.GetNormalAttackDamage (actor, target, effect));
			outcome.hpChange -= damage;
		}
		outcome.effectPrefabPath = skill.EffectPrefabPath;

		// Deduct Hp
		target.CurrentHp = Math.Min (target.MaxHp, Math.Max (0d, target.CurrentHp + outcome.hpChange));

		Debug.LogWarning (target.Name + " takes " + outcome.hpChange + " damage");
		Debug.LogWarning (target.Name + " remaining hp " + target.CurrentHp);

		return outcome;
	}
	

}