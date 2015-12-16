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

//	public static UnitControllerBase CreateBattleUnit(Const.Team team, CharacterStats character)
//	{
//		var prefab = Resources.Load (character.ModelPath) as GameObject;
//		var go = GameObject.Instantiate (prefab) as GameObject;
//		var unitController = go.GetComponent<UnitControllerBase> ();
//
//		var postfix = GetUnitPostfix (character.Name);
//		unitController.Init (team, character, postfix);
//		unitController.gameObject.name = unitController.UnitName;
//		return unitController;
//	}
//
//	private static char GetUnitPostfix(string characterName)
//	{
//		if(_unitNameDictionary.ContainsKey(characterName))
//		{
//			if(_unitNameDictionary[characterName] == 'Z')
//			{
//				_unitNameDictionary[characterName] = 'A';
//			}
//			else
//			{
//				_unitNameDictionary[characterName] ++;
//			}
//		}
//		else
//		{
//			_unitNameDictionary.Add(characterName, 'A');
//		}
//		return _unitNameDictionary[characterName];
//	}
}
