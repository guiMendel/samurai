using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.InputSystem;

public class Samurai : MonoBehaviour
{
  // === REFS

  Orchestrator orchestrator;
  Samurai opponent;


  private void Awake()
  {
    orchestrator = FindObjectOfType<Orchestrator>();
    var samurais = FindObjectsOfType<Samurai>();

    opponent = Object.ReferenceEquals(gameObject, samurais[0].gameObject)
      ? samurais[1]
      : samurais[0];

    opponentDirection = Mathf.Sign(OpponentDistance);
  }

  private void Update()
  {
    ApplyMovement();

    UpdateTransitionTimer();
  }


  // ====================================
  #region GUARD
  [Header("Guard")]

  [Tooltip("Seconds it takes the samurai to enter guard stance")]
  public float guardStanceWindup = 0.3f;

  [Tooltip("Seconds it takes the samurai to enter guard stance")]
  public float guardStanceUnwind = 0.2f;

  enum Stance { Idle, TransitionIn, TransitionOut, Guard }

  // Current stance of the samurai
  Stance currentStance = Stance.Idle;

  // Current coroutine for stance transition
  Coroutine stanceTransitionCoroutine;

  // How many seconds a transition has been going for
  float transitionDuration = -1f;

  void UpdateTransitionTimer()
  {
    if (transitionDuration >= 0) transitionDuration += Time.deltaTime;
  }

  IEnumerator SetGuardStance(bool enterGuardStance, float modifier = 0f)
  {
    transitionDuration = 0f;

    currentStance = enterGuardStance ? Stance.TransitionIn : Stance.TransitionOut;

    GetComponent<SpriteRenderer>().color = enterGuardStance ? Color.blue : Color.gray;

    if (modifier > 0f)
      yield return new WaitForSeconds(
        (enterGuardStance ? guardStanceWindup : guardStanceUnwind) * modifier);

    GetComponent<SpriteRenderer>().color = enterGuardStance ? Color.red : Color.white;

    currentStance = enterGuardStance ? Stance.Guard : Stance.Idle;

    transitionDuration = -1f;
  }

  public void InputGuard(InputAction.CallbackContext value)
  {
    float modifier = 1f;

    void Interrupt(float currentStanceDuration)
    {
      if (stanceTransitionCoroutine != null)
      {
        // If in the middle of an unwind, discount time
        if (transitionDuration > 0f)
          modifier = Mathf.Max(transitionDuration / currentStanceDuration, 0.2f);

        StopCoroutine(stanceTransitionCoroutine);
      }
    }

    // Enter guard on press
    if (value.performed && currentStance != Stance.TransitionIn)
    {
      Interrupt(guardStanceUnwind);

      stanceTransitionCoroutine = StartCoroutine(SetGuardStance(true, modifier));
      return;
    }

    // Leave guard stance on release
    if (value.canceled && currentStance != Stance.TransitionOut)
    {
      Interrupt(guardStanceWindup);

      stanceTransitionCoroutine = StartCoroutine(SetGuardStance(false, modifier));
    }
  }

  #endregion


  // ====================================
  #region MOVEMENT
  [Header("Movement")]

  [Tooltip("Speed of walking")]
  public float walkSpeed = 3f;

  // Direction the samurai is currently moving towards
  float moveDirection = 0f;

  // Direction towards the opponent
  float opponentDirection;

  // Distance to the opponent
  float OpponentDistance => opponent.transform.position.x - transform.position.x;

  void ApplyMovement()
  {
    if (moveDirection == 0) return;

    float newDistance = OpponentDistance - moveDirection * walkSpeed * Time.deltaTime;

    // Respect distance boundaries
    newDistance = Helper.UnsignedClamp(
      newDistance, orchestrator.minSamuraisDistance, orchestrator.maxSamuraisDistance);

    transform.position = new Vector2(opponent.transform.position.x - newDistance, transform.position.y);
  }

  void Move(float direction) => moveDirection = Mathf.Sign(direction);

  private void Halt() => moveDirection = 0;

  public void InputMove(InputAction.CallbackContext value)
  {
    // Move on x axis
    if (value.phase == InputActionPhase.Performed) Move(value.ReadValue<Vector2>().x);

    // Stop movement when released
    else if (value.phase != InputActionPhase.Started) Halt();
  }
  #endregion
}
