using UnityEngine;

public class AIKeeper : MonoBehaviour
{
    [Header("Keeper Reference")]
    [SerializeField]
    private MonoBehaviour _keeperObject; // Debe implementar IKeepable
    private IKeepable _keeper => _keeperObject as IKeepable;

    [Header("Team Info")]
    [SerializeField]
    private Team _team = Team.AI;

    [Header("AI Settings")]
    [SerializeField]

    private float moveSpeed = 5f;

    [Header("Goal Dimensions")]
    [SerializeField]
    private float arcHalfWidth = 4f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool moving = false;

    private void Awake()
    {
        startPos = transform.position;
        targetPos = startPos;
    }

    private void OnEnable()
    {
        if (EventManager.instance != null)
            EventManager.instance.OnBallKicked += HandleBallKicked;
    }

    private void OnDisable()
    {
        if (EventManager.instance != null)
            EventManager.instance.OnBallKicked -= HandleBallKicked;
    }

    private void HandleBallKicked(IKickable kicker)
    {
        if (kicker == null)
            return;

        // Ignorar si es el turno del equipo del keeper
        Team kickerTeam =
            (kicker is MonoBehaviour mb && mb.CompareTag("Player")) ? Team.Player : Team.AI;
        if (kickerTeam == _team)
            return;

        // Elegir SIEMPRE una dirección aleatoria (-1=izq, 0=centro, 1=der)
        int chosenDir = Random.Range(-1, 2);

        if (_keeper != null)
        {
            _keeper.JumpDirection = chosenDir;
            _keeper.HasJumped = true;
        }

        // Calcular target según la dirección elegida
        targetPos = startPos;
        if (chosenDir == -1)
            targetPos.x = startPos.x - arcHalfWidth;
        else if (chosenDir == 1)
            targetPos.x = startPos.x + arcHalfWidth;

        moving = true;
    }

    private void Update()
    {
        if (!moving)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        // Si llegó al target, dejar de moverse
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            moving = false;
    }

    public void ResetPosition()
    {
        transform.position = startPos;
        targetPos = startPos;
        moving = false;

        if (_keeper != null)
        {
            _keeper.JumpDirection = 0;
            _keeper.HasJumped = false;
        }
    }
}
