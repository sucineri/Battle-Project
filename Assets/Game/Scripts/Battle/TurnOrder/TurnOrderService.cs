using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TurnOrderService 
{
	public List<UnitControllerBase> GetActionOrder(List<UnitControllerBase> allUnits)
    {
        allUnits = allUnits.FindAll( x => !x.IsDead );

		this.OrderByWeight (allUnits);

		return allUnits;
    }

	private void OrderByWeight(List<UnitControllerBase> allUnits)
    {
		if (allUnits != null) {
			allUnits.Sort( (a, b) => {
				return a.TurnOrderWeight.CompareTo(b.TurnOrderWeight);  
			});
		}
    }
}
