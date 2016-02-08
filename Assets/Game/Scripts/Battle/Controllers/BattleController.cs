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
    [SerializeField] private EnmityView _enmityView;

    private BattleModel _battleModel;

    private IEnumerator Start()
    {
        var numberOfRows = 4;
        var numberOfColumns = 3;

        this._battleView.InitMap(numberOfRows, numberOfColumns, OnTileClick);

        this._battleModel = new BattleModel();

        this.SetupEventListeners();

        this._battleModel.CreateBattleMap(numberOfRows, numberOfColumns);
        yield return 0;

        this._battleModel.SpawnCharactersOnMap();
        yield return 0;

        this._battleActionMenu.Init(this._battleModel);

        this._battleModel.ChangePhase(BattleModel.BattlePhase.NextRound);
    }

    private void SetupEventListeners()
    {
        this._battleModel.onTileCreated += this._battleView.BindTileController;
        this._battleModel.onTurnOrderChanged += this._turnOrderView.UpdateView;
        this._battleModel.onBattleCharacterCreated += this.OnCreateBattleUnit;
        this._battleModel.onProcessActionResult += this.ProcessActionResultQueue;
        this._battleModel.onBattlePhaseChange += this.OnBattlePhaseChange;
        this._battleModel.onNextActionableEnemyChanged += this._enmityView.UpdateView;
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

    private void OnCreateBattleUnit(BattleCharacter character)
    {
        this._battleView.SpawnUnit(character);
    }

    private void ProcessActionResultQueue(Queue<BattleActionResult> resultQueue, Action callback)
    {
        StartCoroutine(this.ProcessActionResultQueueCoroutine(resultQueue, callback));
    }

    private IEnumerator ProcessActionResultQueueCoroutine(Queue<BattleActionResult> resultQueue, Action callback)
    {
        while (resultQueue.Count > 0)
        {
            var result = resultQueue.Dequeue();
            if (result != null)
            {
                yield return StartCoroutine(this.ProcessActionResult(result));
            }
        }
        callback();
    }

    private IEnumerator ProcessActionResult(BattleActionResult actionResult)
    {
        switch (actionResult.type)
        {

            case Const.ActionType.Movement:
                yield return StartCoroutine(this.ProcessMovementActionResult(actionResult));
                break;
            case Const.ActionType.Skill:
                yield return StartCoroutine(this.ProcessSkillActionResult(actionResult));
                break;
            default:
                yield return null;
                break;	
        }
    }

    private IEnumerator ProcessMovementActionResult(BattleActionResult actionResult)
    {
        var movementEffect = actionResult.allSkillEffectResult[0].effectsOnTarget[0];
        var actor = movementEffect.target;
        var newPosition = movementEffect.positionChangeTo;

        var occupiedPositions = this._battleModel.GetMapPositionsForPattern(actor.BaseCharacter.PatternShape, Const.SkillTargetGroup.Ally, actor.Team, newPosition);

        yield return StartCoroutine(this._battleView.MoveUnitToMapPosition(actor, occupiedPositions));
    }

    private IEnumerator ProcessSkillActionResult(BattleActionResult actionResult)
    {
        yield return StartCoroutine(this._battleView.PlaySkillAnimation(actionResult));
    }

    private void OnBattlePhaseChange(BattleModel.BattlePhase battlePhase)
    {
        switch (battlePhase)
        {
            default:
                break;
        }
    }

    void OnDestroy()
    {
        this._battleModel.onTileCreated -= this._battleView.BindTileController;
        this._battleModel.onTurnOrderChanged -= this._turnOrderView.UpdateView;
        this._battleModel.onBattleCharacterCreated -= this.OnCreateBattleUnit;
        this._battleModel.onProcessActionResult -= this.ProcessActionResultQueue;
        this._battleModel.onBattlePhaseChange -= this.OnBattlePhaseChange;
        this._battleModel.onNextActionableEnemyChanged -= this._enmityView.UpdateView;
    }
}
