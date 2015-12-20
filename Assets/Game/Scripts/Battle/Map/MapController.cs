using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
//    [SerializeField] private Grid _playerGrid;
//    [SerializeField] private Grid _enemyGrid;

    private Dictionary<string, TileController> _playerTiles = new Dictionary<string, TileController>();
    private Dictionary<string, TileController> _enemyTiles = new Dictionary<string, TileController>();

    private BattleUnitController _selectedPlayer;

//    public void Init()
//    {
//        _playerTiles = _playerGrid.InitTiles(this.OnTileClick);
//        _enemyTiles = _enemyGrid.InitTiles(this.OnTileClick);
//    }

//    public MapTile GetTile(Const.Team team, int x, int y)
//    {
//        if (team == Const.Team.Player)
//        {
//            return GetTile(_playerTiles, x, y);
//        }
//        else
//        {
//            return GetTile(_enemyTiles, x, y);
//        }
//    }
//
    private void OnTileClick(TileController tileClicked)
    {
        if (!BattleManager.Instance.EnableTileTouch)
        {
            return;
        }
			
		switch (BattleManager.Instance.Phase) {

		case BattleManager.BattlePhase.MovementSelect:
			var processes = new Queue<IEnumerator> ();
			processes.Enqueue (this.MoveUnitToTile (this._selectedPlayer, tileClicked));
			StartCoroutine (this.RunAnimationQueue (processes));
			break;
		case BattleManager.BattlePhase.TargetSelect:
//			this.ConfirmSkillSelection (this._selectedPlayer.GetSelectedSkill (), tileClicked);
			break;
		default:
			break;
		}
    }

	private void ConfirmSkillSelection(SkillComponentBase skillComponent, TileController targetTile)
	{
		var skill = skillComponent.GetSkill ();
		var affectedTiles = BattleManager.Instance.GetAffectedTiles (targetTile, skill.SkillTarget.Pattern);
		this.SetTilesAffected (affectedTiles, true);

//		Action onCancel = () => {
//			this.SetTilesAffected(affectedTiles, false);
//		};
//
//		Action onOk = () => {
//			var processes = new Queue<IEnumerator> ();
//			processes.Enqueue (skillComponent.PlaySkillSequence (this._selectedPlayer, targetTile));
//			this.SetTilesAffected(affectedTiles, false);
//			StartCoroutine(this.RunAnimationQueue(processes));
//		};
//
//		PopupManager.OkCancel (onOk, onCancel);
	}

	private void SetTilesAffected(List<TileController> tiles, bool affected)
	{
		// TODO: show affected color
		foreach (var tile in tiles) {
//			tile.SetSelected (affected);
		}
	}

	private void OnTileSelect(TileController tileClicked, BattleAction onComplete = null)
	{
		var processes = new Queue<IEnumerator> ();
		switch (BattleManager.Instance.Phase) {

		case BattleManager.BattlePhase.MovementSelect:
			processes.Enqueue (this.MoveUnitToTile (this._selectedPlayer, tileClicked));
			break;
		case BattleManager.BattlePhase.TargetSelect:
//			processes.Enqueue (this._selectedPlayer.GetSelectedSkill ().PlaySkillSequence (this._selectedPlayer, tileClicked));
			break;
		default:
			break;
		}
		StartCoroutine (this.RunAnimationQueue (processes));
	}

	public void SetActor(BattleUnitController actor)
	{
		this._selectedPlayer = actor;
//		this._selectedPlayer.CurrentTile.SetSelected(true);
	}

	public void RunAI(BattleUnitController unit)
	{
		var processes = new Queue<IEnumerator> ();
//		processes.Enqueue (unit.RunAI ());
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
//			_selectedPlayer.CurrentTile.SetSelected(false);
		}

		_selectedPlayer = null;
		BattleManager.Instance.Phase = BattleManager.BattlePhase.NextRound;
	}

	private IEnumerator MoveUnitToTile(BattleUnitController unit, TileController targetTile)
	{
//		var tileBefore = unit.CurrentTile;
//		tileBefore.SetSelected(false);
		yield return StartCoroutine(unit.MoveToTile(targetTile));
//		tileBefore.AssignUnit (null);
//		targetTile.AssignUnit (unit);
	}

//    private MapTile GetTile(Dictionary<string, MapTile> dict, int x, int y)
//    {
//        MapTile tile = null;
//        dict.TryGetValue(Const.GetTileKey(x, y), out tile);
//        return tile;
//    }
}
