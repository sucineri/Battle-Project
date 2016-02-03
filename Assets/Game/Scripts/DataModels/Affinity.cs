using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Affinity 
{
    private Dictionary<Const.Affinities, double> _affinities = new Dictionary<Const.Affinities, double>();

    public double Healing { get { return this.GetAffinity(Const.Affinities.Healing); } }

    public Affinity(double heal)
    {
        _affinities.Add(Const.Affinities.Healing, heal);
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
