using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TileController : MonoBehaviour
{

	[SerializeField] private Image _tileSprite;
	[SerializeField] private Color _defaultColor;
	[SerializeField] private Color _selectedColor;

	public BattleUnitController CurrentUnit { get; private set; }

	public int X { get; set; }

	public int Y { get; set; }

	public Const.Team Team { get; private set; }

	private MapPosition _mapPosition;

	private event Action<MapPosition> _onTileClick;

	public void Init(MapPosition position, Action<MapPosition> onClick)
	{
		this._mapPosition = position;
		this._onTileClick = onClick;
	}

	public void AssignUnit (BattleUnitController unit)
	{
		CurrentUnit = unit;
		if (unit != null) {
			unit.AssignToTile (this);
		}
	}

	public void OnTileStateChange(Tile.TileState state)
	{
		Debug.LogWarning ("State is " + (int)state);
	}

	public void SetSelected (bool selected)
	{
		_tileSprite.color = selected ? _selectedColor : _defaultColor;
	}

	public void OnClick ()
	{
		if (this._onTileClick != null) {
			this._onTileClick (this._mapPosition);
		}
	}
}
