using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BattleService
{
	public BattleActionOutcome ProcessSkillAction(BattleAction action, Dictionary<MapPosition, Tile> map, Dictionary<BattleCharacter, MapPosition> characters)
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
