using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapService
{
    public List<MapPosition> GetMovablePositions(BattleCharacter actor, List<BattleCharacter> allCharacters, Dictionary<MapPosition, Tile> map)
    {
        var actorPosition = actor.OccupiedMapPositions;
        var movement = actor.BaseCharacter.Movement;

        var occupiedPositions = new List<MapPosition>();
        foreach (var character in allCharacters)
        {
            if (character != actor)
            {
                occupiedPositions.Add(character.OccupiedMapPositions);
            }
        }

        var list = map.Keys.ToList().FindAll(x => x.Team == actor.Team && x.GetDistance(actorPosition) <= movement && !occupiedPositions.Contains(x));
        return list;
    }

    public List<MapPosition> GeAffectedMapPositions(List<Cordinate> pattern, Dictionary<MapPosition, Tile> map, MapPosition targetedPosition)
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
