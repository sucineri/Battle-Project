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

    public override bool Equals(System.Object obj)
    {
        // If parameter is null or can not the same type return false.
        if ((obj == null) || !(obj is Cordinate))
        {
            return false;
        }
            
        Cordinate c = (Cordinate)obj;

        // Return true if the fields match:
        return (this.X == c.X) && (this.Y == c.Y);
    }

    public bool Equals(Cordinate c)
    {
        // If parameter is null return false:
        if ((object)c == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (this.X == c.X) && (this.Y == c.Y);
    }

    public override int GetHashCode()
    {
        var result = 17;
        var prime = 31;

        result = prime * result + this.X;
        result = prime * result + this.Y;
        return result;
    }

    public override string ToString()
    {
        return string.Format("[Cordinate: X={0}, Y={1}]", X, Y);
    }
}
