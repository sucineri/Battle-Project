using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ActionMenu : MonoBehaviour {

	[SerializeField] private GameObject _menuPrefab;
	[SerializeField] private VerticalLayoutGroup _layout;

	private Action<UnitControllerBase> _onMoveSelect;
	private Action<UnitControllerBase, Skill> _onSkillSelect;
	private UnitControllerBase _unit;

	public void Init(Action<UnitControllerBase> onMoveSelect, Action<UnitControllerBase, Skill> onSkillSelect)
	{
		this._onMoveSelect = onMoveSelect;
		this._onSkillSelect = onSkillSelect;
	}


	public void Show(UnitControllerBase unit)
	{
		this.gameObject.SetActive (true);
		this._unit = unit;
		foreach (Transform child in _layout.transform) {
			Destroy (child.gameObject);
		}

		CreateMoveButton ();
		CreateSkillButtons (unit);
	}

	private void CreateMoveButton()
	{
		var menuItem = this.CreateMenuItem ();
		menuItem.Init ("Move", 0, MoveSelect);
	}

	private void CreateSkillButtons(UnitControllerBase unit)
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
