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

    public bool IsAnimating { get; private set; }

    protected IEnumerator Start()
    {
        this._mapController.Init();
        yield return new WaitForEndOfFrame();
        BattleManager.CreateBattleInstance(this, this._mapController);
        this.InitUnits();
        BattleManager.Instance.InitTurnOrder();
        StartCoroutine(StartBattle());
    }
        
    public void UpdateTurnOrderView(List<UnitControllerBase> orderedList)
    {
        this._turnOrderView.ShowOrder(orderedList);
    }

    private void InitUnits()
    {
        var layout = MapLayout.GetDefaultLayout();
        foreach (var playerPosition in layout.playerPositions)
        {
            var tile = _mapController.GetTile(Const.Team.Player, playerPosition.X, playerPosition.Y);
            this.CreateUnitOnTile(Const.Team.Player, tile);
        }

        foreach (var enemyPosition in layout.enemyPositions)
        {
            var tile = _mapController.GetTile(Const.Team.Enemy, enemyPosition.X, enemyPosition.Y);
            this.CreateUnitOnTile(Const.Team.Enemy, tile);
        }
    }

    private IEnumerator StartBattle()
    {
        var allUnits = BattleManager.Instance.AllUnits;

        while (true)
        {
            var actor = BattleManager.Instance.GetNextActor();
            if (actor != null)
            {
                if (actor.Team == Const.Team.Enemy)
                {
                    yield return StartCoroutine(actor.RunAI(allUnits));
                }
                else
                {
                    yield return StartCoroutine(this._mapController.WaitForUserInput(actor));
                }

                if (BattleManager.Instance.AllOpponentsDefeated(actor.Team))
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
        yield return 0;
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

            var postfix = BattleManager.Instance.GetUnitPostfix(character.Name);

            unitController.Init(team, character, postfix);
            tile.AssignUnit(unitController);

            unitController.gameObject.name = unitController.UnitName;

            BattleManager.Instance.AddUnit(team, unitController);
        }
    }

    private void OnUnitAnimationStateChange(bool isAnimating)
    {
        this.IsAnimating = isAnimating;
    }
}
