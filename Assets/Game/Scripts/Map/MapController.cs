using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour 
{
    [SerializeField] private Grid _playerGrid;
    [SerializeField] private Grid _enemyGrid;

    public bool EnableTouch { get { return _battleController.EnableInput; } }

    private Dictionary<string, MapTile> _playerTiles = new Dictionary<string, MapTile>();
    private Dictionary<string, MapTile> _enemyTiles = new Dictionary<string, MapTile>();

    private UnitController _selectedPlayer;
    private BattleController _battleController;
    private Action _onTileClick = null;

    public void Init(Action<MapTile> onPlayerTileClick, Action<MapTile> onEnemyTileClick)
    {
        _playerTiles = _playerGrid.Init(onPlayerTileClick);
        _enemyTiles = _enemyGrid.Init(onEnemyTileClick);
    }

    public void Init(BattleController battleController)
    {
        this._battleController = battleController;
        _playerTiles = _playerGrid.Init(this.OnTileClick);
        _enemyTiles = _enemyGrid.Init(this.OnTileClick);
    }

    public MapTile GetTile(Const.Team team, int row, int column)
    {
        if (team == Const.Team.Player)
        {
            return GetTile(_playerTiles, row, column);
        }
        else
        {
            return GetTile(_enemyTiles, row, column);
        }
    }

    public IEnumerator WaitForUserInput(UnitController currentActor)
    {
        var waitForInput = true;
        this._onTileClick = () => { waitForInput = false; };
        this._selectedPlayer = currentActor;
        this._selectedPlayer.CurrentTile.SetSelected(true);

        while (waitForInput)
        {
            yield return 0;
        }
        this._onTileClick = null;
    }

    private void OnTileClick(MapTile tileClicked)
    {
        if(!this.EnableTouch) return;
        if(tileClicked.Team == Const.Team.Player)
        {
            StartCoroutine(OnPlayerTileClicked(tileClicked, this._onTileClick));
        }
        else if(tileClicked.CurrentUnit != null)
        {
            StartCoroutine(OnEnemyTileClicked(tileClicked, this._onTileClick));
        }
    }

    private IEnumerator OnPlayerTileClicked(MapTile tileClicked, Action onComplete = null)
    {
        // if a unit has been selectd before
        if(_selectedPlayer != null)
        {
            var tileBefore = _selectedPlayer.CurrentTile;
            var tileAfter = tileClicked;

            // if a different tile is clicked
            if(tileBefore != tileAfter)
            {
                tileBefore.SetSelected(false);

                var unitOnTileClicked = tileAfter.CurrentUnit;

                // if there's another unit on tile clied
                if(unitOnTileClicked != null)
                {
                    StartCoroutine(unitOnTileClicked.MoveToTile(tileBefore));
                }

                yield return StartCoroutine(_selectedPlayer.MoveToTile(tileAfter));

                // swap units assignment
                tileBefore.AssignUnit(unitOnTileClicked);
                tileAfter.AssignUnit(_selectedPlayer);
            }
            else // the same tile is clicked again
            {
                tileClicked.SetSelected(false);
            }

            _selectedPlayer = null;
        }
        else  // no unit was selected before
        {
            // assign selectedPlayer pointer
            _selectedPlayer = tileClicked.CurrentUnit;
            if(_selectedPlayer != null)
            {
                // set the state of the tile to be selected
                tileClicked.SetSelected(true);
            }
        }
        yield return 0;
        if(onComplete != null)
        {
            onComplete();
        }
    }

    private IEnumerator OnEnemyTileClicked(MapTile tileClicked, Action onComplete = null)
    {
        if(_selectedPlayer != null)
        {
            if(tileClicked.CurrentUnit != null)
            {
                yield return StartCoroutine(_selectedPlayer.AttackOpponentOnTile(tileClicked));
                _selectedPlayer.CurrentTile.SetSelected(false);
                _selectedPlayer = null;
            }
        }
        yield return 0;
        if(onComplete != null)
        {
            onComplete();
        }
    }

    private MapTile GetTile(Dictionary<string, MapTile> dict, int row, int column)
    {
        MapTile tile = null;
        dict.TryGetValue(Const.GetTileKey(row, column), out tile);
        return tile;
    }
}
