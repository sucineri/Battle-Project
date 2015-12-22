using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TurnOrderService 
{
	public List<BattleCharacter> GetActionOrder(List<BattleCharacter> allUnits)
    {
        allUnits = allUnits.FindAll( x => !x.IsDead );

		this.OrderByWeight (allUnits);

		return allUnits;
    }

	public void AssignDefaultTurnOrderWeight(BattleCharacter character)
	{
		character.TurnOrderWeight = 1.0d / character.BaseCharacter.Agility;
	}

	public void AddActionTurnOrderWeight(BattleAction action)
	{
		// TODO: differnet weight for differnt actions
		if (action.ActionType == Const.ActionType.Skill) {
			var character = action.Actor;
			character.TurnOrderWeight += 1.0d / character.BaseCharacter.Agility;
		}
	}

	private void OrderByWeight(List<BattleCharacter> allUnits)
    {
		if (allUnits != null) {
			allUnits.Sort( (a, b) => {
				return a.TurnOrderWeight.CompareTo(b.TurnOrderWeight);  
			});
		}
    }

}
