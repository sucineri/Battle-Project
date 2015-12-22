using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurnOrderCell : MonoBehaviour
{

    [SerializeField] private RawImage portrait;
    [SerializeField] private Text postfixLabel;

    public void Setup(BattleCharacter character)
    {
        var image = Resources.Load(character.BaseCharacter.PortraitPath) as Texture;
        portrait.texture = image;
        postfixLabel.text = character.Postfix.ToString();
    }
}
