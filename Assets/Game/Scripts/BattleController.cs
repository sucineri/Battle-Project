using UnityEngine;
using System.Collections;

public class BattleController : MonoBehaviour 
{
    [SerializeField] private MapController mapController;
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject slimePrefab;

	private UnitController selectedPlayer = null;
    private UnitController selectedEnemy = null;

    private bool isAnimating = false;

    public void Start()
    {
        StartCoroutine(Init());
    }

	private IEnumerator Init()
	{
        mapController.Init(this.OnPlayerTileClick, this.OnEnemyTileClick);
		yield return new WaitForEndOfFrame();

		InitUnits ();
	}

	private void InitUnits()
	{
        var layout = MapLayout.GetDefaultLayout();
        foreach(var playerPosition in layout.playerPositions)
        {
            var tile = mapController.GetTile(Const.Team.Player, playerPosition.Row, playerPosition.Column);
            var prefab = this.GetUnitPrefab(Const.Team.Player);
            this.CreateUnitOnTile(tile, prefab);
        }

        foreach(var enemyPosition in layout.enemyPositions)
        {
            var tile = mapController.GetTile(Const.Team.Enemy, enemyPosition.Row, enemyPosition.Column);
            var prefab = this.GetUnitPrefab(Const.Team.Enemy);
            this.CreateUnitOnTile(tile, prefab);
        }
	}

    private void CreateUnitOnTile(MapTile tile, GameObject prefab)
    {
        if(tile != null)
        {
            var position = tile.transform.position;
            var unit = Instantiate(prefab) as GameObject;
            unit.transform.position = position;
            unit.transform.SetParent(this.transform);

            var unitController = unit.GetComponent<UnitController>();
            unitController.onAnimationStateChange += OnUnitAnimationStateChange;
            tile.AssignUnit(unitController);
            unitController.RegisterDefaultRotation();

            // generate character
            unitController.SetCharacter(Character.Fighter());
        }
    }

    private GameObject GetUnitPrefab(Const.Team team)
    {
        return team == Const.Team.Player ? knightPrefab : slimePrefab;
    }

    private void OnUnitAnimationStateChange(bool isAnimating)
    {
        this.isAnimating = isAnimating;
    }

    private void OnPlayerTileClick(MapTile tileClicked)
    {
        if(isAnimating)
        {
            return;
        }

        // if a unit has been selectd before
        if(selectedPlayer != null)
        {
            var tileBefore = selectedPlayer.CurrentTile;
            var tileAfter = tileClicked;

            // if a different tile is clicked
            if(tileBefore != tileAfter)
            {
                tileBefore.SetSelected(false);
                StartCoroutine(selectedPlayer.MoveToTile(tileAfter));

                var unitOnTileClicked = tileAfter.CurrentUnit;

                // if there's another unit on tile clied
                if(unitOnTileClicked != null)
                {
                    StartCoroutine(unitOnTileClicked.MoveToTile(tileBefore));
                }
                tileBefore.AssignUnit(unitOnTileClicked);

                // assign the unit to the tile clicked and reset selectedPlayer pointer
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
    }

    private void OnEnemyTileClick(MapTile tileClicked)
    {
        if(isAnimating)
        {
            return;
        }

        if(selectedPlayer != null)
        {
            if(tileClicked.CurrentUnit != null)
            {
                StartCoroutine(selectedPlayer.AttackTile(tileClicked));
                selectedPlayer.CurrentTile.SetSelected(false);
                selectedPlayer = null;
            }
        }
    }
}
