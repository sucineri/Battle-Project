using UnityEngine;
using System.Collections;

public struct MapPosition
{
    public int Row { get; set; }
    public int Column { get; set; }

    public MapPosition(int row, int column)
    {
        this.Row = row;
        this.Column = column;
    }
}
