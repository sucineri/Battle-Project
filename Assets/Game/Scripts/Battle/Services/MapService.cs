using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapService
{
    public List<MapPosition> GetUnoccupiedTiles(List<BattleCharacter> allCharacters, Dictionary<MapPosition, Tile> map)
    {
        var occupiedPositions = new List<MapPosition>();
        foreach (var character in allCharacters)
        {
            occupiedPositions.AddRange(character.OccupiedMapPositions);
        }

        return map.Keys.ToList().Except(occupiedPositions).ToList();
    }

    public List<MapPosition> GetMovablePositions(BattleCharacter actor, List<BattleCharacter> allCharacters, Dictionary<MapPosition, Tile> map)
    {
        var unOccupiedPositions = this.GetUnoccupiedTiles(allCharacters, map);
        unOccupiedPositions = unOccupiedPositions.Union(actor.OccupiedMapPositions).ToList();

        var moveableTiles = new List<MapPosition>();
        foreach (var availablePosition in unOccupiedPositions)
        {
            if (this.CanCharacterMoveToPosition(actor, availablePosition, unOccupiedPositions, map))
            {
                moveableTiles.Add(availablePosition);
            }
        }

        return moveableTiles;
    }

    public bool CanCharacterMoveToPosition(BattleCharacter actor, MapPosition position, List<MapPosition> availablePositions, Dictionary<MapPosition, Tile> map)
    {
        var movement = actor.BaseCharacter.Movement;
        var actorPosition = actor.BasePosition;

        if (position.Team == actor.Team && actorPosition.GetDistance(position) <= movement)
        {
            var tilesRequired = this.RequestPositions(actor.BaseCharacter.PatternShape.Shape, map, position, availablePositions);

            return (tilesRequired.Count == actor.BaseCharacter.PatternShape.Shape.Count);
        }
        return false;
    }

    public List<MapPosition> RequestPositions(List<Cordinate> shape, Dictionary<MapPosition, Tile> map, MapPosition basePosition, List<MapPosition> availableTiles)
    {
        var list = new List<MapPosition>();
        var team = basePosition.Team;
        foreach (var offset in shape)
        {
            var newX = basePosition.X + offset.X;
            var newY = basePosition.Y + offset.Y;
            var newPosition = new MapPosition(newX, newY, team);
            if (map.ContainsKey(newPosition) && availableTiles.Contains(newPosition))
            {
                list.Add(new MapPosition(newX, newY, team));
            }
        }
        return list;
    }

    public List<MapPosition> GetValidMapPositionsForSkill(Skill skill, BattleCharacter actor, Dictionary<MapPosition, Tile> map, List<BattleCharacter> allBattleCharacters)
    {
        var basePosition = actor.BasePosition;
        var targeting = skill.SkillTargetArea;
        if (targeting.TargetGroup == Const.SkillTargetGroup.Opponent)
        {
            // creates a fake position
            var oppositeTeam = actor.Team == Const.Team.Enemy ? Const.Team.Player : Const.Team.Enemy;
            basePosition = new MapPosition(-(basePosition.X + 1), basePosition.Y, oppositeTeam);
        }

        var validPositions = this.GeMapPositionsForPattern(targeting.Pattern, map, basePosition);
        return validPositions;
    }

    public List<MapPosition> GeMapPositionsForPattern(Pattern pattern, Dictionary<MapPosition, Tile> map, MapPosition basePosition)
    {
        var list = new List<MapPosition>();
        var team = basePosition.Team;
        var sameTeamPositions = map.Keys.ToList().FindAll(x => x.Team == team);

        foreach (var mapPosition in sameTeamPositions)
        {
            if (this.IsPositionCoveredByPattern(pattern, mapPosition, basePosition))
            {
                list.Add(mapPosition);
            }
        }
        return list;
    }

    private bool IsPositionCoveredByPattern(Pattern pattern, MapPosition position, MapPosition basePosition)
    {
        var offsetX = position.X - basePosition.X;
        var offsetY = position.Y - basePosition.Y;
        if (pattern.IsWholeGrid)
        {
            return true;
        }
        else if (pattern.WholeRows.Contains(offsetY))
        {
            return true;
        }
        else if (pattern.WholeColumns.Contains(offsetX))
        {
            return true;
        }
        else
        {
            return pattern.Shape.Contains(new Cordinate(offsetX, offsetY));
        }
    }

    private bool ValidatePosition(Dictionary<MapPosition, Tile> map, int x, int y, Const.Team team)
    {
        var newPosition = new MapPosition(x, y, team);
        return map.ContainsKey(newPosition);
    }
}
