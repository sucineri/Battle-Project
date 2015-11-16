using UnityEngine;
using System.Collections;

public class Const
{
	public enum Team
	{
		Player = 0,
		Enemy = 1
	}

	public enum SkillTargetGroup
	{
		None,
		Ally,
		Opponent,
		Self
	}

	public enum SkillTargetType
	{
		Unit,
		Tile
	}

	public enum BasicStats
	{
		MaxHp,
		MaxMp,
		Attack,
		Defense,
		Agility,
		Wisdom
	}

	public const string TileKey = "Tile";

	public static string GetTileKey (int x, int y)
	{
		return string.Format ("{0}_{1}_{2}", Const.TileKey, x, y);
	}
}
