using UnityEngine;
using System.Collections;

public class StatModifier
{
    public Const.BasicStats Stat;
    public double Magnitude;
    public Const.ModifierType Type;

    public StatModifier(Const.BasicStats stat, double magnitude, Const.ModifierType type)
    {
        this.Stat = stat;
        this.Magnitude = magnitude;
        this.Type = type;
    }
}
