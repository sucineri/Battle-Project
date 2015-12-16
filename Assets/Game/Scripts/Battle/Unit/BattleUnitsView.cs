using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleUnitsView : MonoBehaviour {

	private Dictionary<BattleCharacter, BattleUnitController> _allUnits = new Dictionary<BattleCharacter, BattleUnitController>();

	public IEnumerator MoveUnitToTile(BattleCharacter character, TileController targetTile)
	{
		// TODO: move the move animation logic here maybe?
		var unitController = this._allUnits[character];
		if (unitController != null) {
			yield return StartCoroutine(unitController.MoveToTile(targetTile));
		}
	}

	public void SpawnUnitOnTile(BattleCharacter character, TileController tile)
	{
		var position = tile.transform.position;
		var battleUnit = BattleUnitFactory.CreateBattleUnit (character);
		battleUnit.transform.position = position;

		if (character.Team == Const.Team.Enemy) {
			battleUnit.transform.Rotate (new Vector3 (0f, 180f, 0f));
		}

		battleUnit.transform.SetParent(this.transform);
		this._allUnits.Add (character, battleUnit);
	}
}
