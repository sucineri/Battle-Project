using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BattleView : MonoBehaviour {

	[SerializeField] private MapView _mapView;

	private Dictionary<BattleCharacter, BattleUnitController> _battleUnits = new Dictionary<BattleCharacter, BattleUnitController>();
	private Dictionary<MapPosition, TileController> _mapTiles = new Dictionary<MapPosition, TileController>();

	public void InitMap(int numberOfRows, int numberOfColumns, Action<MapPosition> onTileClick)
	{
		foreach (var kv in this._mapView.InitGrids(numberOfRows, numberOfColumns, onTileClick)) {
			this._mapTiles.Add (kv.Key, kv.Value);
		}
	}

	public IEnumerator MoveUnitToTile(BattleCharacter character, TileController targetTile)
	{
		var unitController = this._battleUnits[character];
		if (unitController != null) {
			yield return StartCoroutine(unitController.MoveToTile(targetTile));
		}
	}

	public BattleUnitController GetBattleUnit(BattleCharacter character)
	{
		return this._battleUnits[character];
	}

	public TileController GetTileAtMapPosition(MapPosition mapPosition)
	{
		return this._mapTiles [mapPosition];
	}

	public void BindTileController(MapPosition mapPosition, Tile tile)
	{
		var controller = this._mapTiles [mapPosition];
		tile.onStateChange += controller.OnTileStateChange;
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
		battleUnit.Init (character);
		this._battleUnits.Add (character, battleUnit);
	}

	public IEnumerator PlaySkillAnimation(Skill skill, BattleActionOutcome outcome)
	{
		var skillController = SkillControllerFactory.CreateSkillController (skill);
		if (skillController != null) {
			var actor = outcome.actor;
			var actorController = this.GetBattleUnit (actor);
			yield return StartCoroutine (skillController.PlaySkillSequence (actorController, this, outcome));
		}
		yield return null;
	}
}
