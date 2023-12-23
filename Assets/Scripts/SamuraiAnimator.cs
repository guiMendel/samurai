using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiAnimator : MonoBehaviour
{
    // === REFS
    Animator animator;
    Samurai samurai;

    private void Awake()
    {
        samurai = GetComponent<Samurai>();
        animator = GetComponent<Animator>();

        Helper.AssertNotNull(animator, samurai);

        samurai.OnMove.AddListener(OnMove);
        samurai.OnGuard.AddListener(OnGuard);
    }

    private void OnMove(float direction)
    {
        if (samurai.ReadyToDuel)
        {
            if (direction == samurai.OpponentDirection)
                animator.Play("ReadyLeaning");
            else
                animator.Play("ReadyStanding");

            return;
        }

        if (direction == 0f)
            animator.Play("Idle");
        else
            animator.Play("Walk");
    }

    private void OnGuard(bool enterGuardStance)
    {
        if (enterGuardStance)
        {
            if (samurai.WalkDirection == samurai.OpponentDirection)
                animator.Play("ReadyLeaning");
            else
                animator.Play("ReadyStanding");
        }
        else if (samurai.WalkDirection == 0f)
            animator.Play("Idle");
        else
            animator.Play("Walk");
    }
}
