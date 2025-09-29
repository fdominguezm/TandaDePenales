using UnityEngine;

public class GoalDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;

        PenaltyGameManager manager = FindObjectOfType<PenaltyGameManager>();
        if (manager != null)
        {
            manager.BallEnteredGoal();
        }
    }
}
