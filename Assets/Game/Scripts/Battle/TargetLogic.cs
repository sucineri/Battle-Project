using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetLogic 
{
    public static MapTile GetTargetTile (UnitControllerBase actor, List<UnitControllerBase> list)
    {
        var validList = list.FindAll( x => !x.IsDead && x.Team != actor.Team );

        // TODO: more logics
        if(validList.Count > 0)
        {
            return RowFirstTargetLogic(actor, validList);
        }
        return null; 
    }
        
    public static MapTile ColumnFirstTargetLogic (UnitControllerBase actor, List<UnitControllerBase> targetList)
    {
        targetList.Sort( (a, b) => {
            var result = a.CurrentTile.Column.CompareTo(b.CurrentTile.Column);
            return result == 0 ? (RowDistance(actor, a).CompareTo(RowDistance(actor, b))) : result; 
        });            
        return targetList[0].CurrentTile;
    }

    public static MapTile RowFirstTargetLogic (UnitControllerBase actor, List<UnitControllerBase> targetList)
    {
        targetList.Sort( (a, b) => {
            var result = (RowDistance(actor, a).CompareTo(RowDistance(actor, b)));
            return result == 0 ? a.CurrentTile.Column.CompareTo(b.CurrentTile.Column) : result;
        });
        return targetList[0].CurrentTile;
    }

    private static int RowDistance (UnitControllerBase a, UnitControllerBase b)
    {
        return Mathf.Abs(a.CurrentTile.Row - b.CurrentTile.Row);
    }
}
