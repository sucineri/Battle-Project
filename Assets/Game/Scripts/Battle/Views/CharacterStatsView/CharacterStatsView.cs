using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterStatsView : MonoBehaviour {

    [SerializeField] private Text _nameLabel;
    [SerializeField] private Text _hpLabel;
    [SerializeField] private ResourceBar _hpBar;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Transform _statsContainer;
    [SerializeField] private GameObject _statCellPrefab;

    public void Init(BattleCharacter character)
    {
        foreach (Transform child in _statsContainer)
        {
            Destroy(child.gameObject);
        }

        this._nameLabel.text = character.Name;
        this._hpBar.Init(character.HpPercentage);

        this._hpLabel.text = string.Format("{0}/{1}", character.CurrentHp, character.MaxHp);

        this.CreateStatsCell(Const.Stats.Attack, character.BaseCharacter.Attack.ToString());
        this.CreateStatsCell(Const.Stats.Defense, character.BaseCharacter.Defense.ToString());
        this.CreateStatsCell(Const.Stats.Wisdom, character.BaseCharacter.Wisdom.ToString());
        this.CreateStatsCell(Const.Stats.Agility, character.BaseCharacter.Agility.ToString());
        this.CreateStatsCell(Const.Stats.Mind, character.BaseCharacter.Mind.ToString());
        this.CreateEmptyCell();

        this.CreateStatsCell(Const.Stats.Accuracy, string.Format("{0}%", character.BaseCharacter.Accuracy * 100d));
        this.CreateStatsCell(Const.Stats.Evasion, string.Format("{0}%", character.BaseCharacter.Evasion * 100d));
        this.CreateStatsCell(Const.Stats.Critical, string.Format("{0}%", character.BaseCharacter.Critical * 100d));

        this.gameObject.SetActive(true);
    }

    private void CreateEmptyCell()
    {
        var go = this.CreateCell();
        var cell = go.GetComponent<CharacterStatCell>();
        cell.Empty();
    }

    private void CreateStatsCell(Const.Stats stat, string value)
    {
        var go = this.CreateCell();

        var cell = go.GetComponent<CharacterStatCell>();
        cell.Init(stat, value);
    }

    private GameObject CreateCell()
    {
        var go = Instantiate(this._statCellPrefab) as GameObject;
        go.transform.SetParent(this._statsContainer, false);
        go.transform.localScale = Vector3.one;

        return go;
    }
}
