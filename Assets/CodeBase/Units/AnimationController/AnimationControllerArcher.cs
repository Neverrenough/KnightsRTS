using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerArcher : AnimationController
{
    protected override string GetIdleAnimationName() => "Idle";
    protected override string GetRunAnimationName() => "Run ";

    protected override string GetAttackAnimationName(Vector3 targetPosition, Vector3 unitPosition)
    {
        float deltaX = targetPosition.x - unitPosition.x;
        float deltaY = targetPosition.y - unitPosition.y;

        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            return "Shoot Front";
        }
        return deltaY > 0 ? "Shoot Up" : "Shoot Down";
    }
}
