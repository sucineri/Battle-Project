using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class MapTile : MonoBehaviour
{

	[SerializeField] private Image _tileSprite;
	[SerializeField] private Color _defaultColor;
	[SerializeField] private Color _selectedColor;

	public UnitControllerBase CurrentUnit { get; private set; }

	public int X { get; set; }

	public int Y { get; set; }

	public Const.Team Team { get; private set; }

	private event Action<MapTile> _onTileClick;

	public void Init (int x, int y, Const.Team team, Action<MapTile> onClick)
	{
		this.X = x;
		this.Y = y;
		this.Team = team;
		this._onTileClick = onClick;
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
		_tileSprite.color = selected ? _selectedColor : _defaultColor;
	}

	public void OnClick ()
	{
		if (this._onTileClick != null) {
			this._onTileClick (this);
		}
	}
}
