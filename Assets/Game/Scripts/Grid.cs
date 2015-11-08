using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	[SerializeField] private GameObject tilePrefab;
    [SerializeField] private GridLayoutGroup layout;
	[SerializeField] private int numberOfRows;
    [SerializeField] private int numberOfColumns;
	[SerializeField] private Const.Team team;

	public Dictionary<string, MapTile> Init(System.Action<MapTile> onTileClick)
	{
        Dictionary<string, MapTile> dict = new Dictionary<string, MapTile>();
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = numberOfColumns;
        for (int i = 0; i < this.numberOfRows; i++)
        {
            for (int j = 0; j < this.numberOfColumns; j++)
            {
                var tile = Instantiate(tilePrefab) as GameObject;
                tile.transform.SetParent(layout.transform);
                tile.transform.localScale = Vector3.one;
                tile.transform.localEulerAngles = Vector3.zero;
                tile.transform.localPosition = Vector3.zero;

                var mapTile = tile.GetComponent<MapTile>();
                mapTile.Init(i, j, onTileClick);
                tile.name = Const.GetTileKey(i, j);
                dict.Add(tile.name, mapTile);
            }
        }
        return dict;
	}
}
