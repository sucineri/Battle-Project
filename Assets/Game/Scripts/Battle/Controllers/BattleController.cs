using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BattleController : MonoBehaviour
{

    [SerializeField] private BattleActionMenu _battleActionMenu;
    [SerializeField] private TurnOrderView _turnOrderView;
    [SerializeField] private BattleView _battleView;

    private BattleModel _battleModel;

    private IEnumerator Start()
    {
        var numberOfRows = 4;
        var numberOfColumns = 3;

        this._battleView.InitMap(numberOfRows, numberOfColumns, OnTileClick);

        this._battleModel = new BattleModel();

        this._battleModel.onTileCreated += this._battleView.BindTileController;
        this._battleModel.onTurnOrderChanged += this._turnOrderView.UpdateView;
        this._battleModel.onBattleCharacterCreated += this.OnCreateBattleUnit;
        this._battleModel.onProcessOutcome += this.ProcessOutcomeQueue;
        this._battleModel.onBattlePhaseChange += this.OnBattlePhaseChange;

        this._battleModel.CreateBattleMap(numberOfRows, numberOfColumns);
        yield return 0;

        this._battleModel.SpawnCharactersOnMap();
        yield return 0;

        this._battleActionMenu.Init(this._battleModel);

        this._battleModel.ChangePhase(BattleModel.BattlePhase.NextRound);
    }

    private void OnTileClick(MapPosition tilePosition)
    {
        switch (this._battleModel.CurrentPhase)
        {
            case BattleModel.BattlePhase.ActionSelect:
                this._battleModel.CurrentCharacterMoveAction(tilePosition);
                break;
            case BattleModel.BattlePhase.TargetSelect:
                this._battleModel.CurrentCharacterSkillAction(tilePosition);
                break;
            default:
                break;
        }
    }

    private void OnCreateBattleUnit(MapPosition mapPosition, BattleCharacter character)
    {
        var mapTile = this._battleView.GetTileAtMapPosition(mapPosition);

        this._battleView.SpawnUnitOnTile(character, mapTile);
    }

    private void ProcessOutcomeQueue(Queue<BattleActionOutcome> outcomeQueue, Action callback)
    {
        StartCoroutine(this.ProcessOutcomeQueueCoroutine(outcomeQueue, callback));
    }

    private IEnumerator ProcessOutcomeQueueCoroutine(Queue<BattleActionOutcome> outcomeQueue, Action callback)
    {
        while (outcomeQueue.Count > 0)
        {
            var outcome = outcomeQueue.Dequeue();
            if (outcome != null)
            {
                yield return StartCoroutine(this.ProcessOutcome(outcome));
            }
        }
        callback();
    }

    private IEnumerator ProcessOutcome(BattleActionOutcome outcome)
    {
        switch (outcome.type)
        {

            case Const.ActionType.Movement:
                yield return StartCoroutine(this.ProcessMovementOutcome(outcome));
                break;
            case Const.ActionType.Skill:
                yield return StartCoroutine(this.ProcessSkillOutcome(outcome));
                break;
            default:
                yield return null;
                break;	
        }
    }

    private IEnumerator ProcessMovementOutcome(BattleActionOutcome outcome)
    {
        var movementOutcome = outcome.actorOutcome;
        var actor = movementOutcome.target;
        var tile = this._battleView.GetTileAtMapPosition(movementOutcome.positionChangeTo);
        if (tile != null)
        {
            yield return StartCoroutine(this._battleView.MoveUnitToTile(actor, tile));
        }
    }

    private IEnumerator ProcessSkillOutcome(BattleActionOutcome outcome)
    {
        yield return StartCoroutine(this._battleView.PlaySkillAnimation(this._battleModel.CurrentActor.SelectedSkill, outcome));
    }

    private void OnBattlePhaseChange(BattleModel.BattlePhase battlePhase)
    {
        switch (battlePhase)
        {
            default:
                break;
        }
    }

    private void CreateUnitOnTile(Const.Team team, TileController tile)
    {
        if (tile != null)
        {
            var character = team == Const.Team.Player ? Character.Fighter() : Character.Slime();
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

            unitController.gameObject.name = character.Name;
        }
    }

    void OnDestroy()
    {
        this._battleModel.onTileCreated -= this._battleView.BindTileController;
        this._battleModel.onTurnOrderChanged -= this._turnOrderView.UpdateView;
        this._battleModel.onBattleCharacterCreated -= this.OnCreateBattleUnit;
        this._battleModel.onProcessOutcome -= this.ProcessOutcomeQueue;
        this._battleModel.onBattlePhaseChange -= this.OnBattlePhaseChange;
    }
}
