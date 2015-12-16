using UnityEngine;
using System.Collections;

public class BattleActionWeight  
{
    public static double GetDefaultTurnOrderWeight(BattleUnitController unit)
    {
        return 1.0d / unit.Character.Agility;
    }

	public static double GetAttackActionWeight(BattleUnitController unit)
    {
        return 1.0d / unit.Character.Agility;
    }

	public static double GetMoveActionWeight(BattleUnitController unit)
    {
        return 0.5d / unit.Character.Agility;
    }

}
