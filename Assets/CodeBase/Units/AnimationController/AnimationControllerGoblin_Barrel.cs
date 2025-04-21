using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerGoblin_Barrel : AnimationController
{
    protected override string GetIdleAnimationName() => "Idle_In";
    protected override string GetRunAnimationName() => "Run";

    protected override string GetAttackAnimationName(Vector3 targetPosition, Vector3 unitPosition)
    {
        return "Fired";
    }
}
