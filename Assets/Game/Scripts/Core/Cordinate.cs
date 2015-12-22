using UnityEngine;
using System.Collections;

public struct Cordinate
{
	public int X { get; set; }
    public int Y { get; set; }

    public Cordinate(int x, int y)
    {
		this.X = x;
        this.Y = y;
    }
}
