using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerGoblin_Demoman : AnimationController
{
    protected override string GetIdleAnimationName() => "Idle";
    protected override string GetRunAnimationName() => "Run ";

    protected override string GetAttackAnimationName(Vector3 targetPosition, Vector3 unitPosition)
    {
        float deltaX = targetPosition.x - unitPosition.x;
        float deltaY = targetPosition.y - unitPosition.y;

        return "Throw";
    }
}
