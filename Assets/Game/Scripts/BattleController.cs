using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BattleController : MonoBehaviour 
{
    [SerializeField] private MapController mapController;

	private UnitController selectedPlayer = null;
    private UnitController selectedEnemy = null;

    private List<UnitController> playerUnits = new List<UnitController>();
    private List<UnitController> enemyUnits = new List<UnitController>();

    private bool isAnimating = false;

    private TurnOrderManager turnOrderManager = new TurnOrderManager();

    private Action onTileClick = null;

    private Dictionary<string, char> nameDict = new Dictionary<string, char>();

    public void Start()
    {
        StartCoroutine(Init());
    }

	private IEnumerator Init()
	{
        mapController.Init(this.OnTileClick, this.OnTileClick);
		yield return new WaitForEndOfFrame();

		InitUnits ();

        var allUnits = this.playerUnits.Concat(this.enemyUnits).ToList();
        turnOrderManager.Init(allUnits);

        StartCoroutine(StartBattle());
	}

	private void InitUnits()
	{
        var layout = MapLayout.GetDefaultLayout();
        foreach(var playerPosition in layout.playerPositions)
        {
            var tile = mapController.GetTile(Const.Team.Player, playerPosition.Row, playerPosition.Column);
            this.CreateUnitOnTile(Const.Team.Player, tile);
        }

        foreach(var enemyPosition in layout.enemyPositions)
        {
            var tile = mapController.GetTile(Const.Team.Enemy, enemyPosition.Row, enemyPosition.Column);
            this.CreateUnitOnTile(Const.Team.Enemy, tile);
        }
	}

    private char GetPostfix(string characterName)
    {
        if(this.nameDict.ContainsKey(characterName))
        {
            if(this.nameDict[characterName] == 'Z')
            {
                this.nameDict[characterName] = 'A';
            }
            else
            {
                this.nameDict[characterName] ++;
            }
        }
        else
        {
            this.nameDict.Add(characterName, 'A');
        }
        return this.nameDict[characterName];
    }

    private IEnumerator StartBattle()
    {
        var allUnits = this.playerUnits.Concat(this.enemyUnits).ToList();

        var index = 0;

        while(true)
        {
            var actor = this.turnOrderManager.GetNextActor();
            if(actor != null)
            {
                if(actor.Team == Const.Team.Enemy)
                {
                    yield return StartCoroutine(actor.RunAI(allUnits));
                }
                else
                {
                    yield return StartCoroutine(this.WaitForUserInput(actor));
                }

                if(AllOpponentDefeated(actor.Team))
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

    private List<UnitController> GetOpponentList(Const.Team actorTeam)
    {
        return actorTeam == Const.Team.Player ? this.enemyUnits : this.playerUnits;
    }

    private bool AllOpponentDefeated(Const.Team actorTeam)
    {
        var list = GetOpponentList(actorTeam);
        return list.Find( x => !x.IsDead ) == null;
    }

    private void CreateUnitOnTile(Const.Team team, MapTile tile)
    {
        if(tile != null)
        {
            var character = team == Const.Team.Player ? Character.Fighter() : Character.Slime();
            var prefab = Resources.Load(character.ModelPath) as GameObject;

            var position = tile.transform.position;
            var unit = Instantiate(prefab) as GameObject;

            unit.transform.position = position;
            if(team == Const.Team.Enemy)
            {
                unit.transform.Rotate(new Vector3(0f, 180f, 0f));
            }
            unit.transform.SetParent(this.transform);

            var unitController = unit.GetComponent<UnitController>();
            unitController.onAnimationStateChange += OnUnitAnimationStateChange;

            var postfix = this.GetPostfix(character.Name);

            unitController.Init(team, character, postfix);
            tile.AssignUnit(unitController);

            unitController.gameObject.name = unitController.UnitName;

            var list = team == Const.Team.Player ? playerUnits : enemyUnits;
            list.Add(unitController);
        }
    }

    private void OnUnitAnimationStateChange(bool isAnimating)
    {
        this.isAnimating = isAnimating;
    }

    private IEnumerator WaitForUserInput(UnitController currentActor)
    {
        var waitForInput = true;
        this.onTileClick = () => { waitForInput = false; };
        this.selectedPlayer = currentActor;
        this.selectedPlayer.CurrentTile.SetSelected(true);

        while (waitForInput)
        {
            yield return 0;
        }
        this.onTileClick = null;
    }

    private void OnTileClick(MapTile tileClicked)
    {
        if(isAnimating) return;
        if(tileClicked.Team == Const.Team.Player)
        {
            StartCoroutine(OnPlayerTileClicked(tileClicked, this.onTileClick));
        }
        else if(tileClicked.CurrentUnit != null)
        {
            StartCoroutine(OnEnemyTileClicked(tileClicked, this.onTileClick));
        }
    }

    private IEnumerator OnPlayerTileClicked(MapTile tileClicked, Action onComplete = null)
    {
        // if a unit has been selectd before
        if(selectedPlayer != null)
        {
            var tileBefore = selectedPlayer.CurrentTile;
            var tileAfter = tileClicked;

            // if a different tile is clicked
            if(tileBefore != tileAfter)
            {
                tileBefore.SetSelected(false);

                var unitOnTileClicked = tileAfter.CurrentUnit;

                // if there's another unit on tile clied
                if(unitOnTileClicked != null)
                {
                    StartCoroutine(unitOnTileClicked.MoveToTile(tileBefore));
                }

                yield return StartCoroutine(selectedPlayer.MoveToTile(tileAfter));

                // swap units assignment
                tileBefore.AssignUnit(unitOnTileClicked);
                tileAfter.AssignUnit(selectedPlayer);
            }
            else // the same tile is clicked again
            {
                tileClicked.SetSelected(false);
            }

            selectedPlayer = null;
        }
        else  // no unit was selected before
        {
            // assign selectedPlayer pointer
            selectedPlayer = tileClicked.CurrentUnit;
            if(selectedPlayer != null)
            {
                // set the state of the tile to be selected
                tileClicked.SetSelected(true);
            }
        }
        yield return 0;
        if(onComplete != null)
        {
            onComplete();
        }
    }

    private IEnumerator OnEnemyTileClicked(MapTile tileClicked, Action onComplete = null)
    {
        if(selectedPlayer != null)
        {
            if(tileClicked.CurrentUnit != null)
            {
                yield return StartCoroutine(selectedPlayer.AttackOpponentOnTile(tileClicked));
                selectedPlayer.CurrentTile.SetSelected(false);
                selectedPlayer = null;
            }
        }
        yield return 0;
        if(onComplete != null)
        {
            onComplete();
        }
    }
}
