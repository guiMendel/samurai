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

        samurai.OnChangeWalkDirection.AddListener(OnChangeWalkDirection);
        samurai.OnChangeStance.AddListener(OnChangeStance);
        samurai.OnDash.AddListener(OnDash);
        samurai.OnSlash.AddListener(OnSlash);
        samurai.OnDie.AddListener(OnDie);
        samurai.OnWin.AddListener(OnWin);
    }

    private void OnWin() => animator.Play("ReadyLeaning");

    private void OnDie() => animator.Play("Dead");

    private void OnSlash() => animator.Play("Slash");

    private void OnChangeWalkDirection(float direction)
    {
        if (samurai.IsDueling)
            return;

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

    private void OnChangeStance()
    {
        if (samurai.IsDueling)
            return;

        if (samurai.CurrentStance != Samurai.Stance.Idle)
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

    private void OnDash() => animator.Play("Dash");
}
