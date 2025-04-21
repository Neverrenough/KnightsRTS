using UnityEngine;

public abstract class AnimationController : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    public void PlayIdle()
    {
        PlayAnimation(GetIdleAnimationName());
    }

    public void PlayRun()
    {
        PlayAnimation(GetRunAnimationName());
    }

    public void PlayAttack(Vector3 targetPosition, Vector3 unitPosition)
    {
        string attackAnimation = GetAttackAnimationName(targetPosition, unitPosition);
        PlayAnimation(attackAnimation);
    }

    protected void PlayAnimation(string animationName)
    {
        if (animator == null) return;

        if (animator.HasState(0, Animator.StringToHash(animationName)))
        {
            animator.Play(animationName);
        }
    }

    protected abstract string GetIdleAnimationName();
    protected abstract string GetRunAnimationName();
    protected abstract string GetAttackAnimationName(Vector3 targetPosition, Vector3 unitPosition);
}
