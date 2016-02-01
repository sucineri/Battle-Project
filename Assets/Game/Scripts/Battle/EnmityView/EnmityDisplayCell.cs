using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnmityDisplayCell : MonoBehaviour 
{

    [SerializeField] private RawImage _portrait;
    [SerializeField] private Text _postfixLabel;
    [SerializeField] private Text _enmityLabel;

    public void Setup(BattleEnmity.EnmityTarget characterEnmity)
    {
        var character = characterEnmity.Character;
        var image = Resources.Load(character.BaseCharacter.PortraitPath) as Texture;
        _portrait.texture = image;
        _postfixLabel.text = character.Postfix.ToString();
        _enmityLabel.text = characterEnmity.EnmityLevel.ToString();
    }
}
