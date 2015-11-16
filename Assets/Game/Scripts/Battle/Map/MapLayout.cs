using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapLayout
{
	public List<Cordinate> playerPositions = new List<Cordinate> ();
	public List<Cordinate> enemyPositions = new List<Cordinate> ();

	protected MapLayout ()
	{
	}

	public static MapLayout GetDefaultLayout ()
	{
		var layout = new MapLayout ();
		layout.playerPositions.Add (new Cordinate (1, 0));
		layout.playerPositions.Add (new Cordinate (1, 1));
		layout.playerPositions.Add (new Cordinate (1, 2));
		layout.playerPositions.Add (new Cordinate (1, 3));

		layout.enemyPositions.Add (new Cordinate (1, 0));
		layout.enemyPositions.Add (new Cordinate (1, 1));
		layout.enemyPositions.Add (new Cordinate (1, 2));
		layout.enemyPositions.Add (new Cordinate (1, 3));
		return layout;
	}
}
