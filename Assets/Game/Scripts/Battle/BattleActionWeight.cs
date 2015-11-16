using UnityEngine;
using System.Collections;

public class BattleActionWeight  
{
    public static double GetDefaultTurnOrderWeight(UnitControllerBase unit)
    {
        return 1.0d / unit.Character.Agility;
    }

	public static double GetAttackActionWeight(UnitControllerBase unit)
    {
        return 1.0d / unit.Character.Agility;
    }

	public static double GetMoveActionWeight(UnitControllerBase unit)
    {
        return 0.5d / unit.Character.Agility;
    }

}
