using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterStatCell : MonoBehaviour {

    [SerializeField] private Text _name;
    [SerializeField] private Text _value;

    public void Init(Const.BasicStats stat, string value)
    {
        this._name.text = stat.ToString() + ":";
        this._value.text = value;
    }

    public void Empty()
    {
        this._name.text = "";
        this._value.text = "";
    }
}
