using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapLayout
{
    public List<MapPosition> positions = new List<MapPosition>();

    protected MapLayout()
    {
    }

    public static MapLayout GetDefaultLayout()
    {
        var layout = new MapLayout();
        layout.positions.Add(new MapPosition(1, 0, Const.Team.Player));
        layout.positions.Add(new MapPosition(1, 1, Const.Team.Player));
        layout.positions.Add(new MapPosition(1, 2, Const.Team.Player));
        layout.positions.Add(new MapPosition(1, 3, Const.Team.Player));

        layout.positions.Add(new MapPosition(1, 0, Const.Team.Enemy));
        layout.positions.Add(new MapPosition(1, 1, Const.Team.Enemy));
        layout.positions.Add(new MapPosition(1, 2, Const.Team.Enemy));
        layout.positions.Add(new MapPosition(1, 3, Const.Team.Enemy));
        return layout;
    }

    public static MapLayout BossLayout()
    {
        var layout = new MapLayout();
        layout.positions.Add(new MapPosition(1, 0, Const.Team.Player));
        layout.positions.Add(new MapPosition(0, 1, Const.Team.Player));
        layout.positions.Add(new MapPosition(0, 2, Const.Team.Player));
        layout.positions.Add(new MapPosition(2, 3, Const.Team.Player));

        layout.positions.Add(new MapPosition(1, 1, Const.Team.Enemy));
        return layout;
    }
}
