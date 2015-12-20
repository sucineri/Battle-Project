using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BattleController : MonoBehaviour
{

	[SerializeField] private BattleActionMenu _battleActionMenu;
	[SerializeField] private MapView _mapView;
	[SerializeField] private TurnOrderView _turnOrderView;
	[SerializeField] private BattleUnitsView _battleUnitsView;

	private BattleModel _battleModel;

	private IEnumerator Start()
    {
		var numberOfRows = 4;
		var numberOfColumns = 3;

		this._mapView.InitGrids (numberOfRows, numberOfColumns, OnTileClick);

		this._battleModel = new BattleModel ();

		this._battleModel.onTileCreated += this._mapView.AssignTile;
		this._battleModel.onTurnOrderChanged += this._turnOrderView.UpdateView;
		this._battleModel.onBattleCharacterCreated += this.OnCreateBattleUnit;
		this._battleModel.onProcessOutcome += this.ProcessOutcomeQueue;
		this._battleModel.onBattlePhaseChange += this.OnBattlePhaseChange;

		this._battleModel.CreateBattleMap (numberOfRows, numberOfColumns);
		yield return 0;

		this._battleModel.SpawnCharactersOnMap ();
		yield return 0;

		this._battleActionMenu.Init (this._battleModel);

		this._battleModel.ChangePhase(BattleModel.BattlePhase.NextRound);
    }

	private void OnTileClick(MapPosition tilePosition)
	{
		switch (this._battleModel.CurrentPhase) {
		case BattleModel.BattlePhase.ActionSelect:
			this._battleModel.CurrentCharacterMoveAction (tilePosition);
			break;
		case BattleModel.BattlePhase.TargetSelect:
			this._battleModel.CurrentCharacterSkillAction (tilePosition);
			break;
		default:
			break;
		}
	}

	private void OnCreateBattleUnit(MapPosition mapPosition, BattleCharacter character)
	{
		var mapTile = this._mapView.GetTileAtMapPosition (mapPosition);

		this._battleUnitsView.SpawnUnitOnTile (character, mapTile);
	}

	private void ProcessOutcomeQueue(Queue<BattleActionOutcome> outcomeQueue, Action callback)
	{
		StartCoroutine (this.ProcessOutcomeQueueCoroutine(outcomeQueue, callback));
	}

	private IEnumerator ProcessOutcomeQueueCoroutine(Queue<BattleActionOutcome> outcomeQueue, Action callback)
	{
		while (outcomeQueue.Count > 0) {
			var outcome = outcomeQueue.Dequeue ();
			if (outcome != null) {
				yield return StartCoroutine(this.ProcessOutcome(outcome));
			}
		}
		callback ();
	}

	private IEnumerator ProcessOutcome(BattleActionOutcome outcome)
	{
		switch (outcome.Type) {

		case Const.ActionType.Movement:
			yield return StartCoroutine (this.ProcessMovementOutcome (outcome));
			break;
		default:
			yield return null;
			break;	
		}
	}

	private IEnumerator ProcessMovementOutcome(BattleActionOutcome outcome)
	{
		var movementOutcome = outcome.ActorOutcome;
		var actor = movementOutcome.Target;
		var tile = this._mapView.GetTileAtMapPosition (movementOutcome.PositionChangeTo);
		if (tile != null) {
			yield return StartCoroutine (this._battleUnitsView.MoveUnitToTile (actor, tile));
		}

	}

//    private void InitUnits()
//    {
//        var layout = MapLayout.GetDefaultLayout();
//        foreach (var playerPosition in layout.playerPositions)
//        {
//            var tile = _mapController.GetTile(Const.Team.Player, playerPosition.X, playerPosition.Y);
//            this.CreateUnitOnTile(Const.Team.Player, tile);
//        }
//
//        foreach (var enemyPosition in layout.enemyPositions)
//        {
//            var tile = _mapController.GetTile(Const.Team.Enemy, enemyPosition.X, enemyPosition.Y);
//            this.CreateUnitOnTile(Const.Team.Enemy, tile);
//        }
//    }

	private void OnBattlePhaseChange(BattleModel.BattlePhase battlePhase)
	{
		switch (battlePhase) {
		default:
			break;
		}
	}

//	private void NextRound()
//	{
//		var actor = BattleManager.Instance.GetNextActor();
//		if (BattleManager.Instance.AllOpponentsDefeated (actor.Team)) {
//			// end battle
//		}
//		else if (actor.Team == Const.Team.Enemy) {
//			this.ProcessEnemyTurn (actor);
//		}
//		else 
//		{
//			this.ProcessPlayerTurn (actor);
//		}
//	}

//	private void ProcessEnemyTurn(UnitControllerBase actor)
//	{
//		this._mapController.RunAI (actor);
//	}
//
//	private void ProcessPlayerTurn(UnitControllerBase actor)
//	{
//		BattleManager.Instance.Phase = BattleManager.BattlePhase.ActionSelect;
//		this._actionMenu.CreateMenu (actor);
//		this._mapController.SetActor (actor);
//	}

	private void OnMoveSelect(BattleUnitController actor)
	{
		BattleManager.Instance.Phase = BattleManager.BattlePhase.MovementSelect;
		this._battleActionMenu.ShowMenu (false);
		this._battleActionMenu.ShowCancel (true);
	}

	private void OnSkillSelect(BattleUnitController actor, Skill selectedSkill)
	{
		BattleManager.Instance.Phase = BattleManager.BattlePhase.TargetSelect;
//		actor.SelectedSkill = selectedSkill;
		this._battleActionMenu.ShowMenu (false);
		this._battleActionMenu.ShowCancel (true);
	}

	private void OnSelectionCancel()
	{
		BattleManager.Instance.Phase = BattleManager.BattlePhase.ActionSelect;
		this._battleActionMenu.ShowMenu (true);
		this._battleActionMenu.ShowCancel (false);
	}

    private void CreateUnitOnTile(Const.Team team, TileController tile)
    {
        if (tile != null)
        {
            var character = team == Const.Team.Player ? CharacterStats.Fighter() : CharacterStats.Slime();
            var prefab = Resources.Load(character.ModelPath) as GameObject;

            var position = tile.transform.position;
            var unit = Instantiate(prefab) as GameObject;

            unit.transform.position = position;
            if (team == Const.Team.Enemy)
            {
                unit.transform.Rotate(new Vector3(0f, 180f, 0f));
            }
            unit.transform.SetParent(this.transform);

            var unitController = unit.GetComponent<BattleUnitController>();

//            var postfix = BattleManager.Instance.GetUnitPostfix(character.Name);

//            unitController.Init(team, character, 'A');
//            tile.AssignUnit(unitController);

			unitController.gameObject.name = character.Name;

//            unitController.gameObject.name = unitController.UnitName;

//            BattleManager.Instance.AssignTileToUnit(team, unitController);
        }
    }

	void OnDestroy()
	{
		this._battleModel.onTileCreated -= this._mapView.AssignTile;
		this._battleModel.onTurnOrderChanged -= this._turnOrderView.UpdateView;
		this._battleModel.onBattleCharacterCreated -= this.OnCreateBattleUnit;
		this._battleModel.onProcessOutcome -= this.ProcessOutcomeQueue;
		this._battleModel.onBattlePhaseChange -= this.OnBattlePhaseChange;
	}
}
