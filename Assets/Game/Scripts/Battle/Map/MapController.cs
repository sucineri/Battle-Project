using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    [SerializeField] private Grid _playerGrid;
    [SerializeField] private Grid _enemyGrid;

    private Dictionary<string, MapTile> _playerTiles = new Dictionary<string, MapTile>();
    private Dictionary<string, MapTile> _enemyTiles = new Dictionary<string, MapTile>();

    private UnitControllerBase _selectedPlayer;
    private Action _onTileClick = null;

    public void Init()
    {
        _playerTiles = _playerGrid.Init(this.OnTileClick);
        _enemyTiles = _enemyGrid.Init(this.OnTileClick);
    }

    public MapTile GetTile(Const.Team team, int x, int y)
    {
        if (team == Const.Team.Player)
        {
            return GetTile(_playerTiles, x, y);
        }
        else
        {
            return GetTile(_enemyTiles, x, y);
        }
    }

    public IEnumerator WaitForUserInput(UnitControllerBase currentActor)
    {
        var waitForInput = true;
        this._onTileClick = () =>
        {
            waitForInput = false;
        };
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
        if (!BattleManager.Instance.EnableTileTouch)
        {
            return;
        }
		StartCoroutine (this.OnTileClicked (tileClicked, this._onTileClick));
//
//        if (tileClicked.Team == Const.Team.Player)
//        {
//            StartCoroutine(OnPlayerTileClicked(tileClicked, this._onTileClick));
//        }
//        else if (tileClicked.CurrentUnit != null)
//        {
//            StartCoroutine(OnEnemyTileClicked(tileClicked, this._onTileClick));
//        }
    }

//    private IEnumerator OnPlayerTileClicked(MapTile tileClicked, Action onComplete = null)
//    {
//		this.OnAnimationStateChange (true);
//        // if a unit has been selectd before
//        if (_selectedPlayer != null)
//        {
//            var tileBefore = _selectedPlayer.CurrentTile;
//            var tileAfter = tileClicked;
//
//            // if a different tile is clicked
//            if (tileBefore != tileAfter)
//            {
//                tileBefore.SetSelected(false);
//
//                var unitOnTileClicked = tileAfter.CurrentUnit;
//
//                // if there's another unit on tile clied
//                if (unitOnTileClicked != null)
//                {
//                    StartCoroutine(unitOnTileClicked.MoveToTile(tileBefore));
//                }
//
//                yield return StartCoroutine(_selectedPlayer.MoveToTile(tileAfter));
//
//                // swap units assignment
//                tileBefore.AssignUnit(unitOnTileClicked);
//                tileAfter.AssignUnit(_selectedPlayer);
//            }
//            else
//            { // the same tile is clicked again
//                tileClicked.SetSelected(false);
//            }
//
//            _selectedPlayer = null;
//        }
//        else
//        {  // no unit was selected before
//            // assign selectedPlayer pointer
//            _selectedPlayer = tileClicked.CurrentUnit;
//            if (_selectedPlayer != null)
//            {
//                // set the state of the tile to be selected
//                tileClicked.SetSelected(true);
//            }
//        }
//        yield return 0;
//        if (onComplete != null)
//        {
//            onComplete();
//        }
//		this.OnAnimationStateChange (false);
//    }
//
//    private IEnumerator OnEnemyTileClicked(MapTile tileClicked, Action onComplete = null)
//    {
//		this.OnAnimationStateChange (true);
//        if (_selectedPlayer != null)
//        {
//            if (tileClicked.CurrentUnit != null)
//            {
//                yield return StartCoroutine(_selectedPlayer.GetSelectedSkill().PlaySkillSequence(_selectedPlayer, tileClicked));
//                _selectedPlayer.CurrentTile.SetSelected(false);
//                _selectedPlayer = null;
//            }
//        }
//        yield return 0;
//        if (onComplete != null)
//        {
//            onComplete();
//        }
//		this.OnAnimationStateChange (false);
//    }

	private IEnumerator OnTileClicked(MapTile tileClicked, Action onComplete = null)
	{
		if (BattleManager.Instance.Phrase == BattleManager.BattlePhrase.MovementSelect) {
			if (tileClicked.Team == this._selectedPlayer.Team) {
				BattleManager.Instance.Phrase = BattleManager.BattlePhrase.Animation;
				yield return StartCoroutine(this.MoveUnitToTile(this._selectedPlayer, tileClicked));
			}
		} 
		else if (BattleManager.Instance.Phrase == BattleManager.BattlePhrase.TargetSelect) {
			if (tileClicked.CurrentUnit != null)
			{
				BattleManager.Instance.Phrase = BattleManager.BattlePhrase.Animation;
				yield return StartCoroutine(_selectedPlayer.GetSelectedSkill().PlaySkillSequence(_selectedPlayer, tileClicked));
				_selectedPlayer.CurrentTile.SetSelected(false);
				_selectedPlayer = null;
			}
		}

		if (onComplete != null)
		{
			onComplete();
		}
	}

	private IEnumerator MoveUnitToTile(UnitControllerBase unit, MapTile targetTile)
	{
		var tileBefore = unit.CurrentTile;
		tileBefore.SetSelected(false);
		yield return StartCoroutine(unit.MoveToTile(targetTile));
		tileBefore.AssignUnit (null);
		targetTile.AssignUnit (unit);
	}

    private MapTile GetTile(Dictionary<string, MapTile> dict, int x, int y)
    {
        MapTile tile = null;
        dict.TryGetValue(Const.GetTileKey(x, y), out tile);
        return tile;
    }
}
