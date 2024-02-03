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
    [Header("Duel")]
    [Tooltip("How many seconds to wait before duel outcome is visible")]
    public float outcomeDelay = 1f;

    // Defines how a samurai fared in the duel
    public enum DuelPerformance
    {
        Missed,
        Hit,
        Yielded
    }

    // Accuracies of the samurai's hits.
    // -2 is no slash yet, -1 is a miss, >=0 is how far from sweetspot was the hit
    // -3 means the samurai did not attack in the duel
    (DuelPerformance?, float) leftSamuraiPerformance = (null, 100f);
    (DuelPerformance?, float) rightSamuraiPerformance = (null, 100f);

    // Allows a samurai to declare how accurate his slash was
    public void DeclareSlashAccuracy(
        Samurai samurai,
        DuelPerformance performance,
        float accuracy = 100f
    )
    {
        if (Object.ReferenceEquals(samurai, LeftSamurai))
            leftSamuraiPerformance = (performance, accuracy);
        else
            rightSamuraiPerformance = (performance, accuracy);

        // If both have slashed, then calculate aftermath
        if (leftSamuraiPerformance.Item1 != null && rightSamuraiPerformance.Item1 != null)
            StartCoroutine(Aftermath());
    }

    IEnumerator Aftermath()
    {
        yield return new WaitForSeconds(outcomeDelay);

        ShowOutcome();

        yield return new WaitForSeconds(1f);

        FindObjectOfType<GameMenu>().SetGameOver();
    }

    private void ShowOutcome()
    {
        // Both die
        if (
            // If both yielded
            (
                leftSamuraiPerformance.Item1 == DuelPerformance.Yielded
                && rightSamuraiPerformance.Item1 == DuelPerformance.Yielded
            )
            // If both hit with same accuracy
            || (
                leftSamuraiPerformance.Item1 == DuelPerformance.Hit
                && leftSamuraiPerformance.Item2 == rightSamuraiPerformance.Item2
            )
        )
        {
            RightSamurai.Die();
            LeftSamurai.Die();

            return;
        }

        if (
            leftSamuraiPerformance.Item1 == DuelPerformance.Missed
            || leftSamuraiPerformance.Item2 > rightSamuraiPerformance.Item2
        )
            LeftSamurai.Die();

        if (
            rightSamuraiPerformance.Item1 == DuelPerformance.Missed
            || rightSamuraiPerformance.Item2 > leftSamuraiPerformance.Item2
        )
            RightSamurai.Die();

        if (RightSamurai.IsDead == false)
            RightSamurai.Win();
        else if (LeftSamurai.IsDead == false)
            LeftSamurai.Win();

        print("Left accuracy:" + leftSamuraiPerformance);
        print("Right accuracy:" + rightSamuraiPerformance);
    }

    public void MaybeStartDuel()
    {
        if (!LeftSamurai.ReadyToDuel || !RightSamurai.ReadyToDuel)
            return;

        // At least one needs to be in dash stance
        if (LeftSamurai.WalkDirection <= 0 && RightSamurai.WalkDirection >= 0)
            return;

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
        LeftSamurai
            .GetComponent<PlayerInput>()
            .SwitchCurrentControlScheme("WASD", Keyboard.current);
        RightSamurai
            .GetComponent<PlayerInput>()
            .SwitchCurrentControlScheme("Arrows", Keyboard.current);
    }

    #endregion
}
