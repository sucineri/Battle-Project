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
        this.characterView.onDamageAnimationComplete += this.OnDamageAnimationComplete;
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

    public virtual IEnumerator MoveToPositionWithOffset(Vector3 destination, float speed, float distanceOffset)
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
        yield return StartCoroutine(this.MoveToPositionWithOffset(targetPosition, this.attackMovementSpeed, offset));
    }

    public virtual IEnumerator ReturnToPosition(Vector3 originalPosition)
    {
        yield return StartCoroutine(this.MoveToPositionWithOffset(originalPosition, this.attackMovementSpeed, 0));
        this.DefaultStance();
    }

    public virtual IEnumerator AnimateAttack()
    {
        yield return StartCoroutine(this.characterView.PlayAttackAnimation());
    }

    public virtual IEnumerator MoveToPosition(Vector3 position)
    {
        yield return StartCoroutine(this.MoveToPositionWithOffset(position, this.regularMovementSpeed, 0));
        this.DefaultStance();
    }

    public virtual void DefaultStance()
    {
        this.characterView.CurrentAnimationState = BattleCharacterView.AnimationState.Idle;
        this.transform.localEulerAngles = defaultRotation;
    }

    public void PlayEffect(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            var prefab = Resources.Load(path);
            var go = Instantiate(prefab) as GameObject;
            go.transform.SetParent(this.transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.SetActive(true); 
        }
    }

    public IEnumerator TakeEffect(BattleActionResult.EffectOnTarget effectOnTarget, float hpPercentage)
    {
        this.ShowEffectText(effectOnTarget);

        this.characterView.IsDead = hpPercentage <= 0f;
        if (effectOnTarget.hpChange < 0)
        {
            this.characterView.CurrentAnimationState = BattleCharacterView.AnimationState.Damage;
        }

        yield return StartCoroutine(this.AnimateHpChange(hpPercentage));
    }

    private void ShowEffectText(BattleActionResult.EffectOnTarget effectOnTarget)
    {
        var delay = 0f;

        if (effectOnTarget.hasDamageEffect)
        {
            var damageText = this.CreateDamageText();

            var damageResultText = "";
            var isNegativeEffect = true;
            if (effectOnTarget.isSuccess)
            {
                var damage = effectOnTarget.hpChange;
                if (damage > 0)
                {
                    isNegativeEffect = false;
                }
                damageResultText = damage.ToString("F0");
            }
            else
            {
                damageResultText = Const.MissedText;
            }

            damageText.ShowText(damageResultText, isNegativeEffect, effectOnTarget.isCritical, delay);

            delay += 0.5f;
        }


        if (effectOnTarget.HasStatusEffectResult)
        {
            var resultType = effectOnTarget.statusEffectResult.resultType;
            var statusEffectResultText = "";
            switch (resultType)
            {
                case BattleActionResult.StatusEffectResultType.Resisted:
                    statusEffectResultText = Const.ResistedText;
                    break;
                case BattleActionResult.StatusEffectResultType.Ineffetive:
                    statusEffectResultText = Const.IneffectiveText;
                    break;
            }
            if (!string.IsNullOrEmpty(statusEffectResultText))
            {
                var statusEffectText = this.CreateDamageText();
                statusEffectText.ShowText(statusEffectResultText, true, false, delay);
            }
        }
    }

    private DamageText CreateDamageText()
    {
        var go = Instantiate(this.damageTextPrefab) as GameObject;
        var damageText = go.GetComponent<DamageText>();
        go.transform.SetParent(this.transform);
        go.transform.localPosition = this.damageTextPrefab.transform.localPosition;
        go.transform.localScale = this.damageTextPrefab.transform.localScale;
        go.SetActive(true);

        return damageText;
    }

    private IEnumerator AnimateHpChange(float hpPercentage)
    {
        yield return StartCoroutine(hpBar.AnimateValueChange(hpPercentage));
    }

    private void OnDamageAnimationComplete(bool isDead)
    {
        if (isDead)
        {
            this.hpBar.gameObject.SetActive(false);
        }
    }
}
