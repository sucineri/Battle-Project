using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class MenuItem : MonoBehaviour
{
    [SerializeField] private Text _label;

    private Action<int> _onClick;
    private int _index = 0;

    public void Init(String title, int index, Action<int> onClick)
    {
        this._label.text = title;
        this._onClick = onClick;
        this._index = index;
    }

    public void OnItemClick()
    {
        if (this._onClick != null)
        {
            this._onClick(this._index);
        }
    }
}
