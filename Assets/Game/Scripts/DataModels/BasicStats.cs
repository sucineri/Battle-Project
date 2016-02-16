using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicStats
{
    private Dictionary<Const.BasicStats, double> _stats = new Dictionary<Const.BasicStats, double>();

    public BasicStats()
    {
        _stats.Add(Const.BasicStats.MaxHp, 0d);
        _stats.Add(Const.BasicStats.MaxMp, 0d);
        _stats.Add(Const.BasicStats.Attack, 0d);
        _stats.Add(Const.BasicStats.Defense, 0d);
        _stats.Add(Const.BasicStats.Agility, 0d);
        _stats.Add(Const.BasicStats.Wisdom, 0d);
        _stats.Add(Const.BasicStats.Mind, 0d);
        _stats.Add(Const.BasicStats.Critical, 0d);
        _stats.Add(Const.BasicStats.Accuracy, 0d);
        _stats.Add(Const.BasicStats.Evasion, 0d);
    }

    public BasicStats(double maxHp, double maxMp, double atk, double def, double agi, double wis, double mnd, double crit, double acc, double eva)
    {
        _stats.Add(Const.BasicStats.MaxHp, maxHp);
        _stats.Add(Const.BasicStats.MaxMp, maxMp);
        _stats.Add(Const.BasicStats.Attack, atk);
        _stats.Add(Const.BasicStats.Defense, def);
        _stats.Add(Const.BasicStats.Agility, agi);
        _stats.Add(Const.BasicStats.Wisdom, wis);
        _stats.Add(Const.BasicStats.Mind, mnd);
        _stats.Add(Const.BasicStats.Critical, crit);
        _stats.Add(Const.BasicStats.Accuracy, acc);
        _stats.Add(Const.BasicStats.Evasion, eva);

    }

    public void SetStat(Const.BasicStats stat, double value)
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

    public double GetStats(Const.BasicStats key)
    {
        return _stats[key];
    }
}
