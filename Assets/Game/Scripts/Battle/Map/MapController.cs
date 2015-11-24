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

    private void OnTileClick(MapTile tileClicked)
    {
        if (!BattleManager.Instance.EnableTileTouch)
        {
            return;
        }

		var processes = new Queue<IEnumerator> ();
		switch (BattleManager.Instance.Phase) {

		case BattleManager.BattlePhase.MovementSelect:
			processes.Enqueue (this.MoveUnitToTile (this._selectedPlayer, tileClicked));
			break;
		case BattleManager.BattlePhase.TargetSelect:
			processes.Enqueue (this._selectedPlayer.GetSelectedSkill ().PlaySkillSequence (this._selectedPlayer, tileClicked));
			break;
		default:
			break;
		}
		StartCoroutine (this.RunAnimationQueue (processes));
    }

	private void OnTileSelect(MapTile tileClicked, Action onComplete = null)
	{
		var processes = new Queue<IEnumerator> ();
		switch (BattleManager.Instance.Phase) {

		case BattleManager.BattlePhase.MovementSelect:
			processes.Enqueue (this.MoveUnitToTile (this._selectedPlayer, tileClicked));
			break;
		case BattleManager.BattlePhase.TargetSelect:
			processes.Enqueue (this._selectedPlayer.GetSelectedSkill ().PlaySkillSequence (this._selectedPlayer, tileClicked));
			break;
		default:
			break;
		}
		StartCoroutine (this.RunAnimationQueue (processes));
	}

	public void SetActor(UnitControllerBase actor)
	{
		this._selectedPlayer = actor;
		this._selectedPlayer.CurrentTile.SetSelected(true);
	}

	public void RunAI(UnitControllerBase unit)
	{
		var processes = new Queue<IEnumerator> ();
		processes.Enqueue (unit.RunAI ());
		StartCoroutine (this.RunAnimationQueue (processes));
	}

	private IEnumerator RunAnimationQueue(Queue<IEnumerator> queue)
	{
		BattleManager.Instance.Phase = BattleManager.BattlePhase.Animation;
		while (queue.Count > 0) {
			var process = queue.Dequeue ();
			yield return StartCoroutine (process);
		}

		if (this._selectedPlayer != null) {
			_selectedPlayer.CurrentTile.SetSelected(false);
		}

		_selectedPlayer = null;
		BattleManager.Instance.Phase = BattleManager.BattlePhase.NextRound;
	}

	private IEnumerator ShowConfirmation(Action onOk, Action onCancel)
	{
		yield return StartCoroutine (PopupManager.OkCancel (onOk, onCancel));
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
