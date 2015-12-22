using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleUnitFactory
{
    public static BattleUnitController CreateBattleUnit(BattleCharacter character)
    {
        var prefab = Resources.Load(character.BaseCharacter.ModelPath) as GameObject;
        var go = GameObject.Instantiate(prefab) as GameObject;
        var unitController = go.GetComponent<BattleUnitController>();
        return unitController;
    }
}
