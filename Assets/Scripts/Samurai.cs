using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    ApplyWalk();

    ApplyDash();

    UpdateTransitionTimer();
  }


  // ====================================
  #region DUEL
  [Header("Duel")]

  [Tooltip("Speed of the dash")]
  public float dashSpeed = 10f;

  [Tooltip("Distance beyond opponent which to stop dashing")]
  public float dashEndDistance = 4f;

  enum DuelState { None, Standing, Dashing, Over }

  // Which state the duel is in
  DuelState duelState = DuelState.None;

  // Whether this samurai is engaged in a duel
  bool IsDueling => duelState != DuelState.None;

  // Whether the samurai is dashing towards the opponent
  bool IsDashing => duelState == DuelState.Dashing;

  public void StartDuel()
  {
    if (IsDueling) return;

    if (walkDirection == opponentDirection) Dash();
    else duelState = DuelState.Standing;
  }

  void Dash()
  {
    duelState = DuelState.Dashing;
  }

  void ApplyDash()
  {
    if (!IsDashing) return;

    transform.position = new Vector2(
      transform.position.x + opponentDirection * dashSpeed * Time.deltaTime,
      transform.position.y
    );

    // Stop dash after the dash limit
    if (
      (opponentDirection > 0 && OpponentDistance < -dashEndDistance) ||
      (opponentDirection < 0 && OpponentDistance > dashEndDistance)
    )
      duelState = DuelState.Over;
  }

  #endregion


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

  // Whether samurai is ready to start the duel
  public bool ReadyToDuel => currentStance == Stance.Guard;

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
    orchestrator.MaybeStartDuel();

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
  #region WALK
  [Header("Walk")]

  [Tooltip("Speed of walking")]
  public float walkSpeed = 3f;

  [Tooltip("Speed modifier of backing away")]
  [Range(0.1f, 1f)] public float retreatModifier = 0.8f;

  // Direction the samurai is currently moving towards
  [DoNotSerialize] public float walkDirection = 0f;

  // Direction towards the opponent
  float opponentDirection;

  // Distance to the opponent
  float OpponentDistance => opponent.transform.position.x - transform.position.x;

  void ApplyWalk()
  {
    if (IsDueling || walkDirection == 0 || currentStance != Stance.Idle) return;

    float newDistance = OpponentDistance - walkDirection * walkSpeed * Time.deltaTime;

    // Respect distance boundaries
    newDistance = Helper.UnsignedClamp(
      newDistance, orchestrator.minSamuraisDistance, orchestrator.maxSamuraisDistance);

    transform.position = new Vector2(opponent.transform.position.x - newDistance, transform.position.y);
  }

  void SetWalkDirection(float direction)
  {
    walkDirection = Mathf.Sign(direction);

    if (walkDirection == opponentDirection)
    {
      transform.rotation = Quaternion.Euler(0, 0, 20 * -opponentDirection);

      // If both are in guard and stopped, starts a duel
      if (!IsDueling) orchestrator.MaybeStartDuel();

      else if (duelState == DuelState.Standing) Dash();
    }
    else walkDirection = retreatModifier * walkDirection;
  }

  private void StopWalk()
  {
    walkDirection = 0;

    if (!IsDueling) transform.rotation = Quaternion.identity;
  }

  public void InputWalk(InputAction.CallbackContext value)
  {
    // Move on x axis
    if (value.phase == InputActionPhase.Performed) SetWalkDirection(value.ReadValue<Vector2>().x);

    // Stop movement when released
    else if (value.phase != InputActionPhase.Started) StopWalk();
  }
  #endregion
}
