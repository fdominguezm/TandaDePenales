using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    #region SINGLETON
    public static EventManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // no solo el componente
            return;
        }

        instance = this;
    }
    #endregion

    #region GAME_EVENTS
    public event Action<IKickable> OnBallKicked;

    public void EventBallKicked(IKickable kicker) => OnBallKicked?.Invoke(kicker);

    public event Action<Team> OnGoalScored;

    public void EventGoalScored(Team scoringTeam) => OnGoalScored?.Invoke(scoringTeam);

    public event Action<Team> OnMissedShot;

    public void EventMissedShot(Team shootingTeam) => OnMissedShot?.Invoke(shootingTeam);

    public event Action<bool, Team> OnGameOver; // bool = player won, Team = winner

    public void EventGameOver(bool isVictory, Team winningTeam) =>
        OnGameOver?.Invoke(isVictory, winningTeam);

    public event Action<IKeepable> OnKeeperJump;

    public void EventKeeperJump(IKeepable keeper) => OnKeeperJump?.Invoke(keeper);

    public event Action OnRoundStart;

    public void EventRoundStart() => OnRoundStart?.Invoke();

    public event Action OnAIKickingTurn;

    public void EventAIKickingTurn() => OnAIKickingTurn?.Invoke();

    #endregion
}
