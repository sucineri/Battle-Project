using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UnitController : MonoBehaviour {

    [SerializeField] private Animator animator;
    [SerializeField] private HpBar hpBar;
    [SerializeField] private GameObject damageTextPrefab;

    public MapTile CurrentTile { get; private set; }

    public event Action<bool> onAnimationStateChange;

    private float moveSpeed = 30f;
    private Vector3 defaultRotation = Vector3.zero;
    private float unitAttackDistance = 3f;
    private float attackToDamgeDelay = 0.2f;
    private Character character;

    public void RegisterDefaultRotation()
    {
        defaultRotation = transform.localEulerAngles;
    }

    public void SetCharacter(Character character)
    {
        this.character = character;
        this.hpBar.Init(0f, this.character.MaxHp, this.character.CurrentHp);
    }

    public void AssignToTile(MapTile tile)
    {
        this.CurrentTile = tile;
    }

    public IEnumerator AttackTile(MapTile tile)
    {
        OnAnimationStateChange(true);
        var startPosition = this.transform.position;
        yield return StartCoroutine(this.MoveTowards(tile.transform.position, unitAttackDistance));
        yield return StartCoroutine(Attack(tile.CurrentUnit));
        yield return StartCoroutine(this.MoveTowards(startPosition));
        DefaultStance();
        OnAnimationStateChange(false);
    }

    public IEnumerator MoveToTile(MapTile tile)
    {
        OnAnimationStateChange(true);
        yield return StartCoroutine(this.MoveTowards(tile.transform.position));
        tile.AssignUnit(this);
        DefaultStance();
        OnAnimationStateChange(false);
    }

    public IEnumerator TakeDamage(Character attacker)
    {
        var defender = this.character;
        var damage = this.CalculateDamage(attacker, defender);

        var hpRemaining = Mathf.Max(0f, this.character.CurrentHp - damage);
        this.character.CurrentHp = (int)hpRemaining;
        var isDead = this.character.CurrentHp == 0;

        animator.SetBool("dead", isDead);
        animator.CrossFade("Damage", 0);

        ShowDamageText(damage);
        AnimateHpChange(hpRemaining);

        yield return new WaitForSeconds(0.4f);
    }

    private int CalculateDamage(Character attacker, Character defender)
    {
        return attacker.Attack - defender.Defense;
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

    private IEnumerator MoveTowards(Vector3 destination, float distanceOffset = 0f)
    {
        transform.LookAt(destination);
        animator.CrossFade("Walk", 0);
        var startPosition = transform.position;
        var distance = Vector3.Distance(transform.position, destination);
        var distanceActual = distance - distanceOffset;
        var totalProgress = distanceActual / distance;
        var timeTotal = distanceActual / moveSpeed;
        var timeElapsed = 0f;

        while(timeElapsed < timeTotal)
        {
            timeElapsed += Time.deltaTime;
            var progress = Mathf.Min(timeElapsed / timeTotal, totalProgress);
            transform.position = Vector3.Lerp(startPosition, destination, progress);
            yield return 0;
        }
        yield return 0;
    }
        
    private IEnumerator Attack(UnitController opponent)
    {
        animator.CrossFade("Attack", 0);
        if(opponent != null)
        {
            yield return new WaitForSeconds(attackToDamgeDelay);
            yield return StartCoroutine(opponent.TakeDamage(this.character));
        }
        animator.CrossFade("Wait", 0);
    }

    private void OnAnimationStateChange(bool isAnimating)
    {
        if(this.onAnimationStateChange != null)
        {
            this.onAnimationStateChange(isAnimating);
        }
    }

    private void DefaultStance()
    {
        animator.CrossFade("Wait", 0);
        transform.localEulerAngles = defaultRotation;
    }
}
