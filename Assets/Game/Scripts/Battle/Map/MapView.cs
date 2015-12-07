using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapView : MonoBehaviour {

	[SerializeField] private Grid _playerGrid;
	[SerializeField] private Grid _enemyGrid;

	private Action<MapTile> _onTileClick;

	public Dictionary<string, MapTile> InitTiles(Action<MapTile> onTileClick)
	{
		Dictionary<string, MapTile> allTiles = new Dictionary<string, MapTile> ();
		var playerTiles = _playerGrid.InitTiles(onTileClick);
		var enemyTiles = _enemyGrid.InitTiles(onTileClick);

		foreach (var kv in playerTiles) {
			allTiles.Add (kv.Key, kv.Value);
		}

		foreach (var kv in enemyTiles) {
			allTiles.Add (kv.Key, kv.Value);
		}
		return allTiles;
	}
		
	public void SetTilesAffected(List<MapTile> tiles, bool affected)
	{
		// TODO: show affected color
		foreach (var tile in tiles) {
			tile.SetSelected (affected);
		}
	}
}
