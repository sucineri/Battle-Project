using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillService  
{

	public List<MapPosition> GetSkillAffectedMapPositions(Skill skill, Dictionary<MapPosition, Tile> map, MapPosition targetedPosition)
	{
		var list = new List<MapPosition>();
		var team = targetedPosition.Team;
		foreach (var offset in skill.SkillTarget.Pattern)
		{
			var newX = targetedPosition.X + offset.X;
			var newY = targetedPosition.Y + offset.Y;
			list.Add(new MapPosition(newX, newY, team));
		}
		return list;
	}
}
