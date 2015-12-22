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

    protected Vector3 defaultRotation = Vector3.zero;

    public void Init(BattleCharacter character)
    {
        this.hpBar.Init(character.HpPercentage);		
        this.defaultRotation = this.transform.localEulerAngles;
    }

    public float GetAttackPositionOffset(BattleUnitController targetedUnit)
    {
        var targetBodySizeOffset = 0f;
        if (targetedUnit != null)
        {
            targetBodySizeOffset = targetedUnit.bodySizeOffset;
        }
        return this.attackDistanceOffset + targetBodySizeOffset;
    }

    public virtual IEnumerator MoveToPosition(Vector3 destination, float speed, float distanceOffset = 0f)
    {
        this.transform.LookAt(destination);
        this.characterView.CurrentAnimationState = BattleCharacterView.AnimationState.Walk;

        var distance = Vector3.Distance(this.transform.position, destination);
        var distanceActual = distance - distanceOffset;
        var completion = distanceActual / distance;
        var moveDuration = distanceActual / speed;

        yield return StartCoroutine(AnimationHelper.MoveToPosition(this.transform, destination, moveDuration, completion));
    }

    public virtual IEnumerator MoveToAttackPosition(BattleUnitController targetedUnit, Vector3 targetPosition)
    {
        var offset = this.GetAttackPositionOffset(targetedUnit);
        yield return StartCoroutine(this.MoveToPosition(targetPosition, this.attackMovementSpeed, offset));
    }

    public virtual IEnumerator ReturnToPosition(Vector3 originalPosition)
    {
        yield return StartCoroutine(this.MoveToPosition(originalPosition, this.attackMovementSpeed, 0));
        this.DefaultStance();
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

    public virtual void DefaultStance()
    {
        this.characterView.CurrentAnimationState = BattleCharacterView.AnimationState.Idle;
        this.transform.localEulerAngles = defaultRotation;
    }

    public void PlayEffect(string path)
    {
        var prefab = Resources.Load(path);
        var go = Instantiate(prefab) as GameObject;
        go.transform.SetParent(this.transform);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        go.SetActive(true);
    }

    public IEnumerator TakeDamage(double damage, float hpPercentage)
    {
        this.ShowDamageText(damage);

        yield return StartCoroutine(this.AnimateHpChange(hpPercentage));
    }

    public IEnumerator Die()
    {
        yield return StartCoroutine(this.characterView.PlayDamagedAnimation());
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
    }
}
