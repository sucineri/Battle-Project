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

    public Character Character { get; private set; }

    public float TurnOrderWeight { get; private set; }

    public char Postfix { get; private set; }

    public string UnitName { get { return this.Character.Name + " " + Postfix; } }

    public bool IsDead { get { return this.Character.CurrentHp == 0; } }

    public event Action<bool> onAnimationStateChange;

    private float hpBarAnimationDuration = 0.5f;

    public void Init(Const.Team team, Character character, char postFix)
    {
        this.Team = team;
        this.Character = character;
        this.Postfix = postFix;
        this._hpBar.Init(0f, this.Character.MaxHp, this.Character.CurrentHp);
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
        OnAnimationStateChange(true);
        this.TurnOrderWeight += BattleActionWeight.GetMoveActionWeight(this);
        yield return StartCoroutine(_animationController.MoveTowards(tile.transform.position));
        _animationController.DefaultStance();
        OnAnimationStateChange(false);
    }

    public virtual IEnumerator ReturnToBaseTile()
    {
        var position = this.CurrentTile.transform.position;
        yield return StartCoroutine(this._animationController.MoveTowards(position));
        this._animationController.DefaultStance();
    }

    public virtual IEnumerator TakeDamage(int damage, bool isLastHit)
    {
        var hpRemaining = Mathf.Max(0f, this.Character.CurrentHp - damage);
        this.Character.CurrentHp = (int)hpRemaining;

        this.ShowDamageText(damage);
        StartCoroutine(this.AnimateHpChange(hpRemaining));
        var isDead = this.IsDead && isLastHit;
        yield return StartCoroutine(this._animationController.PlayDamagedAnimation(isDead));
    }

    public SkillComponentBase GetSelectedSkill()
    {
        // TODO: refactor these shit
        var prefab = Resources.Load("Skills/MeleeAttack");
        var go = Instantiate(prefab) as GameObject;
        var skillComp = go.GetComponent<SkillComponentBase>();
        return skillComp;
    }

    // default AI attack
    public IEnumerator RunAI(List<UnitControllerBase> allUnits)
    {
        var targetTile = TargetLogic.GetTargetTile(this, allUnits);
        yield return StartCoroutine(this.GetSelectedSkill().PlaySkillSequence(this, targetTile));
    }

    private void ShowDamageText(int damage)
    {
        var go = Instantiate(this._damageTextPrefab) as GameObject;
        var damageText = go.GetComponent<DamageText>();
        go.transform.SetParent(this.transform);
        go.transform.localPosition = this._damageTextPrefab.transform.localPosition;
        go.transform.localScale = this._damageTextPrefab.transform.localScale;
        go.SetActive(true);
        damageText.ShowDamage(damage);
    }

    private IEnumerator AnimateHpChange(float hpRemaining)
    {
        yield return StartCoroutine(_hpBar.AnimateValueChange(hpRemaining, hpBarAnimationDuration));
        if (this.IsDead)
        {
            _hpBar.gameObject.SetActive(false);
        }
    }

    private void OnAnimationStateChange(bool isAnimating)
    {
        if (this.onAnimationStateChange != null)
        {
            this.onAnimationStateChange(isAnimating);
        }
    }
}
