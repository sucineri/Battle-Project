using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class BattleUnitController : MonoBehaviour
{

    [SerializeField] protected BattleCharacterView characterView;
    [SerializeField] protected ResourceBar hpBar;
    [SerializeField] protected GameObject damageTextPrefab;
	[SerializeField] protected float regularMovementSpeed;
	[SerializeField] protected float attackMovementSpeed;
	[SerializeField] protected float attackDistanceOffset;
	[SerializeField] protected float bodySizeOffset;

//    public TileController CurrentTile { get; private set; }
//
//    public Const.Team Team { get; private set; }
//
//    public CharacterStats Character { get; private set; }
//
//    public double TurnOrderWeight { get; set; }

//	public Skill SelectedSkill { get; set; }

//    public char Postfix { get; private set; }
//
//    public string UnitName { get { return this.Character.Name + " " + Postfix; } }
//
//    public bool IsDead { get { return this.Character.CurrentHp <= 0d; } }

	protected Vector3 defaultRotation = Vector3.zero;

    public void Init(BattleCharacter character)
    {
//        this.Team = team;
//        this.Character = character;
//        this.Postfix = postFix;
		this.hpBar.Init (character.HpPercentage);
//        this.TurnOrderWeight = BattleActionWeight.GetDefaultTurnOrderWeight(this);
		this.defaultRotation = this.transform.localEulerAngles;
    }

//    public void AssignToTile(TileController tile)
//    {
//        this.CurrentTile = tile;
//        this._characterView.Init();
//    }

    public float GetAttackPositionOffset(BattleUnitController opponentUnit)
    {
		return this.attackDistanceOffset + opponentUnit.bodySizeOffset;
    }

    public virtual IEnumerator MoveToPosition(Vector3 destination, float speed, float distanceOffset = 0f)
    {
		this.transform.LookAt(destination);
		this.characterView.CurrentAnimationState = BattleCharacterView.AnimationState.Walk;

		var distance = Vector3.Distance(this.transform.position, destination);
		var distanceActual = distance - distanceOffset;
		var completion = distanceActual / distance;
		var moveDuration = distanceActual / speed;

		yield return StartCoroutine(AnimationService.MoveToPosition(this.transform, destination, moveDuration, completion));
    }

    public virtual IEnumerator AnimateAttack()
    {
        yield return StartCoroutine(this.characterView.PlayAttackAnimation());
    }

    public virtual IEnumerator MoveToTile(TileController tile)
    {
		yield return StartCoroutine(this.MoveToPosition(tile.transform.position, this.regularMovementSpeed));
		this.DefaultStance();
    }

//    public virtual IEnumerator ReturnToBaseTile()
//    {
//        var position = this.CurrentTile.transform.position;
//        yield return StartCoroutine(this.characterView.MoveTowards(position));
//        this.DefaultStance();
//    }

	public virtual void DefaultStance()
	{
		this.characterView.CurrentAnimationState = BattleCharacterView.AnimationState.Idle;
		this.transform.localEulerAngles = defaultRotation;
	}

//    public virtual IEnumerator TakeDamage(double damage, bool isLastHit)
//    {
//		this.ShowDamageText(damage);
//
//		var hpRemaining = System.Math.Max (0d, this.Character.CurrentHp - damage);
//        this.Character.CurrentHp = hpRemaining;
//		var hpPercentage = this.Character.HpPercentage;
//		StartCoroutine(this.AnimateHpChange(hpPercentage));
//        
//		var isDead = this.IsDead && isLastHit;
//        yield return StartCoroutine(this.characterView.PlayDamagedAnimation(isDead));
//    }

//    public SkillComponentBase GetSelectedSkill()
//    {
//		var skill = this.Character.Skills [0];
//		if (SelectedSkill != null) {
//			skill = SelectedSkill;
//		}
//
//        // TODO: refactor these shit
//		var prefab = Resources.Load (skill.PrefabPath);
//		var go = Instantiate (prefab) as GameObject;
//		var skillComp = go.GetComponent<SkillComponentBase> ();
//		skillComp.InitWithSkill (skill);
//		return skillComp;
//    }

    // default AI attack
//    public IEnumerator RunAI()
//    {
//		var allUnits = BattleManager.Instance.AllUnits;
//        var targetTile = TargetLogic.GetTargetTile(this, allUnits);
//        yield return StartCoroutine(this.GetSelectedSkill().PlaySkillSequence(this, targetTile));
//    }
//
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
        var go = Instantiate(this.damageTextPrefab) as GameObject;
        var damageText = go.GetComponent<DamageText>();
        go.transform.SetParent(this.transform);
        go.transform.localPosition = this.damageTextPrefab.transform.localPosition;
        go.transform.localScale = this.damageTextPrefab.transform.localScale;
        go.SetActive(true);
        damageText.ShowDamage(damage);
    }

    private IEnumerator AnimateHpChange(float hpPercentage)
    {
		yield return StartCoroutine(hpBar.AnimateValueChange(hpPercentage));
//        if (this.IsDead)
//        {
//            hpBar.gameObject.SetActive(false);
//        }
    }
}
