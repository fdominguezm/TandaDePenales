using UnityEngine;

public class Actor : MonoBehaviour, IKickable, IKeepable
{
    [SerializeField] private ActorStats _actorStats;

    // IKickable
    public float CurrentPower { get; set; }
    public int KickDirection { get; set; }

    // IKeepable
    public int JumpDirection { get; set; }
    public bool HasJumped { get; set; } = false; 

    public ActorStats ActorStats => _actorStats;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Optional: simple movement or jump method
    public void Move(Vector3 direction)
    {
        _rb.MovePosition(transform.position + direction * _actorStats.Speed * Time.deltaTime);
    }

    public void Jump(Vector3 direction)
    {
        _rb.MovePosition(transform.position + direction * _actorStats.JumpSpeed * Time.deltaTime);
    }
}
