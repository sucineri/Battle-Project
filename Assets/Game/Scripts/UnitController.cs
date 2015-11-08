using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UnitController : MonoBehaviour {

    [SerializeField] private UnitAnimationController animationController;
    [SerializeField] private HpBar hpBar;
    [SerializeField] private GameObject damageTextPrefab;

    public MapTile CurrentTile { get; private set; }

    public event Action<bool> onAnimationStateChange;

    private Character character;
    private float attackDistanceOffset = 1.5f;
    private float sizeDistanceOffset = 1.5f;

    public void SetCharacter(Character character)
    {
        this.character = character;
        this.hpBar.Init(0f, this.character.MaxHp, this.character.CurrentHp);
    }

    public void AssignToTile(MapTile tile)
    {
        this.CurrentTile = tile;
        this.animationController.RegisterDefaultRotation();
    }

    public IEnumerator AttackOpponentOnTile(MapTile opponentTile)
    {
        OnAnimationStateChange(true);
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
        yield return StartCoroutine(animationController.MoveTowards(tile.transform.position));
        tile.AssignUnit(this);
        animationController.DefaultStance();
        OnAnimationStateChange(false);
    }

    private IEnumerator TakeDamage(int damage)
    {
        var hpRemaining = Mathf.Max(0f, this.character.CurrentHp - damage);
        this.character.CurrentHp = (int)hpRemaining;
        var isDead = this.character.CurrentHp == 0;

        ShowDamageText(damage);
        AnimateHpChange(hpRemaining);

        yield return StartCoroutine(animationController.AnimateTakeDamage(isDead));
        if(isDead)
        {
            hpBar.gameObject.SetActive(false);
        }
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

    private void AnimateHpChange(float hpRemaining)
    {
        StartCoroutine(hpBar.AnimateValueChange(hpRemaining));
    }
        
    private IEnumerator AttackOpponent(UnitController opponent)
    {
        StartCoroutine(animationController.AnimateAttack());
        yield return new WaitForSeconds(animationController.GetAttackToDamageDelay());
        if(opponent != null)
        {
            var damage = DamageLogic.GetNormalAttackDamage(this.character, opponent.character);
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
