using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapLayout
{
    public List<MapPosition> playerPositions = new List<MapPosition>();
    public List<MapPosition> enemyPositions = new List<MapPosition>();

    protected MapLayout() {}

    public static MapLayout GetDefaultLayout()
    {
        var layout = new MapLayout();
        layout.playerPositions.Add(new MapPosition(0, 1));
        layout.playerPositions.Add(new MapPosition(1, 1));
        layout.playerPositions.Add(new MapPosition(2, 1));
        layout.playerPositions.Add(new MapPosition(3, 1));

        layout.enemyPositions.Add(new MapPosition(0, 1));
        layout.enemyPositions.Add(new MapPosition(1, 1));
        layout.enemyPositions.Add(new MapPosition(2, 1));
        layout.enemyPositions.Add(new MapPosition(3, 1));
        return layout;
    }
}
