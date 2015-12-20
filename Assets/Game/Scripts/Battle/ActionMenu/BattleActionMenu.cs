using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class BattleActionMenu : MonoBehaviour {

	[SerializeField] private GameObject _menuPrefab;
	[SerializeField] private VerticalLayoutGroup _layout;
	[SerializeField] private GameObject _cancelButton;

	private BattleModel _battleModel;

	public void Init(BattleModel battleModel)
	{
		this._battleModel = battleModel;
		this._battleModel.onBattlePhaseChange += this.OnBattlePhaseChange;
	}

	public void CreateMenu(BattleCharacter character)
	{
		this.ShowMenu (true);
		this.ShowCancel (false);
		foreach (Transform child in _layout.transform) {
			Destroy (child.gameObject);
		}
			
		CreateSkillButtons (character);
	}

	public void ShowMenu(bool show)
	{
		this._layout.gameObject.SetActive (show);
	}

	public void ShowCancel(bool show)
	{
		this._cancelButton.SetActive (show);
	}

	public void OnCancel()
	{
		this._battleModel.CancelLastSelection ();
	}

	private void CreateSkillButtons(BattleCharacter character)
	{
		for (int i = 0; i < character.BaseCharacter.Skills.Count; ++i) {
			var skill = character.BaseCharacter.Skills [i];
			var menuItem = this.CreateMenuItem ();
			menuItem.Init (skill.Name, i, this.OnSkillSelect);
		}
	}

	private void OnSkillSelect(int skillIndex)
	{
		this._battleModel.SelecteSkill(skillIndex);
	}

	private MenuItem CreateMenuItem()
	{
		var go = Instantiate (this._menuPrefab) as GameObject;
		go.transform.SetParent (this._layout.transform);
		go.transform.localScale = Vector3.one;
		var menuItem = go.GetComponent<MenuItem> ();
		return menuItem;
	}

	private void OnBattlePhaseChange(BattleModel.BattlePhase battlePhase)
	{
		switch (battlePhase) {
			case BattleModel.BattlePhase.ActionSelect:
				this.CreateMenu (_battleModel.CurrentActor);
				this.ShowCancel (false);
				break;
			case BattleModel.BattlePhase.TargetSelect:
				this.ShowMenu (false);
				this.ShowCancel (true);
				break;
			default:
				this.ShowCancel (false);
				this.ShowMenu (false);
				break;
		}
	}

	void OnDestroy()
	{
		this._battleModel.onBattlePhaseChange -= this.OnBattlePhaseChange;
	}
}
