using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BattleController : MonoBehaviour
{
    [SerializeField]
    private MapController _mapController;
    [SerializeField]
    private TurnOrderView _turnOrderView;
	[SerializeField]
	private ActionMenuView _actionMenu;

    protected IEnumerator Start()
    {
        this._mapController.Init();
		this._actionMenu.Init (this.OnMoveSelect, this.OnSkillSelect, this.OnSelectionCancel);
        yield return new WaitForEndOfFrame();
//		BattleManager.CreateBattleInstance(this, this._mapController);
//        this.InitUnits();
//        BattleManager.Instance.InitTurnOrder();
		BattleManager.Instance.onBattlePhaseChange += this.OnBattlePhaseChange;
//		this.NextRound ();
    }
        
    public void UpdateTurnOrderView(List<UnitControllerBase> orderedList)
    {
        this._turnOrderView.ShowOrder(orderedList);
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

	private void OnBattlePhaseChange(BattleManager.BattlePhase battlePhase)
	{
		if (battlePhase == BattleManager.BattlePhase.NextRound) {
//			this.NextRound ();
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

	private void ProcessEnemyTurn(UnitControllerBase actor)
	{
		this._mapController.RunAI (actor);
	}

	private void ProcessPlayerTurn(UnitControllerBase actor)
	{
		BattleManager.Instance.Phase = BattleManager.BattlePhase.ActionSelect;
		this._actionMenu.CreateMenu (actor);
		this._mapController.SetActor (actor);
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

    private void CreateUnitOnTile(Const.Team team, MapTile tile)
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

            var unitController = unit.GetComponent<UnitControllerBase>();

//            var postfix = BattleManager.Instance.GetUnitPostfix(character.Name);

            unitController.Init(team, character, 'A');
            tile.AssignUnit(unitController);

            unitController.gameObject.name = unitController.UnitName;

//            BattleManager.Instance.AssignTileToUnit(team, unitController);
        }
    }
}
