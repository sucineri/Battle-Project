using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicStats
{
    private Dictionary<Const.Stats, double> _stats = new Dictionary<Const.Stats, double>();

    public BasicStats()
    {
        _stats.Add(Const.Stats.MaxHp, 0d);
        _stats.Add(Const.Stats.MaxMp, 0d);
        _stats.Add(Const.Stats.Attack, 0d);
        _stats.Add(Const.Stats.Defense, 0d);
        _stats.Add(Const.Stats.Agility, 0d);
        _stats.Add(Const.Stats.Wisdom, 0d);
        _stats.Add(Const.Stats.Mind, 0d);
        _stats.Add(Const.Stats.Critical, 0d);
        _stats.Add(Const.Stats.Accuracy, 0d);
        _stats.Add(Const.Stats.Evasion, 0d);
    }

    public BasicStats(double maxHp, double maxMp, double atk, double def, double agi, double wis, double mnd, double crit, double acc, double eva)
    {
        _stats.Add(Const.Stats.MaxHp, maxHp);
        _stats.Add(Const.Stats.MaxMp, maxMp);
        _stats.Add(Const.Stats.Attack, atk);
        _stats.Add(Const.Stats.Defense, def);
        _stats.Add(Const.Stats.Agility, agi);
        _stats.Add(Const.Stats.Wisdom, wis);
        _stats.Add(Const.Stats.Mind, mnd);
        _stats.Add(Const.Stats.Critical, crit);
        _stats.Add(Const.Stats.Accuracy, acc);
        _stats.Add(Const.Stats.Evasion, eva);

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
        return _stats[key];
    }
}
