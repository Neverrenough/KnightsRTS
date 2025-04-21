using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerGoblin_Pyro : AnimationController
{
    protected override string GetIdleAnimationName() => "Idle";
    protected override string GetRunAnimationName() => "Run";

    protected override string GetAttackAnimationName(Vector3 targetPosition, Vector3 unitPosition)
    {
        float deltaX = targetPosition.x - unitPosition.x;
        float deltaY = targetPosition.y - unitPosition.y;

        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            return "Attack_Right";
        }
        return deltaY > 0 ? "Attack_Up" : "Attack_Down";
    }
}
