using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Orchestrator : MonoBehaviour
{
  private void Awake()
  {
    samurais = FindObjectsOfType<Samurai>();
  }

  private void Start()
  {
    AssignSamuraisControlSchemes();
  }


  // === SAMURAI

  // Array with the 2 samurai
  Samurai[] samurais = new Samurai[2];

  Samurai LeftSamurai => samurais[0];
  Samurai RightSamurai => samurais[1];

  // Assigns each samurai its control scheme
  void AssignSamuraisControlSchemes()
  {
    LeftSamurai.GetComponent<PlayerInput>().SwitchCurrentControlScheme("WASD", Keyboard.current);
    RightSamurai.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Arrows", Keyboard.current);
  }
}
