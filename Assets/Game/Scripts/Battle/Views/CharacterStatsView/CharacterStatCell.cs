using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterStatCell : MonoBehaviour {

    [SerializeField] private Text _name;
    [SerializeField] private Text _value;
    [SerializeField] private Color _debuffedColor = Color.red;
    [SerializeField] private Color _buffedColor = Color.green;
    [SerializeField] private Color _normalColor = Color.white;

    public void Init(Const.Stats stat, string value, CharacterStatBuffState state)
    {
        this._name.text = stat.ToString() + ":";
        this._value.text = value;

        var color = this._normalColor;
        switch (state)
        {
            case CharacterStatBuffState.Buffed:
                color = this._buffedColor;
                break;
            case CharacterStatBuffState.Debuffed:
                color = this._debuffedColor;
                break;
        }
        this._value.color = color;
    }

    public void Empty()
    {
        this._name.text = "";
        this._value.text = "";
    }
}
