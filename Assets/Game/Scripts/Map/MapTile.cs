using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class MapTile : MonoBehaviour
{

	[SerializeField] private Image tileSprite;
	[SerializeField] private Color defaultColor;
	[SerializeField] private Color selectedColor;

	public UnitControllerBase CurrentUnit { get; private set; }

	public int X { get; set; }

	public int Y { get; set; }

	public Const.Team Team { get; private set; }

	private event Action<MapTile> onTileClick;

	public void Init (int x, int y, Const.Team team, Action<MapTile> onClick)
	{
		this.X = x;
		this.Y = y;
		this.Team = team;
		this.onTileClick = onClick;
	}

	public void AssignUnit (UnitControllerBase unit)
	{
		CurrentUnit = unit;
		if (unit != null) {
			unit.AssignToTile (this);
		}
	}

	public void SetSelected (bool selected)
	{
		tileSprite.color = selected ? selectedColor : defaultColor;
	}

	public void OnClick ()
	{
		if (this.onTileClick != null) {
			this.onTileClick (this);
		}
	}
}
