using System;
using System.Collections;

public class MapPosition
{
    public Const.Team Team;
    private Cordinate _cordinate;

    public int X { get { return this._cordinate.X; } }

    public int Y { get { return this._cordinate.Y; } }

    public MapPosition(int x, int y, Const.Team team)
    {
        this._cordinate = new Cordinate(x, y);
        this.Team = team;
    }

    public int GetDistance(MapPosition otherPosition)
    {
        return this.GetXDistance(otherPosition) + this.GetYDistance(otherPosition);
    }

    public int GetXDistance(MapPosition otherPosition)
    {
        if (this.Team != otherPosition.Team)
        {
            return this.X + otherPosition.X + 1;
        }
        else
        {
            return Math.Abs(this.X - otherPosition.X);
        }
    }

    public int GetYDistance(MapPosition otherPosition)
    {
        return Math.Abs(this.Y - otherPosition.Y);
    }

    public override bool Equals(System.Object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to MapPosition return false.
        MapPosition p = obj as MapPosition;
        if ((System.Object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (this.X == p.X) && (this.Y == p.Y) && (this.Team == p.Team);
    }

    public bool Equals(MapPosition p)
    {
        // If parameter is null return false:
        if ((object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (this.X == p.X) && (this.Y == p.Y) && (this.Team == p.Team);
    }

    public override int GetHashCode()
    {
        var result = 17;
        var prime = 31;

        result = prime * result + this.X;
        result = prime * result + this.Y;
        result = prime * result + (int)this.Team;
        return result;
    }

    public override string ToString()
    {
        return string.Format("[MapPosition: X={0}, Y={1}, Team={2}]", X, Y, Team.ToString());
    }
}
