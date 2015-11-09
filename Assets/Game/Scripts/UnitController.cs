using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour {

    [SerializeField] private UnitAnimationController animationController;
    [SerializeField] private HpBar hpBar;
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private float attackDistanceOffset = 1.5f;
    [SerializeField] private float sizeDistanceOffset = 1f;

    public MapTile CurrentTile { get; private set; }
    public Const.Team Team { get; private set; }
    public Character Character { get; private set; }
    public float TurnOrderWeight { get; private set; }

    public bool IsDead { get { return this.Character.CurrentHp == 0; } }

    public event Action<bool> onAnimationStateChange;

    private float hpBarAnimationDuration = 0.5f;

    public void Init(Const.Team team, Character character)
    {
        this.Team = team;
        this.Character = character;
        this.hpBar.Init(0f, this.Character.MaxHp, this.Character.CurrentHp);
        this.TurnOrderWeight = BattleActionWeight.GetDefaultTurnOrderWeight(this);
    }

    public void AssignToTile(MapTile tile)
    {
        this.CurrentTile = tile;
        this.animationController.RegisterDefaultRotation();
    }

    public IEnumerator AttackOpponentOnTile(MapTile opponentTile)
    {
        OnAnimationStateChange(true);
        this.TurnOrderWeight += BattleActionWeight.GetAttackActionWeight(this);
        var startPosition = this.transform.position;
        var opponent = opponentTile.CurrentUnit;
        var distanceOffset = this.attackDistanceOffset + opponent.sizeDistanceOffset;
        yield return StartCoroutine(animationController.MoveTowards(opponentTile.transform.position, distanceOffset));
        yield return StartCoroutine(this.AttackOpponent(opponent));
        yield return StartCoroutine(animationController.MoveTowards(startPosition));
        animationController.DefaultStance();
        OnAnimationStateChange(false);
    }

    public IEnumerator MoveToTile(MapTile tile)
    {
        OnAnimationStateChange(true);
        this.TurnOrderWeight += BattleActionWeight.GetMoveActionWeight(this);
        yield return StartCoroutine(animationController.MoveTowards(tile.transform.position));
        animationController.DefaultStance();
        OnAnimationStateChange(false);
    }

    // default AI attack
    public IEnumerator RunAI(List<UnitController> allUnits)
    {
        var targetTile = TargetLogic.GetTargetTile(this, allUnits);
        yield return StartCoroutine(this.AttackOpponentOnTile(targetTile));
    }

    private IEnumerator TakeDamage(int damage)
    {
        var hpRemaining = Mathf.Max(0f, this.Character.CurrentHp - damage);
        this.Character.CurrentHp = (int)hpRemaining;

        ShowDamageText(damage);
        StartCoroutine(AnimateHpChange(hpRemaining));
        yield return StartCoroutine(animationController.AnimateTakeDamage(this.IsDead));
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
        if(this.IsDead)
        {
            hpBar.gameObject.SetActive(false);
        }
    }
        
    private IEnumerator AttackOpponent(UnitController opponent)
    {
        StartCoroutine(animationController.AnimateAttack());
        yield return new WaitForSeconds(animationController.GetAttackToDamageDelay());
        if(opponent != null)
        {
            var damage = DamageLogic.GetNormalAttackDamage(this.Character, opponent.Character);
            yield return StartCoroutine(opponent.TakeDamage(damage));
        }
    }

    private void OnAnimationStateChange(bool isAnimating)
    {
        if(this.onAnimationStateChange != null)
        {
            this.onAnimationStateChange(isAnimating);
        }
    }
}
