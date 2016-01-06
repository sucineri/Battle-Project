using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapService
{
    public List<MapPosition> GetMovablePositions(BattleCharacter actor, List<BattleCharacter> allCharacters, Dictionary<MapPosition, Tile> map)
    {
        var occupiedPositions = new List<MapPosition>();
        foreach (var character in allCharacters)
        {
            if (character != actor)
            {
                occupiedPositions.AddRange(character.OccupiedMapPositions);
            }
        }

        var unOccupiedPositions = map.Keys.ToList().Except(occupiedPositions);

        var moveableTiles = new List<MapPosition>();
        foreach (var availablePosition in unOccupiedPositions)
        {
            if (this.CanCharacterMoveToPosition(actor, availablePosition, occupiedPositions, map))
            {
                moveableTiles.Add(availablePosition);
            }
        }

        return moveableTiles;
    }

    public bool CanCharacterMoveToPosition(BattleCharacter actor, MapPosition position, List<MapPosition> occupiedPositions, Dictionary<MapPosition, Tile> map)
    {
        var movement = actor.BaseCharacter.Movement;
        var actorPosition = actor.OccupiedMapPositions[0];
        if (position.Team == actor.Team && actorPosition.GetDistance(position) <= movement)
        {
            var tilesRequired = this.GeMapPositionsForPattern(actor.BaseCharacter.Shape, map, position);
            return (tilesRequired.Count == actor.BaseCharacter.Shape.Count) && (occupiedPositions.Intersect(tilesRequired).ToList().Count == 0);
        }
        return false;
    }

    public List<MapPosition> GeMapPositionsForPattern(List<Cordinate> pattern, Dictionary<MapPosition, Tile> map, MapPosition targetedPosition)
    {
        var list = new List<MapPosition>();
        var team = targetedPosition.Team;
        foreach (var offset in pattern)
        {
            var newX = targetedPosition.X + offset.X;
            var newY = targetedPosition.Y + offset.Y;
            if (this.ValidatePosition(map, newX, newY, team))
            {
                list.Add(new MapPosition(newX, newY, team));
            }
        }
        return list;
    }

    private bool ValidatePosition(Dictionary<MapPosition, Tile> map, int x, int y, Const.Team team)
    {
        var newPosition = new MapPosition(x, y, team);
        return map.ContainsKey(newPosition);
    }
}
