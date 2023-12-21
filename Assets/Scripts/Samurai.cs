using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Samurai : MonoBehaviour
{
  // === PARAMS
  public float dashSpeed = 8f;


  private void Update()
  {
    ApplyMovement();
  }


  // === MOVEMENT
  [Header("MOVEMENT")]

  [Tooltip("Speed of walking")]
  public float walkSpeed = 3f;

  float moveDirection = 0f;

  void ApplyMovement()
  {
    float newX = transform.position.x + moveDirection * walkSpeed * Time.deltaTime;

    transform.position = new Vector2(newX, transform.position.y);
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
