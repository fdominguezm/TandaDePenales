using UnityEngine;

public class KeeperInputManager : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _keeperKeepable; // IKeepable

    private IKeepable keeper => _keeperKeepable as IKeepable;


    [Header("Keeper Controls")]
    [SerializeField] private KeyCode _jumpLeft = KeyCode.LeftArrow;
    [SerializeField] private KeyCode _jumpRight = KeyCode.RightArrow;
    [SerializeField] private KeyCode _jumpCenter = KeyCode.DownArrow;

    private void Start()
    {
        
    }

    private void Update()
    {
        PenaltyGameManager manager = FindObjectOfType<PenaltyGameManager>();
        if (manager.GetCurrentTurn() != Team.AI) return;

        HandleKeeperInput();
    }

    private void HandleKeeperInput()
    {
        if (keeper == null) return;

        if (Input.GetKeyDown(_jumpLeft)) keeper.JumpDirection = -1;
        else if (Input.GetKeyDown(_jumpRight)) keeper.JumpDirection = 1;
        else if (Input.GetKeyDown(_jumpCenter)) keeper.JumpDirection = 0;

        if (Input.GetKeyDown(_jumpLeft) || Input.GetKeyDown(_jumpRight) || Input.GetKeyDown(_jumpCenter))
        {
            var jumpCmd = new JumpCommand(keeper);
            EventQueueManager.Instance.AddCommand(jumpCmd);
        }
    }
}
