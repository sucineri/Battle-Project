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

        this.CreateCellWithDecimalValue(character, Const.Stats.Attack);
        this.CreateCellWithDecimalValue(character, Const.Stats.Defense);
        this.CreateCellWithDecimalValue(character, Const.Stats.Wisdom);
        this.CreateCellWithDecimalValue(character, Const.Stats.Agility);
        this.CreateCellWithDecimalValue(character, Const.Stats.Mind);
        this.CreateEmptyCell();

        this.CreateCellWithPercentageValue(character, Const.Stats.Accuracy);
        this.CreateCellWithPercentageValue(character, Const.Stats.Evasion);
        this.CreateCellWithPercentageValue(character, Const.Stats.Critical);

        this.gameObject.SetActive(true);
    }

    private void CreateEmptyCell()
    {
        var go = this.CreateCell();
        var cell = go.GetComponent<CharacterStatCell>();
        cell.Empty();
    }

    private void CreateCellWithDecimalValue(BattleCharacter character, Const.Stats stat)
    {
        this.CreateStatsCell(stat, character.GetStat(stat).ToString(), character.GetStatBuffState(stat));
    }

    private void CreateCellWithPercentageValue(BattleCharacter character, Const.Stats stat)
    {
        this.CreateStatsCell(stat, string.Format("{0}%", character.GetStat(stat) * 100d), character.GetStatBuffState(stat));
    }

    private void CreateStatsCell(Const.Stats stat, string value, CharacterStatBuffState state)
    {
        var go = this.CreateCell();

        var cell = go.GetComponent<CharacterStatCell>();
        cell.Init(stat, value, state);
    }

    private GameObject CreateCell()
    {
        var go = Instantiate(this._statCellPrefab) as GameObject;
        go.transform.SetParent(this._statsContainer, false);
        go.transform.localScale = Vector3.one;

        return go;
    }
}
