using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UnitControllerBase : MonoBehaviour
{

    [SerializeField]
    private UnitAnimationControllerBase _animationController;
    [SerializeField]
    private HpBar _hpBar;
    [SerializeField]
    private GameObject _damageTextPrefab;

    public MapTile CurrentTile { get; private set; }

    public Const.Team Team { get; private set; }

    public CharacterStats Character { get; private set; }

    public double TurnOrderWeight { get; set; }

	public Skill SelectedSkill { get; set; }

    public char Postfix { get; private set; }

    public string UnitName { get { return this.Character.Name + " " + Postfix; } }

    public bool IsDead { get { return this.Character.CurrentHp <= 0d; } }

    private float hpBarAnimationDuration = 0.5f;

    public void Init(Const.Team team, CharacterStats character, char postFix)
    {
        this.Team = team;
        this.Character = character;
        this.Postfix = postFix;
		this._hpBar.Init (this.Character.HpPercentage);
        this.TurnOrderWeight = BattleActionWeight.GetDefaultTurnOrderWeight(this);
    }

    public void AssignToTile(MapTile tile)
    {
        this.CurrentTile = tile;
        this._animationController.OnInit();
    }

    public float GetAttackPositionOffset(UnitControllerBase opponentUnit)
    {
        return this.Character.AttackDistance + opponentUnit.Character.SizeOffset;
    }

    public virtual IEnumerator MoveToPosition(Vector3 position, float distanceOffset)
    {
        yield return StartCoroutine(this._animationController.MoveTowards(position, distanceOffset));
    }

    public virtual IEnumerator AnimateAttack()
    {
        yield return StartCoroutine(this._animationController.PlayAttackAnimation());
    }

    public virtual IEnumerator MoveToTile(MapTile tile)
    {
        this.TurnOrderWeight += BattleActionWeight.GetMoveActionWeight(this);
        yield return StartCoroutine(_animationController.MoveTowards(tile.transform.position));
        _animationController.DefaultStance();
    }

    public virtual IEnumerator ReturnToBaseTile()
    {
        var position = this.CurrentTile.transform.position;
        yield return StartCoroutine(this._animationController.MoveTowards(position));
        this._animationController.DefaultStance();
    }

    public virtual IEnumerator TakeDamage(double damage, bool isLastHit)
    {
		this.ShowDamageText(damage);

		var hpRemaining = System.Math.Max (0d, this.Character.CurrentHp - damage);
        this.Character.CurrentHp = hpRemaining;
		var hpPercentage = this.Character.HpPercentage;
		StartCoroutine(this.AnimateHpChange(hpPercentage));
        
		var isDead = this.IsDead && isLastHit;
        yield return StartCoroutine(this._animationController.PlayDamagedAnimation(isDead));
    }

    public SkillComponentBase GetSelectedSkill()
    {
		var skill = this.Character.Skills [0];
		if (SelectedSkill != null) {
			skill = SelectedSkill;
		}

        // TODO: refactor these shit
		var prefab = Resources.Load (skill.PrefabPath);
		var go = Instantiate (prefab) as GameObject;
		var skillComp = go.GetComponent<SkillComponentBase> ();
		skillComp.InitWithSkill (skill);
		return skillComp;
    }

    // default AI attack
    public IEnumerator RunAI(List<UnitControllerBase> allUnits)
    {
        var targetTile = TargetLogic.GetTargetTile(this, allUnits);
        yield return StartCoroutine(this.GetSelectedSkill().PlaySkillSequence(this, targetTile));
    }

	public void PlayEffect(string path)
	{
		var prefab = Resources.Load (path);
		var go = Instantiate (prefab) as GameObject;
		go.transform.SetParent (this.transform);
		go.transform.localScale = Vector3.one;
		go.transform.localPosition = Vector3.zero;
		go.SetActive (true);
	}

    private void ShowDamageText(double damage)
    {
        var go = Instantiate(this._damageTextPrefab) as GameObject;
        var damageText = go.GetComponent<DamageText>();
        go.transform.SetParent(this.transform);
        go.transform.localPosition = this._damageTextPrefab.transform.localPosition;
        go.transform.localScale = this._damageTextPrefab.transform.localScale;
        go.SetActive(true);
        damageText.ShowDamage(damage);
    }

    private IEnumerator AnimateHpChange(float hpPercentage)
    {
		yield return StartCoroutine(_hpBar.AnimateValueChange(hpPercentage, hpBarAnimationDuration));
        if (this.IsDead)
        {
            _hpBar.gameObject.SetActive(false);
        }
    }
}
