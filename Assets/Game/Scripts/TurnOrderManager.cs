using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnOrderManager 
{
    private List<UnitController> allUnits = new List<UnitController>();

    public void Init(List<UnitController> units)
    {
        this.allUnits = units;
    }

    public UnitController GetNextActor()
    {
        allUnits = allUnits.FindAll( x => !x.IsDead );
        if(allUnits.Count > 0)
        {
            this.OrderByWeight();
            return this.allUnits[0];
        }
        return null;
    }

    private void OrderByWeight()
    {
        allUnits.Sort( (a, b) => {
            return a.TurnOrderWeight.CompareTo(b.TurnOrderWeight);  
        });
    }
}
