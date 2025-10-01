using UnityEngine;

public class KeeperInputManager : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _keeperKeepable; // IKeepable
    private IKeepable keeper => _keeperKeepable as IKeepable;

    [Header("Keeper Controls")]
    [SerializeField] private KeyCode _jumpLeft = KeyCode.LeftArrow;
    [SerializeField] private KeyCode _jumpRight = KeyCode.RightArrow;
    [SerializeField] private KeyCode _jumpCenter = KeyCode.DownArrow;

    private bool canChooseDirection = false;

    private void OnEnable()
    {
        if (EventManager.instance != null)
        {
            // Se habilita input previo al disparo del AI
            EventManager.instance.OnBallKicked += OnBallKicked;
        }
    }

    private void OnDisable()
    {
        if (EventManager.instance != null)
            EventManager.instance.OnBallKicked -= OnBallKicked;
    }

    private void OnBallKicked(IKickable kicker)
    {
        // Una vez que la pelota se patea, dejamos fija la dirección elegida
        canChooseDirection = false;

        // Ejecutamos el salto final usando la última dirección seleccionada
        if (keeper != null && !keeper.HasJumped)
        {
            keeper.HasJumped = true;
            var jumpCmd = new JumpCommand(keeper);
            EventQueueManager.Instance.AddCommand(jumpCmd);
        }
    }

    private void Update()
    {
        if (!canChooseDirection || keeper == null) return;

        // Cambiar dirección mientras se pueda elegir
        if (Input.GetKeyDown(_jumpLeft)) keeper.JumpDirection = -1;
        else if (Input.GetKeyDown(_jumpRight)) keeper.JumpDirection = 1;
        else if (Input.GetKeyDown(_jumpCenter)) keeper.JumpDirection = 0;
    }

    public void EnableInputForNextKick()
    {
        // Llamar desde GameManager antes de que la IA patee
        canChooseDirection = true;
        if (keeper != null)
            keeper.HasJumped = false;
    }
}
