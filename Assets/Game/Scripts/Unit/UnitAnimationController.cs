using UnityEngine;
using System.Collections;

public class UnitAnimationController : MonoBehaviour {

    [SerializeField] private Animator animator;

    private float moveSpeed = 30f;
    private Vector3 defaultRotation = Vector3.zero;
    private float attackToDamgeDelay = 0.2f;

    private int waitStateHash = Animator.StringToHash("Wait");
    private int attackStateHash = Animator.StringToHash("Attack");
    private int walkStateHash = Animator.StringToHash("Walk");
    private int damageStateHash = Animator.StringToHash("Damage");

    public void RegisterDefaultRotation()
    {
        defaultRotation = transform.localEulerAngles;
    }
        
    public float GetAttackToDamageDelay()
    {
        return this.attackToDamgeDelay;
    }

    public IEnumerator MoveTowards(Vector3 destination, float distanceOffset = 0f)
    {
        transform.LookAt(destination);
        animator.CrossFade(walkStateHash, 0);
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

    public IEnumerator AnimateAttack()
    {
        animator.CrossFade(attackStateHash, 0);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(this.GetCurrentClipDuration());
        animator.CrossFade(waitStateHash, 0);
    }

    public IEnumerator AnimateTakeDamage(bool isDead)
    {
        animator.SetBool("dead", isDead);
        animator.CrossFade(damageStateHash, 0);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(this.GetCurrentClipDuration());
    }

    public void DefaultStance()
    {
        animator.CrossFade(waitStateHash, 0);
        transform.localEulerAngles = defaultRotation;
    }

    private float GetCurrentClipDuration()
    {
        return animator.GetCurrentAnimatorClipInfo(0)[0].clip.averageDuration;
    }
}
