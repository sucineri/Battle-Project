using UnityEngine;
using System.Collections;

public class StatModifier
{
    public Const.Stats Stat;
    public double Magnitude;
    public Const.ModifierType Type;

    public StatModifier(Const.Stats stat, double magnitude, Const.ModifierType type)
    {
        this.Stat = stat;
        this.Magnitude = magnitude;
        this.Type = type;
    }
}
