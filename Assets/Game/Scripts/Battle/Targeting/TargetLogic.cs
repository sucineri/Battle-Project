using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetLogic 
{
    public static TileController GetTargetTile (BattleUnitController actor, List<BattleUnitController> list)
    {
        var validList = list.FindAll( x => !x.IsDead && x.Team != actor.Team );

        // TODO: more logics
        if(validList.Count > 0)
        {
            return DefaultTargetLogic(actor, validList);
        }
        return null; 
    }

    public static TileController DefaultTargetLogic (BattleUnitController actor, List<BattleUnitController> targetList)
    {
        targetList.Sort( (a, b) => {
            var result = (YDistance(actor, a).CompareTo(YDistance(actor, b)));
            return result == 0 ? a.CurrentTile.X.CompareTo(b.CurrentTile.X) : result;
        });
        return targetList[0].CurrentTile;
    }

    private static int YDistance (BattleUnitController a, BattleUnitController b)
    {
        return Mathf.Abs(a.CurrentTile.Y - b.CurrentTile.Y);
    }
}
