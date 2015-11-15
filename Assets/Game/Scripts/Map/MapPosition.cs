using UnityEngine;
using System.Collections;

public struct MapPosition
{
	public int X { get; set; }
    public int Y { get; set; }

    public MapPosition(int x, int y)
    {
		this.X = x;
        this.Y = y;
    }
}
