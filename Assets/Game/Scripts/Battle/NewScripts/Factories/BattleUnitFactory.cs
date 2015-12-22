using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleUnitFactory 
{
	private static Dictionary<string, char> _unitNameDictionary = new Dictionary<string, char>();

	public static BattleUnitController CreateBattleUnit(BattleCharacter character)
	{
		var prefab = Resources.Load (character.BaseCharacter.ModelPath) as GameObject;
		var go = GameObject.Instantiate (prefab) as GameObject;
		var unitController = go.GetComponent<BattleUnitController> ();
		return unitController;
	}
}
