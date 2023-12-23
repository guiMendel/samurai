using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

  // ====================================
  #region DUEL

  // Accuracies of the samurai's hits.
  // -2 is no slash yet, -1 is a miss, >=0 is how far from sweetspot was the hit
  // -3 means the samurai did not attack in the duel
  float leftSamuraiAccuracy = -2f;
  float rightSamuraiAccuracy = -2f;

  // Allows a samurai to declare how accurate his slash was
  public void DeclareSlashAccuracy(Samurai samurai, float accuracy)
  {
    if (Object.ReferenceEquals(samurai, LeftSamurai)) leftSamuraiAccuracy = accuracy;
    else rightSamuraiAccuracy = accuracy;

    // If both have slashed, then calculate aftermath
    if (leftSamuraiAccuracy != -2f && rightSamuraiAccuracy != -2f) StartCoroutine(CalculateAftermath());
  }

  IEnumerator CalculateAftermath()
  {
    print("Left accuracy:" + leftSamuraiAccuracy);
    print("Right accuracy:" + rightSamuraiAccuracy);

    yield return new WaitForSeconds(1.5f);

    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  public void MaybeStartDuel()
  {
    if (!LeftSamurai.ReadyToDuel || !RightSamurai.ReadyToDuel) return;

    // At least one needs to be in dash stance
    if (LeftSamurai.WalkDirection <= 0 && RightSamurai.WalkDirection >= 0) return;

    LeftSamurai.StartDuel();
    RightSamurai.StartDuel();
  }

  #endregion


  // ====================================
  #region SAMURAI
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

  #endregion
}
