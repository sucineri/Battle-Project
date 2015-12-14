using UnityEngine;
using System.Collections;

public class BattleUnitsView : MonoBehaviour {

	public IEnumerator MoveUnitToTile(UnitControllerBase unit, TileController targetTile)
	{
		// TODO: move the move animation logic here maybe?
		yield return StartCoroutine(unit.MoveToTile(targetTile));
	}

	public void SpawnUnitOnTile(UnitControllerBase unitController, TileController tile)
	{
		if (tile != null)
		{
			var position = tile.transform.position;
			unitController.transform.position = position;
			if (unitController.Team == Const.Team.Enemy)
			{
				unitController.transform.Rotate(new Vector3(0f, 180f, 0f));
			}
			unitController.transform.SetParent(this.transform);

			tile.AssignUnit(unitController);

			unitController.gameObject.name = unitController.UnitName;
		}
	}
}
