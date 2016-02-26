using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BattleView : MonoBehaviour
{
    [SerializeField] private MapView _mapView;

    private Dictionary<BattleCharacter, BattleUnitController> _battleUnits = new Dictionary<BattleCharacter, BattleUnitController>();
    private Dictionary<MapPosition, TileController> _mapTiles = new Dictionary<MapPosition, TileController>();

    public void InitMap(int numberOfRows, int numberOfColumns, Action<MapPosition, bool> onTileClick)
    {
        foreach (var kv in this._mapView.InitGrids(numberOfRows, numberOfColumns, onTileClick))
        {
            this._mapTiles.Add(kv.Key, kv.Value);
        }
    }

    public IEnumerator MoveUnitToMapPosition(BattleCharacter character, List<MapPosition> occupiedPositions)
    {
        var occupiedTiles = this.GetTiles(occupiedPositions);
        var targetPosition = this.GetCenterPosition(occupiedTiles);

        var unitController = this._battleUnits[character];
        if (unitController != null)
        {
            yield return StartCoroutine(unitController.MoveToPosition(targetPosition));
        }
    }

    public BattleUnitController GetBattleUnit(BattleCharacter character)
    {
        return this._battleUnits[character];
    }

    public TileController GetTileAtMapPosition(MapPosition mapPosition)
    {
        return this._mapTiles[mapPosition];
    }

    public void BindTileController(MapPosition mapPosition, Tile tile)
    {
        var controller = this._mapTiles[mapPosition];
        tile.onStateChange += controller.OnTileStateChange;
    }

    public void SpawnUnit(BattleCharacter character)
    {
        var tiles = this.GetTiles(character.OccupiedMapPositions);
        var position = this.GetCenterPosition(tiles);
        var battleUnit = BattleUnitFactory.CreateBattleUnit(character);
        battleUnit.transform.position = position;

        if (character.Team == Const.Team.Enemy)
        {
            battleUnit.transform.Rotate(new Vector3(0f, 180f, 0f));
        }

        battleUnit.transform.SetParent(this.transform);
        battleUnit.Init(character);
        this._battleUnits.Add(character, battleUnit);
    }

    public IEnumerator PlaySkillAnimation(BattleActionResult actionResult)
    {
        var skillController = SkillControllerFactory.CreateSkillController(actionResult.skill);
        if (skillController != null)
        {
            var actor = actionResult.actor;
            var actorController = this.GetBattleUnit(actor);
            yield return StartCoroutine(skillController.PlaySkillSequence(actorController, this, actionResult));
        }
        yield return null;
    }
   
    private List<TileController> GetTiles(List<MapPosition> positions)
    {
        var tiles = new List<TileController>();
        foreach (var position in positions)
        {
            tiles.Add(this.GetTileAtMapPosition(position));
        }
        return tiles;
    }

    private Vector3 GetCenterPosition(List<TileController> tiles)
    {
        var minX = float.MaxValue;
        var minY = float.MaxValue;
        var minZ = float.MaxValue;
        var maxX = float.MinValue;
        var maxY = float.MinValue;
        var maxZ = float.MinValue;

        foreach (var tileController in tiles)
        {
            var position = tileController.transform.position;

            minX = Mathf.Min(minX, position.x);
            minY = Mathf.Min(minY, position.y);
            minZ = Mathf.Min(minZ, position.z);
            maxX = Mathf.Max(maxX, position.x);
            maxY = Mathf.Max(maxY, position.y);
            maxZ = Mathf.Max(maxZ, position.z);
        }

        var minPosition = new Vector3(minX, minY, minZ);
        var maxPosition = new Vector3(maxX, maxY, maxZ);

        var centerPosition = Vector3.Lerp(minPosition, maxPosition, 0.5f);

        return centerPosition;
    }
}
