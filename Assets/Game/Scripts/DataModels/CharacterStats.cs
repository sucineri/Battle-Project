using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CharacterStats
{
    private Dictionary<Const.Stats, double> _stats = new Dictionary<Const.Stats, double>();

    public CharacterStats()
    {
        foreach (var stat in Enum.GetValues(typeof(Const.Stats)).Cast<Const.Stats>())
        {
            if (stat != Const.Stats.Invalid)
            {
                // Init all stats to be 0 except healing to be 2
                var baseStat = stat == Const.Stats.HealingResistance ? 2d : 0d;
                _stats.Add(stat, baseStat);
            }
        }
    }

    public void SetStat(Const.Stats stat, double value)
    {
        if (_stats.ContainsKey(stat))
        {
            _stats[stat] = value;
        }
        else
        {
            _stats.Add(stat, value);
        }
    }

    public double GetStats(Const.Stats key)
    {
        if (_stats.ContainsKey(key))
        {
            return _stats[key];
        }
        return 0d;
    }
}
