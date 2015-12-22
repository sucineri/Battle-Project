using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapService
{
	public List<MapPosition> GetMovablePositions(BattleCharacter actor, Dictionary<BattleCharacter, MapPosition> allCharacters, Dictionary<MapPosition, Tile> map)
	{
		var actorPosition = allCharacters [actor];
		var movement = actor.BaseCharacter.Movement;
		var occupiedPositions = allCharacters.Values.ToList ().FindAll( x => x != actorPosition );

		var list = map.Keys.ToList ().FindAll (x => x.Team == actor.Team && x.GetDistance (actorPosition) <= movement && !occupiedPositions.Contains(x) );
		return list;
	}

	public List<MapPosition> GeAffectedMapPositions(List<Cordinate> pattern, Dictionary<MapPosition, Tile> map, MapPosition targetedPosition, Vector2 mapSize)
	{
		var list = new List<MapPosition>();
		var team = targetedPosition.Team;
		foreach (var offset in pattern)
		{
			var newX = targetedPosition.X + offset.X;
			var newY = targetedPosition.Y + offset.Y;
			if (this.ValidatePosition (mapSize, newX, newY)) {
				list.Add(new MapPosition(newX, newY, team));
			}
		}
		return list;
	}

	private bool ValidatePosition(Vector2 mapSize, int x, int y)
	{
		return (x >= 0 && y >= 0 && x < mapSize.x && y < mapSize.y);
	}
}
