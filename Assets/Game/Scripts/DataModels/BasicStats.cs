﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicStats
{
    private Dictionary<Const.BasicStats, double> _stats = new Dictionary<Const.BasicStats, double>();

    public BasicStats(double maxHp, double maxMp, double atk, double def, double agi, double wis, double mnd)
    {
        _stats.Add(Const.BasicStats.MaxHp, maxHp);
        _stats.Add(Const.BasicStats.MaxMp, maxMp);
        _stats.Add(Const.BasicStats.Attack, atk);
        _stats.Add(Const.BasicStats.Defense, def);
        _stats.Add(Const.BasicStats.Agility, agi);
        _stats.Add(Const.BasicStats.Wisdom, wis);
        _stats.Add(Const.BasicStats.Mind, mnd);
    }

    public double GetStats(Const.BasicStats key)
    {
        return _stats[key];
    }
}
