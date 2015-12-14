using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapView : MonoBehaviour {

//	[SerializeField] private Grid _playerGrid;
//	[SerializeField] private Grid _enemyGrid;
//
	[SerializeField] private GameObject tilePrefab;
	[SerializeField] private GridLayoutGroup _playerGrid;
	[SerializeField] private GridLayoutGroup _enemyGrid;

	private Dictionary<MapPosition, TileController> _tiles = new Dictionary<MapPosition, TileController>();

	public void InitGrids(int numberOfRows, int numberOfColumns, Action<MapPosition> onTileClick)
	{
		this.InitGrid (this._playerGrid, numberOfRows, numberOfColumns, Const.Team.Player, onTileClick);
		this.InitGrid (this._enemyGrid, numberOfRows, numberOfColumns, Const.Team.Enemy, onTileClick);
	}

	public void AssignTile(MapPosition mapPosition, Tile tile)
	{
		var view = _tiles [mapPosition];
		tile.onStateChange += view.OnTileStateChange;
	}

	private void InitGrid(GridLayoutGroup grid, int numberOfRows, int numberOfColumns, Const.Team team, Action<MapPosition> onTileClick)
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

				var tielView = tile.GetComponent<TileController>();
				tielView.Init (mapPosition, onTileClick);

				this._tiles.Add (mapPosition, tielView);
			}
		}
	}

//	public Dictionary<string, MapTile> InitTiles (System.Action<MapTile> onTileClick)
//	{
//		Dictionary<string, MapTile> dict = new Dictionary<string, MapTile> ();
//		layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
//		layout.constraintCount = numberOfColumns;
//		for (int i = 0; i < this.numberOfRows; i++) {
//			for (int j = 0; j < this.numberOfColumns; j++) {
//				var tile = Instantiate (tilePrefab) as GameObject;
//				tile.transform.SetParent (layout.transform);
//				tile.transform.localScale = Vector3.one;
//				tile.transform.localEulerAngles = Vector3.zero;
//				tile.transform.localPosition = Vector3.zero;
//
//				var mapTile = tile.GetComponent<MapTile> ();
//				mapTile.Init (j, i, team, onTileClick);
//				tile.name = Const.GetTileKey (team, j, i);
//				dict.Add (tile.name, mapTile);
//			}
//		}
//		return dict;
//	}

//	public Dictionary<string, MapTile> InitTiles(Action<MapTile> onTileClick)
//	{
//		Dictionary<string, MapTile> allTiles = new Dictionary<string, MapTile> ();
//		var playerTiles = _playerGrid.InitTiles(onTileClick);
//		var enemyTiles = _enemyGrid.InitTiles(onTileClick);
//
//		foreach (var kv in playerTiles) {
//			allTiles.Add (kv.Key, kv.Value);
//		}
//
//		foreach (var kv in enemyTiles) {
//			allTiles.Add (kv.Key, kv.Value);
//		}
//		return allTiles;
//	}
		
	public void SetTilesAffected(List<TileController> tiles, bool affected)
	{
		// TODO: show affected color
		foreach (var tile in tiles) {
			tile.SetSelected (affected);
		}
	}
}
