using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class MapTile : MonoBehaviour {

	[SerializeField] private Image tileSprite;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color selectedColor;

    public UnitController CurrentUnit { get; private set; }

	private int row;
	private int column;
    private event Action<MapTile> onTileClick;

	public void Init(int row, int column, Action<MapTile> onClick)
	{
		this.row = row;
		this.column = column;
        this.onTileClick = onClick;
	}

    public void AssignUnit(UnitController unit)
    {
        CurrentUnit = unit;
        if(unit != null)
        {
            unit.AssignToTile(this);
        }
    }

    public void SetSelected(bool selected)
    {
        tileSprite.color = selected ? selectedColor : defaultColor;
    }

    public void OnClick()
    {
        if(this.onTileClick != null)
        {
            this.onTileClick(this);
        }
    }
}
