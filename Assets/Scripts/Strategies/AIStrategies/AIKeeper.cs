using UnityEngine;

public class AIKeeper : MonoBehaviour
{
    [Header("Keeper Reference")]
    [SerializeField] private MonoBehaviour _keeperObject; // Debe implementar IKeepable
    private IKeepable _keeper => _keeperObject as IKeepable;

    [Header("Team Info")]
    [SerializeField] private Team _team = Team.AI;

    [Header("AI Settings")]
    [SerializeField] private bool _predictPlayerKick = true;
    [SerializeField] private float _moveSpeed = 5f;

    [Header("Goal Dimensions")]
    [SerializeField] private float _arcHalfWidth = 4.5f; // mitad del ancho del arco (9 unidades)


    private Vector3 _startPos;
    private int _chosenDirection = 0;
    private Ball _ballToFollow;

    private void Awake()
    {
        _startPos = transform.position;
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
        if (kicker == null) return;

        // Ignorar si es el turno del equipo del keeper
        Team kickerTeam = (kicker is MonoBehaviour mb && mb.CompareTag("Player")) ? Team.Player : Team.AI;
        if (kickerTeam == _team) return;

        // Elegir zona del arco (-1=izq,0=centro,1=der)
        _chosenDirection = _predictPlayerKick ? 
            ((Random.value < 0.7f) ? kicker.KickDirection : Random.Range(-1, 2)) :
            Random.Range(-1, 2);

        if (_keeper != null) _keeper.JumpDirection = _chosenDirection;

        // Guardar la pelota para seguirla
        _ballToFollow = FindObjectOfType<Ball>();
    }

    private void Update()
    {
        if (_ballToFollow == null) return;

        // Calcular posición final según la dirección elegida
        float targetX = _startPos.x; // centro
        if (_chosenDirection == -1) targetX = _startPos.x - _arcHalfWidth; // poste izquierdo
        else if (_chosenDirection == 1) targetX = _startPos.x + _arcHalfWidth; // poste derecho

        // Seguir lateralmente la pelota pero limitarse al rango del poste
        float followX = Mathf.Clamp(
            _ballToFollow.transform.position.x,
            _startPos.x - _arcHalfWidth,
            _startPos.x + _arcHalfWidth
        );

        // Combinar movimiento hacia la dirección elegida y seguimiento de la pelota
        // Para que no se pase de los límites, interpolamos hacia la posición de la pelota dentro del rango permitido
        float newX = Mathf.MoveTowards(transform.position.x, followX, _moveSpeed * Time.deltaTime);
        Vector3 targetPos = new Vector3(newX, transform.position.y, _startPos.z);

        transform.position = targetPos;
    }

    public void ResetPosition()
    {
        transform.position = _startPos;
        if (_keeper != null) _keeper.JumpDirection = 0;
        _ballToFollow = null;
    }
}
