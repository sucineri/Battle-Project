using UnityEngine;
using System;
using System.Collections;

public class BattleCharacterView : MonoBehaviour
{
    public enum AnimationState
    {
        Idle,
        Attack,
        Walk,
        Damage
    }
        
    [SerializeField] protected Animator characterAnimator;

    public event Action<bool> onDamageAnimationComplete;

    protected int waitStateHash = Animator.StringToHash("Wait");
    protected int attackStateHash = Animator.StringToHash("Attack");
    protected int walkStateHash = Animator.StringToHash("Walk");
    protected int damageStateHash = Animator.StringToHash("Damage");

    private AnimationState _currentAnimationState;
    public AnimationState CurrentAnimationState
    {
        get
        {
            return this._currentAnimationState;
        }
        set
        {
            this._currentAnimationState = value;
            this.OnAnimationStateChange();
        }
    }

    private bool _isDead = false;
    public bool IsDead 
    { 
        get
        {
            return this._isDead;
        } 
        set
        {
            this._isDead = value;
            this.characterAnimator.SetBool("dead", this._isDead);
        } 
    }

    public virtual IEnumerator PlayAttackAnimation()
    {
        characterAnimator.CrossFade(attackStateHash, 0);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(this.GetCurrentClipDuration());
        this.CurrentAnimationState = AnimationState.Idle;
    }

    public virtual IEnumerator PlayDamagedAnimation()
    {
        characterAnimator.CrossFade(damageStateHash, 0);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(this.GetCurrentClipDuration());

        if (this.onDamageAnimationComplete != null)
        {
            this.onDamageAnimationComplete(this.IsDead);
        }
    }

    protected float GetCurrentClipDuration()
    {
        return characterAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.averageDuration;
    }

    protected void OnAnimationStateChange()
    {
        switch (this._currentAnimationState)
        {
            case AnimationState.Idle:
                this.characterAnimator.CrossFade(waitStateHash, 0);
                break;
            case AnimationState.Attack:
                this.StartCoroutine(this.PlayAttackAnimation());
                break;
            case AnimationState.Walk:
                this.characterAnimator.CrossFade(walkStateHash, 0);
                break;
            case AnimationState.Damage:
                this.StartCoroutine(this.PlayDamagedAnimation());
                break;			
        }
    }
}
