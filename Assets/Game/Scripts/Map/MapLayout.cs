using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapLayout
{
	public List<MapPosition> playerPositions = new List<MapPosition> ();
	public List<MapPosition> enemyPositions = new List<MapPosition> ();

	protected MapLayout ()
	{
	}

	public static MapLayout GetDefaultLayout ()
	{
		var layout = new MapLayout ();
		layout.playerPositions.Add (new MapPosition (1, 0));
		layout.playerPositions.Add (new MapPosition (1, 1));
		layout.playerPositions.Add (new MapPosition (1, 2));
		layout.playerPositions.Add (new MapPosition (1, 3));

		layout.enemyPositions.Add (new MapPosition (1, 0));
		layout.enemyPositions.Add (new MapPosition (1, 1));
		layout.enemyPositions.Add (new MapPosition (1, 2));
		layout.enemyPositions.Add (new MapPosition (1, 3));
		return layout;
	}
}
