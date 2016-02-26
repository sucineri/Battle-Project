using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UniRx;

public class TileController : MonoBehaviour
{
    [SerializeField] private LongTapObservableTrigger _longTapTrigger;
    [SerializeField] private GameObject _targetIndicator;
    [SerializeField] private Image _tileSprite;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _movementColor;
    [SerializeField] private Color _skillHighlightColor;
    [SerializeField] private Color _skillRadiusColor;

    private MapPosition _mapPosition;

    private event Action<MapPosition, bool> _onTileClick;

    public void Init(MapPosition position, Action<MapPosition, bool> onClick)
    {
        this._mapPosition = position;
        this._onTileClick = onClick;

        this._longTapTrigger.OnPointerUpAsObserable()
            .Subscribe(isLongTap =>{
                this.OnTileTap(isLongTap);
            });
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

        this._targetIndicator.SetActive((state & Tile.TileState.Target) == Tile.TileState.Target);
    }

    private void OnTileTap(bool isLongTap)
    {
        if (this._onTileClick != null)
        {
            this._onTileClick(this._mapPosition, isLongTap);
        }
    }
}
