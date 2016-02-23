using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TileController : MonoBehaviour
{

    [SerializeField] private Image _tileSprite;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _movementColor;
    [SerializeField] private Color _skillHighlightColor;
    [SerializeField] private Color _skillRadiusColor;

    private MapPosition _mapPosition;

    private event Action<MapPosition> _onTileClick;

    public void Init(MapPosition position, Action<MapPosition> onClick)
    {
        this._mapPosition = position;
        this._onTileClick = onClick;
    }

    public void OnTileStateChange(Tile.TileState state)
    {
        if ((state & Tile.TileState.Selected) == Tile.TileState.Selected)
        {
            this._tileSprite.color = _selectedColor;
        }
        else if ((state & Tile.TileState.SkillHighlight) == Tile.TileState.SkillHighlight)
        {
            this._tileSprite.color = _skillHighlightColor;
        }
        else if ((state & Tile.TileState.SkillRadius) == Tile.TileState.SkillRadius)
        {
            this._tileSprite.color = _skillRadiusColor;
        }
        else if ((state & Tile.TileState.MovementHighlight) == Tile.TileState.MovementHighlight)
        {
            this._tileSprite.color = _movementColor;
        }
        else
        {
            this._tileSprite.color = _defaultColor;
        }
    }

    public void OnClick()
    {
        if (this._onTileClick != null)
        {
            this._onTileClick(this._mapPosition);
        }
    }
}
