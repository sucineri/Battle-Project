using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TurnOrderService 
{
    private List<UnitControllerBase> allUnits = new List<UnitControllerBase>();

    private event Action<List<UnitControllerBase>> onOrderChanged;

    public void Init(List<UnitControllerBase> units, Action<List<UnitControllerBase>> onOrderChange)
    {
        this.allUnits = units;
        this.onOrderChanged = onOrderChange;
    }

    public UnitControllerBase GetNextActor()
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
