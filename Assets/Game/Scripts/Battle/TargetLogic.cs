using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetLogic 
{
    public static MapTile GetTargetTile (UnitController actor, List<UnitController> list)
    {
        var validList = list.FindAll( x => !x.IsDead && x.Team != actor.Team );

        // TODO: more logics
        if(validList.Count > 0)
        {
            return RowFirstTargetLogic(actor, validList);
        }
        return null; 
    }
        
    public static MapTile ColumnFirstTargetLogic (UnitController actor, List<UnitController> targetList)
    {
        targetList.Sort( (a, b) => {
            var result = a.CurrentTile.Column.CompareTo(b.CurrentTile.Column);
            return result == 0 ? (RowDistance(actor, a).CompareTo(RowDistance(actor, b))) : result; 
        });            
        return targetList[0].CurrentTile;
    }

    public static MapTile RowFirstTargetLogic (UnitController actor, List<UnitController> targetList)
    {
        targetList.Sort( (a, b) => {
            var result = (RowDistance(actor, a).CompareTo(RowDistance(actor, b)));
            return result == 0 ? a.CurrentTile.Column.CompareTo(b.CurrentTile.Column) : result;
        });
        return targetList[0].CurrentTile;
    }

    private static int RowDistance (UnitController a, UnitController b)
    {
        return Mathf.Abs(a.CurrentTile.Row - b.CurrentTile.Row);
    }
}
