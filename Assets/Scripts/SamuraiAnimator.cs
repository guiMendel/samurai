using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiAnimator : MonoBehaviour
{
  Animator animator;

  private void Awake()
  {
    animator = GetComponent<Animator>();
  }

  public void Walk() => animator.Play("Walk");

  public void Idle() => animator.Play("Idle");

  public void Dash() => animator.Play("Dash");

  public void ReadyStanding() => animator.Play("ReadyStanding");

  public void ReadyLeaning() => animator.Play("ReadyLeaning");

  public void Slash() => animator.Play("Slash");

  public void Dead() => animator.Play("Dead");
}
