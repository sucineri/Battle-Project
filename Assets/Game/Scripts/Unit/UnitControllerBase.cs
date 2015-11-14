using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UnitControllerBase : MonoBehaviour
{

    [SerializeField]
    private UnitAnimationControllerBase animationController;
    [SerializeField]
    private HpBar hpBar;
    [SerializeField]
    private GameObject damageTextPrefab;

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
        this.hpBar.Init(0f, this.Character.MaxHp, this.Character.CurrentHp);
        this.TurnOrderWeight = BattleActionWeight.GetDefaultTurnOrderWeight(this);
    }

    public void AssignToTile(MapTile tile)
    {
        this.CurrentTile = tile;
        this.animationController.OnInit();
    }

    public virtual IEnumerator AttackOpponentOnTile(MapTile opponentTile)
    {
        OnAnimationStateChange(true);
        
        this.TurnOrderWeight += BattleActionWeight.GetAttackActionWeight(this);
        var startPosition = this.transform.position;
        var opponent = opponentTile.CurrentUnit;
        var distanceOffset = this.Character.AttackDistance + opponent.Character.SizeOffset;
        
        // Move to opponent
        yield return StartCoroutine(animationController.MoveTowards(opponentTile.transform.position, distanceOffset));

        // Attack opponent;
        yield return StartCoroutine(this.NormalAttackOpponent(opponent));
        
        // Move back to position
        yield return StartCoroutine(animationController.MoveTowards(startPosition));
        
        animationController.DefaultStance();
        OnAnimationStateChange(false);
    }

    protected virtual IEnumerator NormalAttackOpponent(UnitControllerBase opponent)
    {
        for (int i = 0; i < this.Character.NumberOfAttacks; ++i)
        {
            var damage = DamageLogic.GetNormalAttackDamage(this.Character, opponent.Character);
            StartCoroutine(animationController.AnimateAttack());
            yield return new WaitForSeconds(this.Character.AttackDelay);
            yield return StartCoroutine(opponent.TakeDamage(damage, i == this.Character.NumberOfAttacks - 1));
        }
    }

    public virtual IEnumerator MoveToTile(MapTile tile)
    {
        OnAnimationStateChange(true);
        this.TurnOrderWeight += BattleActionWeight.GetMoveActionWeight(this);
        yield return StartCoroutine(animationController.MoveTowards(tile.transform.position));
        animationController.DefaultStance();
        OnAnimationStateChange(false);
    }

    // default AI attack
    public IEnumerator RunAI(List<UnitControllerBase> allUnits)
    {
        var targetTile = TargetLogic.GetTargetTile(this, allUnits);
        yield return StartCoroutine(this.AttackOpponentOnTile(targetTile));
    }

    protected virtual IEnumerator TakeDamage(int damage, bool isLastHit)
    {
        var hpRemaining = Mathf.Max(0f, this.Character.CurrentHp - damage);
        this.Character.CurrentHp = (int)hpRemaining;

        ShowDamageText(damage);
        StartCoroutine(AnimateHpChange(hpRemaining));
        var isDead = this.IsDead && isLastHit;
        yield return StartCoroutine(animationController.AnimateTakeDamage(isDead));
    }

    private void ShowDamageText(int damage)
    {
        var go = Instantiate(this.damageTextPrefab) as GameObject;
        var damageText = go.GetComponent<DamageText>();
        go.transform.SetParent(this.transform);
        go.transform.localPosition = this.damageTextPrefab.transform.localPosition;
        go.transform.localScale = this.damageTextPrefab.transform.localScale;
        go.SetActive(true);
        damageText.ShowDamage(damage);
    }

    private IEnumerator AnimateHpChange(float hpRemaining)
    {
        yield return StartCoroutine(hpBar.AnimateValueChange(hpRemaining, hpBarAnimationDuration));
        if (this.IsDead)
        {
            hpBar.gameObject.SetActive(false);
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
