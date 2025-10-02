using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PenaltyGameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private int shotsPerTeam = 5;

    [Header("Game Objects")]
    [SerializeField]
    private Ball ball;

    [SerializeField]
    private GameObject playerKicker;

    [SerializeField]
    private GameObject aiKicker;

    [SerializeField]
    private GameObject playerKeeper;

    [SerializeField]
    private GameObject aiKeeper;

    [Header("Start Positions")]
    [SerializeField]
    private Transform ballStartPos;

    [SerializeField]
    private Transform playerKickerStartPos;

    [SerializeField]
    private Transform aiKickerStartPos;

    [SerializeField]
    private Transform playerKeeperStartPos;

    [SerializeField]
    private Transform aiKeeperStartPos;

    [Header("Timing")]
    [SerializeField]
    private float goalCheckTimeout = 3f;

    [SerializeField]
    private float nextTurnDelay = 2f;

    private int playerScore = 0;
    private int aiScore = 0;
    private int currentPlayerShots = 0;
    private int currentAIShots = 0;

    private Team currentTurn = Team.Player;
    private bool isSuddenDeath = false;

    private bool ballEnteredGoal = false;
    private Team ballKickerTeam;
    private bool turnResolved = false;

    private void Start()
    {
        ResetGame();

        if (EventManager.instance == null)
        {
            return;
        }

        EventManager.instance.OnBallKicked += OnBallKicked;
        EventManager.instance.OnGoalScored += OnGoalScored;
        EventManager.instance.OnMissedShot += OnMissedShot;

        SetupTurn();
        EventManager.instance?.EventRoundStart();
    }

    private void ResetGame()
    {
        playerScore = 0;
        aiScore = 0;
        currentPlayerShots = 0;
        currentAIShots = 0;
        currentTurn = Team.Player; // arranca siempre el Player
        isSuddenDeath = false;

        ballEnteredGoal = false;
        turnResolved = false;
    }

    private void OnDestroy()
    {
        if (EventManager.instance == null)
            return;

        EventManager.instance.OnBallKicked -= OnBallKicked;
        EventManager.instance.OnGoalScored -= OnGoalScored;
        EventManager.instance.OnMissedShot -= OnMissedShot;
    }

    private void SetupTurn()
    {
        turnResolved = false; // 🔹 permitir que la corrutina se ejecute para este turno

        bool isPlayerTurn = currentTurn == Team.Player;

        Debug.Log(
            $"--------- RESULT: Player {playerScore} - AI {aiScore} --------- TURNO: {currentTurn}"
        );

        // Kicker
        if (playerKicker != null)
        {
            playerKicker.SetActive(isPlayerTurn);
            playerKicker.transform.position = playerKickerStartPos.position;
            playerKicker.transform.rotation = playerKickerStartPos.rotation;
            playerKicker.GetComponent<PenaltyInputManager>().EnableKick();
            playerKicker.GetComponent<PlayerKickable>().CurrentPower = 0; // resetear potencia
            playerKicker.GetComponent<PlayerKickable>().KickDirection = 0; // resetear dirección
        }

        // AI Kicker
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

            // 🔹 Activar input del arquero para elegir dirección solo si es turno del AI
            if (!isPlayerTurn)
            {
                var keeperInput = playerKeeper.GetComponent<KeeperInputManager>();
                keeperInput?.EnableInputForNextKick();
            }
        }

        // AI Keeper
        if (aiKeeper != null)
        {
            aiKeeper.SetActive(isPlayerTurn);
            aiKeeper.transform.position = aiKeeperStartPos.position;
            aiKeeper.transform.rotation = aiKeeperStartPos.rotation;
            aiKeeper.GetComponent<AIKeeper>().ResetPosition();
        }

        // Reset pelota
        if (ball != null)
        {
            ball.transform.position = ballStartPos.position;
            ball.transform.rotation = ballStartPos.rotation;
            ball.ResetBall();
        }

        if (!isPlayerTurn)
        {
            EventManager.instance?.EventAIKickingTurn();
        }
    }

    private void OnBallKicked(IKickable kicker)
    {
        if (kicker == null)
            return;

        ballKickerTeam = currentTurn;
        ballEnteredGoal = false;

        StartCoroutine(WaitForGoalOrTimeout(ballKickerTeam));
    }

    private IEnumerator WaitForGoalOrTimeout(Team kickerTeam)
    {
        if (turnResolved)
            yield break; // si ya se resolvió el turno, no hacemos nada
        turnResolved = true;
        ballEnteredGoal = false; // resetear al inicio

        float timer = 0f;
        while (timer < goalCheckTimeout)
        {
            if (ballEnteredGoal)
            {
                break;
            }
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
        }

        ballEnteredGoal = false; // asegurar que queda limpio
    }

    public void BallEnteredGoal() => ballEnteredGoal = true;

    private void OnGoalScored(Team kickingTeam)
    {
        if (kickingTeam == Team.Player)
            playerScore++;
        else
            aiScore++;

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
        if (kickingTeam == Team.Player)
            currentPlayerShots++;
        else
            currentAIShots++;
    }

    private void EndTurnWithDelay()
    {
        StartCoroutine(DelayedSetupTurn());
    }

    private IEnumerator DelayedSetupTurn()
    {
        yield return new WaitForSeconds(nextTurnDelay);

        currentTurn = (currentTurn == Team.Player) ? Team.AI : Team.Player;

        SetupTurn();
        EventManager.instance?.EventRoundStart();
    }

    private void CheckEndCondition()
    {
        int remainingPlayerShots = shotsPerTeam - currentPlayerShots;
        int remainingAIShots = shotsPerTeam - currentAIShots;

        if (!isSuddenDeath)
        {
            if (playerScore > (aiScore + remainingAIShots))
            {
                Debug.Log(" -------- GANADOR: Player --------");
                LoadEndScene(true);
                return;
            }

            if (aiScore > (playerScore + remainingPlayerShots))
            {
                Debug.Log(" -------- GANADOR: AI --------");
                LoadEndScene(false);
                return;
            }

            if (currentPlayerShots >= shotsPerTeam && currentAIShots >= shotsPerTeam)
            {
                if (playerScore == aiScore)
                {
                    isSuddenDeath = true;
                    shotsPerTeam++;
                }
                else
                {
                    bool playerWon = playerScore > aiScore;
                    Debug.Log($" -------- GANADOR: {(playerWon ? Team.Player : Team.AI)} --------");
                    LoadEndScene(playerWon);
                }
            }
        }
        else if (isSuddenDeath && currentPlayerShots == currentAIShots)
        {
            if (playerScore != aiScore)
            {
                bool playerWon = playerScore > aiScore;
                Debug.Log($" -------- GANADOR: {(playerWon ? Team.Player : Team.AI)} --------");
                LoadEndScene(playerWon);
            }
        }
    }

    public Team GetCurrentTurn() => currentTurn;

    private void LoadEndScene(bool playerWon)
    {
        string sceneName = playerWon ? "VictoryScene" : "DefeatScene";
        SceneManager.LoadScene(sceneName);
    }
}
