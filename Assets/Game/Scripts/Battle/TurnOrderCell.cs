using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurnOrderCell : MonoBehaviour {

    [SerializeField] private RawImage portrait;
    [SerializeField] private Text postfixLabel;

    public void Setup(UnitControllerBase unit)
    {
        var image = Resources.Load(unit.Character.PortraitPath) as Texture;
        portrait.texture = image;
        postfixLabel.text = unit.Postfix.ToString();
    }
}
