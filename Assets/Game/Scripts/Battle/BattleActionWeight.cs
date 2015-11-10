using UnityEngine;
using System.Collections;

public class BattleActionWeight  
{
    public static float GetDefaultTurnOrderWeight(UnitController unit)
    {
        return 1f /unit.Character.Agility;
    }

    public static float GetAttackActionWeight(UnitController unit)
    {
        return 1f / unit.Character.Agility;
    }

    public static float GetMoveActionWeight(UnitController unit)
    {
        return 0.5f / unit.Character.Agility;
    }

}
