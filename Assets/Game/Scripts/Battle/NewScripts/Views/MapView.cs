using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapView : MonoBehaviour {

	[SerializeField] private GameObject tilePrefab;
	[SerializeField] private GridLayoutGroup _playerGrid;
	[SerializeField] private GridLayoutGroup _enemyGrid;

	public IEnumerable<KeyValuePair<MapPosition, TileController>> InitGrids(int numberOfRows, int numberOfColumns, Action<MapPosition> onTileClick)
	{
		foreach (var kv in this.InitGrid (this._playerGrid, numberOfRows, numberOfColumns, Const.Team.Player, onTileClick)) {
			yield return kv;
		}
		foreach (var kv in this.InitGrid (this._enemyGrid, numberOfRows, numberOfColumns, Const.Team.Enemy, onTileClick)) {
			yield return kv;
		}
	}

	private IEnumerable<KeyValuePair<MapPosition, TileController>> InitGrid(GridLayoutGroup grid, int numberOfRows, int numberOfColumns, Const.Team team, Action<MapPosition> onTileClick)
	{
		grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		grid.constraintCount = numberOfColumns;
		for (int i = 0; i < numberOfRows; i++) {
			for (int j = 0; j < numberOfColumns; j++) {
				var mapPosition = new MapPosition(j, i, team);

				var tile = Instantiate (tilePrefab) as GameObject;
				tile.transform.SetParent (grid.transform);
				tile.transform.localScale = Vector3.one;
				tile.transform.localEulerAngles = Vector3.zero;
				tile.transform.localPosition = Vector3.zero;

				var tileController = tile.GetComponent<TileController>();
				tileController.Init (mapPosition, onTileClick);

				yield return new KeyValuePair<MapPosition, TileController> (mapPosition, tileController);
			}
		}
	}
}
