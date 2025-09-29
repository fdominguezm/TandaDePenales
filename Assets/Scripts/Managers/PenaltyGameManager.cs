using System.Collections;
using UnityEngine;

public class PenaltyGameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int shotsPerTeam = 5;

    [Header("Game Objects")]
    [SerializeField] private Ball ball;
    [SerializeField] private GameObject playerKicker;
    [SerializeField] private GameObject aiKicker;
    [SerializeField] private GameObject playerKeeper;
    [SerializeField] private GameObject aiKeeper;

    [Header("Start Positions")]
    [SerializeField] private Transform ballStartPos;
    [SerializeField] private Transform playerKickerStartPos;
    [SerializeField] private Transform aiKickerStartPos;
    [SerializeField] private Transform playerKeeperStartPos;
    [SerializeField] private Transform aiKeeperStartPos;

    [Header("Timing")]
    [SerializeField] private float goalCheckTimeout = 3f;
    [SerializeField] private float nextTurnDelay = 2f;

    private int playerScore = 0;
    private int aiScore = 0;
    private int currentPlayerShots = 0;
    private int currentAIShots = 0;

    private Team currentTurn = Team.Player;
    private bool isSuddenDeath = false;

    private bool ballEnteredGoal = false;
    private Team ballKickerTeam;

    private void OnEnable()
    {
        if (EventManager.instance != null)
        {
            EventManager.instance.OnGoalScored += OnGoalScored;
            EventManager.instance.OnMissedShot += OnMissedShot;
            EventManager.instance.OnBallKicked += OnBallKicked;
        }
    }

    private void OnDisable()
    {
        if (EventManager.instance != null)
        {
            EventManager.instance.OnGoalScored -= OnGoalScored;
            EventManager.instance.OnMissedShot -= OnMissedShot;
            EventManager.instance.OnBallKicked -= OnBallKicked;
        }
    }

    private void Start()
    {
        SetupTurn();
        EventManager.instance?.EventRoundStart();
    }

    private void SetupTurn()
    {
        bool isPlayerTurn = currentTurn == Team.Player;

        // Kicker
        if (playerKicker != null)
        {
            playerKicker.SetActive(isPlayerTurn);
            playerKicker.transform.position = playerKickerStartPos.position;
            playerKicker.transform.rotation = playerKickerStartPos.rotation;
        }

        if (aiKicker != null)
        {
            aiKicker.SetActive(!isPlayerTurn);
            aiKicker.transform.position = aiKickerStartPos.position;
            aiKicker.transform.rotation = aiKickerStartPos.rotation;
        }

        // Keeper
        if (playerKeeper != null)
        {
            playerKeeper.SetActive(!isPlayerTurn);
            playerKeeper.transform.position = playerKeeperStartPos.position;
            playerKeeper.transform.rotation = playerKeeperStartPos.rotation;

            KeeperMovement km = playerKeeper.GetComponent<KeeperMovement>();
            KeeperKeepable kk = playerKeeper.GetComponent<KeeperKeepable>();
            if (km != null && kk != null)
                km.ResetKeeper(kk, playerKeeperStartPos.position);
        }

        if (aiKeeper != null)
        {
            aiKeeper.SetActive(isPlayerTurn);
            aiKeeper.transform.position = aiKeeperStartPos.position;
            aiKeeper.transform.rotation = aiKeeperStartPos.rotation;
        }

        // Reset pelota
        if (ball != null)
        {
            ball.transform.position = ballStartPos.position;
            ball.transform.rotation = ballStartPos.rotation;
            ball.ResetBall();
        }

        Debug.Log($"[PenaltyGameManager] Turno de {currentTurn}");

        if (!isPlayerTurn)
        {
            EventManager.instance?.EventAIKickingTurn();
            Debug.Log("[PenaltyGameManager] Evento AIKickingTurn lanzado");
        }
    }

    private void OnBallKicked(IKickable kicker)
    {
        if (kicker == null) return;

        ballKickerTeam = currentTurn;
        ballEnteredGoal = false;

        StartCoroutine(WaitForGoalOrTimeout(ballKickerTeam));
    }

    private IEnumerator WaitForGoalOrTimeout(Team kickerTeam)
    {
        float timer = 0f;
        while (timer < goalCheckTimeout)
        {
            if (ballEnteredGoal) break;
            timer += Time.deltaTime;
            yield return null;
        }

        if (currentTurn == kickerTeam)
        {
            if (ballEnteredGoal)
            {
                Debug.Log($"⚽ ¡GOL del equipo {kickerTeam}!");
                EventManager.instance?.EventGoalScored(kickerTeam);
            }
            else
            {
                Debug.Log($"❌ Tiro fallado del equipo {kickerTeam}");
                EventManager.instance?.EventMissedShot(kickerTeam);
            }

            ballEnteredGoal = false;
        }
    }

    public void BallEnteredGoal() => ballEnteredGoal = true;

    private void OnGoalScored(Team kickingTeam)
    {
        if (kickingTeam == Team.Player) playerScore++;
        else aiScore++;

        IncrementShotCounter(kickingTeam);
        CheckEndCondition();
        EndTurnWithDelay();
    }

    private void OnMissedShot(Team kickingTeam)
    {
        IncrementShotCounter(kickingTeam);
        CheckEndCondition();
        EndTurnWithDelay();
    }

    private void IncrementShotCounter(Team kickingTeam)
    {
        if (kickingTeam == Team.Player) currentPlayerShots++;
        else currentAIShots++;
    }

    private void EndTurnWithDelay()
    {
        StartCoroutine(DelayedSetupTurn());
    }

    private IEnumerator DelayedSetupTurn()
    {
        yield return new WaitForSeconds(nextTurnDelay);

        // Cambiar turno
        currentTurn = (currentTurn == Team.Player) ? Team.AI : Team.Player;

        SetupTurn();
        EventManager.instance?.EventRoundStart();
    }

    private void CheckEndCondition()
    {
        int remainingPlayerShots = shotsPerTeam - currentPlayerShots;
        int remainingAIShots = shotsPerTeam - currentAIShots;

        // Primera fase: rondas normales
        if (!isSuddenDeath)
        {
            // Ventaja imposible
            if (playerScore > aiScore + remainingAIShots)
            {
                EventManager.instance?.EventGameOver(true, Team.Player);
                return;
            }

            if (aiScore > playerScore + remainingPlayerShots)
            {
                EventManager.instance?.EventGameOver(true, Team.AI);
                return;
            }

            // Todos los tiros completados
            if (currentPlayerShots >= shotsPerTeam && currentAIShots >= shotsPerTeam)
            {
                if (playerScore == aiScore)
                {
                    isSuddenDeath = true;
                    shotsPerTeam++; // muerte súbita
                }
                else
                {
                    Team winner = (playerScore > aiScore) ? Team.Player : Team.AI;
                    EventManager.instance?.EventGameOver(playerScore > aiScore, winner);
                }
            }
        }
        // Muerte súbita
        else if (isSuddenDeath && currentPlayerShots == currentAIShots)
        {
            if (playerScore != aiScore)
            {
                Team winner = (playerScore > aiScore) ? Team.Player : Team.AI;
                EventManager.instance?.EventGameOver(playerScore > aiScore, winner);
            }
        }
    }

    public Team GetCurrentTurn() => currentTurn;
}
