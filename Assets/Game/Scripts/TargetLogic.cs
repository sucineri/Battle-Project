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
            var result = (ColumnDistance(actor, a).CompareTo(ColumnDistance(actor, b)));
            if(result == 0)
            {
                result = (RowDistance(actor, a).CompareTo(RowDistance(actor, b)));
                if(result == 0)
                {
                    return a.CurrentTile.Column.CompareTo(b.CurrentTile.Column);
                }
            }
            return result;
        });
            
        return targetList[0].CurrentTile;
    }

    public static MapTile RowFirstTargetLogic (UnitController actor, List<UnitController> targetList)
    {
        targetList.Sort( (a, b) => {
            var result = (RowDistance(actor, a).CompareTo(RowDistance(actor, b)));
            if(result == 0)
            {
                result = (ColumnDistance(actor, a).CompareTo(ColumnDistance(actor, b)));
                if(result == 0)
                {
                    return a.CurrentTile.Row.CompareTo(b.CurrentTile.Row);
                }
            }
            return result;
        });
        return targetList[0].CurrentTile;
    }

    private static int ColumnDistance (UnitController a, UnitController b)
    {
        return Mathf.Abs(a.CurrentTile.Column - b.CurrentTile.Column);
    }

    private static int RowDistance (UnitController a, UnitController b)
    {
        return Mathf.Abs(a.CurrentTile.Row - b.CurrentTile.Row);
    }
}
