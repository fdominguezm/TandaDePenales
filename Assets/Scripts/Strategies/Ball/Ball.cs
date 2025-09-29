using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    private Rigidbody _rb;
    private bool _isKicked = false;
    private IKickable _currentKicker;

    [Header("Physics Settings")]
    [SerializeField] private float _kickForceMultiplier = 1f;

    [Header("Goal Info")]
    [SerializeField] private Transform _startPosition;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Kick(IKickable kicker)
    {
        if (_isKicked || kicker == null) return;

        PenaltyGameManager manager = FindObjectOfType<PenaltyGameManager>();
        Team currentTurn = manager.GetCurrentTurn();

        if ((currentTurn == Team.Player && kicker is PlayerKickable) ||
            (currentTurn == Team.AI && kicker is AIKickable))
        {
            _currentKicker = kicker;
            _isKicked = true;

            // Reducir el ángulo lateral
            float lateralFactor = 0.3f; 
            float clampedX = Mathf.Clamp(kicker.KickDirection, -1f, 1f) * lateralFactor;

            Vector3 direction = new Vector3(clampedX, 0f, 1f).normalized;
            _rb.velocity = direction * kicker.CurrentPower * _kickForceMultiplier;

            EventManager.instance.EventBallKicked(kicker);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isKicked) return;

        if (collision.gameObject.CompareTag("Keeper"))
        {
            
        }
    }

    public void ResetBall()
    {
        _isKicked = false;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        if (_startPosition != null)
        {
            transform.position = _startPosition.position;
            transform.rotation = _startPosition.rotation;
        }
    }

    public Team GetTeamFromKicker()
    {
        if (_currentKicker == null) return Team.Player;
        if (_currentKicker is MonoBehaviour mb && mb.CompareTag("Player")) return Team.Player;
        return Team.AI;
    }

    public bool IsKicked => _isKicked;
}
