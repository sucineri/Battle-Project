using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Affinity 
{
    private Dictionary<Const.Affinities, double> _affinities = new Dictionary<Const.Affinities, double>();

    public double Healing { get { return this.GetAffinity(Const.Affinities.Healing); } }

    public Affinity()
    {
        _affinities.Add(Const.Affinities.Physical, 0d);
        _affinities.Add(Const.Affinities.Healing, 0d);
        _affinities.Add(Const.Affinities.Lightning, 0d);
    }

    public Affinity(double physical, double heal, double lightning)
    {
        _affinities.Add(Const.Affinities.Physical, physical);
        _affinities.Add(Const.Affinities.Healing, heal);
        _affinities.Add(Const.Affinities.Lightning, lightning);
    }

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

    public List<KeyValuePair<Const.Affinities, double>> GetNonZeroAffinities()
    {
        var list = new List<KeyValuePair<Const.Affinities, double>>();
        foreach (var kv in this._affinities)
        {
            if(kv.Value != 0d)
            {
                list.Add(kv);
            }
        }
        return list;
    }

    public double GetAffinity(Const.Affinities key)
    {
        return _affinities[key];
    }
}
