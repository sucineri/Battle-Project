using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BattlePresenter : MonoBehaviour {

	[SerializeField] private MapView _mapView;
	[SerializeField] private BattleUnitsView _unitView;
	[SerializeField] private TurnOrderView _turnOrderView;
	[SerializeField] private ActionMenu _actionMenu;

	private UnitControllerBase _currentActor;

	void Start()
	{
		StartCoroutine(this.Init());
	}

	public IEnumerator Init()
	{
		BattleManager.CreateBattleInstance ();
		BattleManager.Instance.onTurnOrderChanged += this.OnTurnOrderChanged;
		BattleManager.Instance.onBattlePhaseChange += this._actionMenu.OnBattlePhaseChange;
		BattleManager.Instance.onBattlePhaseChange += this.OnBattlePhaseChange;

//		this._actionMenu.Init (this.OnMoveSelect, this.OnSkillSelect, this.OnSelectionCancel);
		yield return 0;

		var allTiles = this._mapView.InitTiles (this.OnMapTileClick);
		BattleManager.Instance.SetMapTiles(allTiles);
		yield return 0;

		this.SpawnUnitsOnMap ();
		yield return 0;

		this.NextRound ();
	}

	private void OnBattlePhaseChange(BattleManager.BattlePhase battlePhase)
	{
		if (battlePhase == BattleManager.BattlePhase.NextRound) {
			this.NextRound ();
		}
	}

	private void OnMapTileClick(MapTile tileClicked)
	{
		if (!BattleManager.Instance.EnableTileTouch)
		{
			return;
		}

		switch (BattleManager.Instance.Phase) {

		case BattleManager.BattlePhase.MovementSelect:
			var processes = new Queue<IEnumerator> ();
			processes.Enqueue (this.MoveUnitToTile (this._currentActor, tileClicked));
			StartCoroutine (this.RunAnimationQueue (processes));
			break;
		case BattleManager.BattlePhase.TargetSelect:
			// TODO: selected skill info should be stored somewhere else
			this.ConfirmSkillSelection (this._currentActor.GetSelectedSkill(), tileClicked);
			break;
		default:
			break;
		}
	}

	private void OnTurnOrderChanged(List<UnitControllerBase> unitActionOrder)
	{
		this._turnOrderView.ShowOrder (unitActionOrder);
	}

	private void NextRound()
	{
		if (BattleManager.Instance.AllUnitsDefeated (Const.Team.Enemy)) {
			// player wins
			return;
		}
		else if (BattleManager.Instance.AllUnitsDefeated (Const.Team.Player)) {
			// enemy wins
			return;
		}
			
		this._currentActor = BattleManager.Instance.GetNextActor();

		if (this._currentActor.Team == Const.Team.Enemy) {
			this.ProcessEnemyTurn (this._currentActor);
		}
		else 
		{
			this.ProcessPlayerTurn (this._currentActor);
		}
	}

	private void ProcessEnemyTurn(UnitControllerBase actor)
	{
		this.RunAI (actor);
	}

	private void ProcessPlayerTurn(UnitControllerBase actor)
	{
		this._currentActor = actor;
		BattleManager.Instance.Phase = BattleManager.BattlePhase.ActionSelect;
		this._actionMenu.CreateMenu (actor);

		// TODO: light up movable areas
		var tile = BattleManager.Instance.GetUnitOccupiedTile (this._currentActor);
		if (tile != null) {
			tile.SetSelected (true);
		}
	}

	private void RunAI(UnitControllerBase unit)
	{
		var processes = new Queue<IEnumerator> ();
		// TODO: move AI logic somewhere else
		processes.Enqueue (unit.RunAI ());
		StartCoroutine (this.RunAnimationQueue (processes));
	}

	private IEnumerator MoveUnitToTile(UnitControllerBase unit, MapTile tile)
	{
		var tileBefore = BattleManager.Instance.GetUnitOccupiedTile (this._currentActor);
		tileBefore.SetSelected (false);
		BattleManager.Instance.AssignTileToUnit (unit, tile);
		yield return StartCoroutine (this._unitView.MoveUnitToTile (unit, tile));
	}

	private void ConfirmSkillSelection(SkillComponentBase skillComponent, MapTile targetTile)
	{
		var skill = skillComponent.GetSkill ();
		var affectedTiles = BattleManager.Instance.GetAffectedTiles (targetTile, skill.SkillTarget.Pattern);
		this._mapView.SetTilesAffected (affectedTiles, true);

//		Action onCancel = () => {
//			this._mapView.SetTilesAffected(affectedTiles, false);
//		};
//
//		Action onOk = () => {
//			var processes = new Queue<IEnumerator> ();
//			processes.Enqueue (skillComponent.PlaySkillSequence (this._currentActor, targetTile));
//			this._mapView.SetTilesAffected(affectedTiles, false);
//			StartCoroutine(this.RunAnimationQueue(processes));
//		};
//
//		PopupManager.OkCancel (onOk, onCancel);
	}

	private IEnumerator RunAnimationQueue(Queue<IEnumerator> queue)
	{
		BattleManager.Instance.Phase = BattleManager.BattlePhase.Animation;
		while (queue.Count > 0) {
			var process = queue.Dequeue ();
			yield return StartCoroutine (process);
		}

		if (this._currentActor != null) {
			var tile = BattleManager.Instance.GetUnitOccupiedTile (this._currentActor);
			if (tile != null) {
				tile.SetSelected (true);
			}
		}

		_currentActor = null;
		BattleManager.Instance.Phase = BattleManager.BattlePhase.NextRound;
	}

	private void SpawnUnitsOnMap()
	{
		// TODO: real character data
//		var layout = MapLayout.GetDefaultLayout();
//		foreach (var playerPosition in layout.playerPositions)
//		{
//			var playerUnit = BattleUnitFactory.CreateBattleUnit (Const.Team.Player, CharacterStats.Fighter ());
//			var playerTile = BattleManager.Instance.GetTile (Const.Team.Player, playerPosition.X, playerPosition.Y);
//			this._unitView.SpawnUnitOnTile (playerUnit, playerTile);
//			BattleManager.Instance.AssignTileToUnit (playerUnit, playerTile);
//		}
//
//		foreach (var enemyPosition in layout.enemyPositions)
//		{
//			var enemyUnit = BattleUnitFactory.CreateBattleUnit (Const.Team.Enemy, CharacterStats.Slime ());
//			var enemyTile = BattleManager.Instance.GetTile (Const.Team.Enemy, enemyPosition.X, enemyPosition.Y);
//			this._unitView.SpawnUnitOnTile (enemyUnit, enemyTile);
//			BattleManager.Instance.AssignTileToUnit (enemyUnit, enemyTile);
//		}
	}

	private void OnMoveSelect(UnitControllerBase actor)
	{
		BattleManager.Instance.Phase = BattleManager.BattlePhase.MovementSelect;
		this._actionMenu.ShowMenu (false);
		this._actionMenu.ShowCancel (true);
	}

	private void OnSkillSelect(UnitControllerBase actor, Skill selectedSkill)
	{
		BattleManager.Instance.Phase = BattleManager.BattlePhase.TargetSelect;
		actor.SelectedSkill = selectedSkill;
		this._actionMenu.ShowMenu (false);
		this._actionMenu.ShowCancel (true);
	}

	private void OnSelectionCancel()
	{
		BattleManager.Instance.Phase = BattleManager.BattlePhase.ActionSelect;
		this._actionMenu.ShowMenu (true);
		this._actionMenu.ShowCancel (false);
	}
}
