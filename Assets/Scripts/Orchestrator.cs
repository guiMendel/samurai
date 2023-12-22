using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Orchestrator : MonoBehaviour
{
  private void Awake()
  {
    samurais = FindObjectsOfType<Samurai>();

    if (samurais[0].transform.position.x > samurais[1].transform.position.x)
      (samurais[1], samurais[0]) = (samurais[0], samurais[1]);
  }

  private void Start()
  {
    AssignSamuraisControlSchemes();
  }


  // === SAMURAI
  [Header("Samurai")]

  [Tooltip("Minimum distance between the 2 samurai")]
  public float minSamuraisDistance = 10f;

  [Tooltip("Maximum distance between the 2 samurai")]
  public float maxSamuraisDistance = 100f;


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
