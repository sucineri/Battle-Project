using UnityEngine;
using System.Collections;

public class BattleActionWeight  
{
    public static float GetDefaultTurnOrderWeight(UnitControllerBase unit)
    {
        return 1f /unit.Character.Agility;
    }

    public static float GetAttackActionWeight(UnitControllerBase unit)
    {
        return 1f / unit.Character.Agility;
    }

    public static float GetMoveActionWeight(UnitControllerBase unit)
    {
        return 0.5f / unit.Character.Agility;
    }

}
