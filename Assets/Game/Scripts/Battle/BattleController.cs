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

    private bool _isAnimating = false;
    private bool _playerTurn = false;
    public bool EnableInput { get { return !this._isAnimating && _playerTurn; } }

    private List<UnitControllerBase> _playerUnits = new List<UnitControllerBase>();
    private List<UnitControllerBase> _enemyUnits = new List<UnitControllerBase>();

    private TurnOrderService _turnOrderManager = new TurnOrderService();
    private UnitNameService _unitNameManager = new UnitNameService();

    public void Start()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        _mapController.Init(this);
        yield return new WaitForEndOfFrame();

        InitUnits();

        var allUnits = this._playerUnits.Concat(this._enemyUnits).ToList();
        _turnOrderManager.Init(allUnits, this.UpdateTurnOrderView);

        StartCoroutine(StartBattle());
    }

    private void InitUnits()
    {
        var layout = MapLayout.GetDefaultLayout();
        foreach (var playerPosition in layout.playerPositions)
        {
            var tile = _mapController.GetTile(Const.Team.Player, playerPosition.Row, playerPosition.Column);
            this.CreateUnitOnTile(Const.Team.Player, tile);
        }

        foreach (var enemyPosition in layout.enemyPositions)
        {
            var tile = _mapController.GetTile(Const.Team.Enemy, enemyPosition.Row, enemyPosition.Column);
            this.CreateUnitOnTile(Const.Team.Enemy, tile);
        }
    }

    private IEnumerator StartBattle()
    {
        var allUnits = this._playerUnits.Concat(this._enemyUnits).ToList();

        while (true)
        {
            var actor = this._turnOrderManager.GetNextActor();
            if (actor != null)
            {
                if (actor.Team == Const.Team.Enemy)
                {
                    this._playerTurn = false;
                    yield return StartCoroutine(actor.RunAI(allUnits));
                } else
                {
                    this._playerTurn = true;
                    yield return StartCoroutine(this._mapController.WaitForUserInput(actor));
                }

                if (AllOpponentsDefeated(actor.Team))
                {
                    break;
                }
            } else
            {
                break;
            }
        }
        yield return 0;
    }

    private List<UnitControllerBase> GetOpponentList(Const.Team actorTeam)
    {
        return actorTeam == Const.Team.Player ? this._enemyUnits : this._playerUnits;
    }

    private bool AllOpponentsDefeated(Const.Team actorTeam)
    {
        var list = GetOpponentList(actorTeam);
        return list.Find(x => !x.IsDead) == null;
    }

    private void CreateUnitOnTile(Const.Team team, MapTile tile)
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

            var unitController = unit.GetComponent<UnitControllerBase>();
            unitController.onAnimationStateChange += OnUnitAnimationStateChange;

            var postfix = this._unitNameManager.GetPostfix(character.Name);

            unitController.Init(team, character, postfix);
            tile.AssignUnit(unitController);

            unitController.gameObject.name = unitController.UnitName;

            var list = team == Const.Team.Player ? _playerUnits : _enemyUnits;
            list.Add(unitController);
        }
    }

    private void OnUnitAnimationStateChange(bool isAnimating)
    {
        this._isAnimating = isAnimating;
    }
   
    private void UpdateTurnOrderView(List<UnitControllerBase> orderedList)
    {
        this._turnOrderView.ShowOrder(orderedList);
    }
}
