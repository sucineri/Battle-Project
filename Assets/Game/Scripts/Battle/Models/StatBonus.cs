using UnityEngine;
using System.Collections;

public class StatBonus
{
    public Const.BasicStats Stat;
    public double Magnitude;
    public Const.StatBonusType Type;

    public StatBonus(Const.BasicStats stat, double magnitude, Const.StatBonusType type)
    {
        this.Stat = stat;
        this.Magnitude = magnitude;
        this.Type = type;
    }
}
