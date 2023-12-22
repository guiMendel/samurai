using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Samurai : MonoBehaviour
{
  // === REFS

  Orchestrator orchestrator;
  Samurai opponent;

  // === PARAMS
  public float dashSpeed = 8f;


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
  }


  // === MOVEMENT
  [Header("MOVEMENT")]

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

    print("before " + newDistance);

    // Respect distance boundaries
    newDistance = Helper.UnsignedClamp(
      newDistance, orchestrator.minSamuraisDistance, orchestrator.maxSamuraisDistance);

    print("after " + newDistance);

    transform.position = new Vector2(opponent.transform.position.x - newDistance, transform.position.y);
  }


  void Move(float direction) => moveDirection = Mathf.Sign(direction);

  private void Halt() => moveDirection = 0;


  // === INPUT CALLBACKS

  public void Move(InputAction.CallbackContext value)
  {
    // Move on x axis
    if (value.phase == InputActionPhase.Performed) Move(value.ReadValue<Vector2>().x);

    // Stop movement when released
    else if (value.phase != InputActionPhase.Started) Halt();
  }
}
