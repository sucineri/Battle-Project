using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SkillEffectAffinities 
{
    private Dictionary<Const.Affinities, double> _affinities = new Dictionary<Const.Affinities, double>();

    public void SetAffinity(Const.Affinities affinity, double value)
    {
        if (_affinities.ContainsKey(affinity))
        {
            _affinities[affinity] = value;
        }
        else
        {
            _affinities.Add(affinity, value);
        }
    }

    public Dictionary<Const.Affinities, double> GetNonZeroAffinities()
    {
        var dict = new Dictionary<Const.Affinities, double>();
        foreach (var kv in this._affinities)
        {
            if(kv.Value != 0d)
            {
                dict.Add(kv.Key, kv.Value);
            }
        }
        return dict;
    }

    public double GetAffinity(Const.Affinities key)
    {
        if (_affinities.ContainsKey(key))
        {
            return _affinities[key];
        }
        return 0;
    }
}
