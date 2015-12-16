using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ActionMenu : MonoBehaviour {

	[SerializeField] private GameObject _menuPrefab;
	[SerializeField] private VerticalLayoutGroup _layout;
	[SerializeField] private GameObject _cancelButton;

	private Action<BattleUnitController> _onMoveSelect;
	private Action<BattleUnitController, Skill> _onSkillSelect;
	private BattleAction _onCancel;
	private BattleUnitController _unit;

	public void OnBattlePhaseChange(BattleManager.BattlePhase battlePhase)
	{
		
	}

	public void Init(Action<BattleUnitController> onMoveSelect, Action<BattleUnitController, Skill> onSkillSelect, BattleAction onCancel)
	{
		this._onMoveSelect = onMoveSelect;
		this._onSkillSelect = onSkillSelect;
		this._onCancel = onCancel;
	}

	public void CreateMenu(BattleUnitController unit)
	{
		this.ShowMenu (true);
		this.ShowCancel (false);
		this._unit = unit;
		foreach (Transform child in _layout.transform) {
			Destroy (child.gameObject);
		}

		CreateMoveButton ();
		CreateSkillButtons (unit);
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
		if(this._onCancel != null)
		{
//			this._onCancel ();
		}
	}

	private void CreateMoveButton()
	{
		var menuItem = this.CreateMenuItem ();
		menuItem.Init ("Move", 0, MoveSelect);
	}

	private void CreateSkillButtons(BattleUnitController unit)
	{
		for (int i = 0; i < unit.Character.Skills.Count; ++i) {
			var skill = unit.Character.Skills [i];
			var menuItem = this.CreateMenuItem ();
			menuItem.Init (skill.Name, i, this.SkillTargetSelect);
		}
	}

	private void MoveSelect(int index)
	{
		if (this._onMoveSelect != null) {
			this._onMoveSelect (this._unit);
		}
	}

	private void SkillTargetSelect(int skillIndex)
	{
		if (this._unit != null && this._unit.Character.Skills.Count > skillIndex) {
			var skill = this._unit.Character.Skills [skillIndex];
			if (this._onSkillSelect != null) {
				this._onSkillSelect (this._unit, skill);
			}
		}
	}

	private MenuItem CreateMenuItem()
	{
		var go = Instantiate (this._menuPrefab) as GameObject;
		go.transform.SetParent (this._layout.transform);
		go.transform.localScale = Vector3.one;
		var menuItem = go.GetComponent<MenuItem> ();
		return menuItem;
	}
}
