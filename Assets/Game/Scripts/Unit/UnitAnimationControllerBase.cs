using UnityEngine;
using System.Collections;

public class UnitAnimationControllerBase : MonoBehaviour
{

    [SerializeField]
    private Animator characterAnimator;
    [SerializeField]
    private float moveDuration;

    private Vector3 defaultRotation = Vector3.zero;

    private int waitStateHash = Animator.StringToHash("Wait");
    private int attackStateHash = Animator.StringToHash("Attack");
    private int walkStateHash = Animator.StringToHash("Walk");
    private int damageStateHash = Animator.StringToHash("Damage");

    public virtual void OnInit()
    {
        defaultRotation = transform.localEulerAngles;
    }

    public virtual IEnumerator MoveTowards(Vector3 destination, float distanceOffset = 0f)
    {
        transform.LookAt(destination);
        characterAnimator.CrossFade(walkStateHash, 0);
        var startPosition = transform.position;
        var distance = Vector3.Distance(startPosition, destination);
        var distanceActual = distance - distanceOffset;
        var completion = distanceActual / distance;

        yield return StartCoroutine(AnimationService.MoveToPosition(this, this.transform, destination, moveDuration, completion));
    }

    public virtual IEnumerator PlayAttackAnimation()
    {
        characterAnimator.CrossFade(attackStateHash, 0);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(this.GetCurrentClipDuration());
        characterAnimator.CrossFade(waitStateHash, 0);
    }

    public virtual IEnumerator PlayDamagedAnimation(bool isDead)
    {
        characterAnimator.SetBool("dead", isDead);
        characterAnimator.CrossFade(damageStateHash, 0);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(this.GetCurrentClipDuration());
    }

    public virtual void DefaultStance()
    {
        characterAnimator.CrossFade(waitStateHash, 0);
        transform.localEulerAngles = defaultRotation;
    }

    protected float GetCurrentClipDuration()
    {
        return characterAnimator.GetCurrentAnimatorClipInfo(0) [0].clip.averageDuration;
    }
}
