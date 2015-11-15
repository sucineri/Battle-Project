using UnityEngine;
using System.Collections;

public class Const
{
	public enum Team
	{
		Player = 0,
		Enemy = 1
	}

	public const string TileKey = "Tile";

	public static string GetTileKey (int x, int y)
	{
		return string.Format ("{0}_{1}_{2}", Const.TileKey, x, y);
	}
}
