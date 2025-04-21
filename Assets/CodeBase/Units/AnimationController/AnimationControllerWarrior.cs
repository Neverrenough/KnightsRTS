using UnityEngine;

public class AnimationControllerWarrior : AnimationController
{
    protected override string GetIdleAnimationName() => "Idle";
    protected override string GetRunAnimationName() => "Run";

    protected override string GetAttackAnimationName(Vector3 targetPosition, Vector3 unitPosition)
    {
        float deltaX = targetPosition.x - unitPosition.x;
        float deltaY = targetPosition.y - unitPosition.y;

        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            return "Front";
        }
        return deltaY > 0 ? "Up" : "Down";
    }
}
