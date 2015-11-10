using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TurnOrderService 
{
    private List<UnitController> allUnits = new List<UnitController>();

    private event Action<List<UnitController>> onOrderChanged;

    public void Init(List<UnitController> units, Action<List<UnitController>> onOrderChange)
    {
        this.allUnits = units;
        this.onOrderChanged = onOrderChange;
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

        if(this.onOrderChanged != null)
        {
            this.onOrderChanged(allUnits);
        }
    }
}
