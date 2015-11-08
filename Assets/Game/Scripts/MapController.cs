using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour 
{
    [SerializeField] private Grid playerGrid;
    [SerializeField] private Grid enemyGrid;

    private Dictionary<string, MapTile> playerTiles = new Dictionary<string, MapTile>();
    private Dictionary<string, MapTile> enemyTiles = new Dictionary<string, MapTile>();

    public void Init(Action<MapTile> onPlayerTileClick, Action<MapTile> onEnemyTileClick)
    {
        playerTiles = playerGrid.Init(onPlayerTileClick);
        enemyTiles = enemyGrid.Init(onEnemyTileClick);
    }

    public MapTile GetTile(Const.Team team, int row, int column)
    {
        if (team == Const.Team.Player)
        {
            return GetTile(playerTiles, row, column);
        }
        else
        {
            return GetTile(enemyTiles, row, column);
        }
    }

    private MapTile GetTile(Dictionary<string, MapTile> dict, int row, int column)
    {
        MapTile tile = null;
        dict.TryGetValue(Const.GetTileKey(row, column), out tile);
        return tile;
    }
}
