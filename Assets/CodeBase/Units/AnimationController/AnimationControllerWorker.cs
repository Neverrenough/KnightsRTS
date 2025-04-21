using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerWorker : AnimationController
{
    protected override string GetIdleAnimationName() => "Idle";
    protected override string GetRunAnimationName() => "Run";
    protected override string GetAttackAnimationName(Vector3 targetPosition, Vector3 unitPosition) => "Build";

    public void PlayChopAnimation()
    {
        PlayAnimation("Chop");
    }

    public void PlayCarryAnimation()
    {
        PlayAnimation("Carry");
    }

    public void PlayIdleAnimation()
    {
        PlayAnimation("Idle");
    }
}


