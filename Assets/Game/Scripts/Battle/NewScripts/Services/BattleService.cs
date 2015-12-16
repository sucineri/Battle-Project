using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BattleService
{
	public Queue<BattleActionOutcome> ProcessActionQueue(Queue<BattleAction> actionQueue, Dictionary<MapPosition, Tile> map, Dictionary<BattleCharacter, MapPosition> characters)
	{
		var outcomeQueue = new Queue<BattleActionOutcome> ();
		while (actionQueue.Count > 0) {
			var action = actionQueue.Dequeue ();
			var outcome = this.ProcessAction (action, map, characters);
			if (outcome != null) {
				outcomeQueue.Enqueue (outcome);
			}
		}
		return outcomeQueue;
	}

	private BattleActionOutcome ProcessAction(BattleAction action, Dictionary<MapPosition, Tile> map, Dictionary<BattleCharacter, MapPosition> characters)
	{
		switch (action.ActionType) 
		{
			case Const.ActionType.Movement:
				return this.ProcessMovementAction (action, map, characters);
			case Const.ActionType.Skill:
				return this.ProcessSkillAction (action, map, characters);
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
		outcome.Type = Const.ActionType.Movement;

		var movementOutcome = new BattleActionOutcome.OutcomePerTarget ();
		movementOutcome.Target = actor;
		movementOutcome.PositionChangeTo = moveTo;

		outcome.ActorOutcome = movementOutcome;

		return outcome;
	}

	private BattleActionOutcome ProcessSkillAction(BattleAction action, Dictionary<MapPosition, Tile> map, Dictionary<BattleCharacter, MapPosition> characters)
	{
		Debug.LogWarning(action.Actor.Name + " uses " + action.SelectedSkill.Name);

		var actor = action.Actor;
		var skill = action.SelectedSkill;
		var affectedPositions = ServiceFactory.GetMapService ().GeAffectedMapPositions (skill.SkillTarget.Pattern, map, action.TargetPosition);
		var affectedCharacters = this.GetAffectdCharacters (characters, affectedPositions);

		var outcome = new BattleActionOutcome ();
		outcome.Type = Const.ActionType.Skill;

		for (int i = 0; i < skill.NumberOfTriggers; ++i) {
			var triggerOutcome = new BattleActionOutcome.OutcomePerTrigger ();
			foreach (var affectedCharacter in affectedCharacters) {
				var targetOutcome = this.ApplyEffects (actor, affectedCharacter, skill.Effects);
				triggerOutcome.AddTargetOutcome (targetOutcome);
			}
			outcome.AddTriggerOutcome (triggerOutcome);
		}

		ServiceFactory.GetTurnOrderService().AddActionTurnOrderWeight (action);

		return outcome;
	}

	private List<BattleCharacter> GetAffectdCharacters(Dictionary<BattleCharacter, MapPosition> characters, List<MapPosition> affectedPositions)
	{
		List<BattleCharacter> affectedCharacters = new List<BattleCharacter> ();
		foreach (var position in affectedPositions) {
			foreach (var kv in characters) {
				if (kv.Value.Equals (position)) {
					affectedCharacters.Add (kv.Key);
				}
			}
		}
		return affectedCharacters;
	}

	private BattleActionOutcome.OutcomePerTarget ApplyEffects(BattleCharacter actor, BattleCharacter target, List<SkillEffect> effects)
	{
		// TODO: more effect types
		var outcome = new BattleActionOutcome.OutcomePerTarget();
		outcome.Target = target;
		foreach (var effect in effects) {
			var damage = Math.Floor(DamageLogic.GetNormalAttackDamage (actor, target, effect));
			outcome.HpChange -= damage;
		}

		// Deduct Hp
		target.CurrentHp = Math.Min (target.MaxHp, Math.Max (0d, target.CurrentHp + outcome.HpChange));

		Debug.LogWarning (target.Name + " takes " + outcome.HpChange + " damage");
		Debug.LogWarning (target.Name + " remaining hp " + target.CurrentHp);

		return outcome;
	}
	

}
