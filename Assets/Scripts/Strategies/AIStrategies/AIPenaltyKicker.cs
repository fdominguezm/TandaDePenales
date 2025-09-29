using System.Collections;
using UnityEngine;

public class AIPenaltyKicker : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _kickableObject; // debe implementar IKickable
    private IKickable _kickable => _kickableObject as IKickable;

    [Header("Kick Settings")]
    [SerializeField] private float _minPower = 10f;
    [SerializeField] private float _maxPower = 20f;

    [Header("Timing")]
    [SerializeField] private float _kickDelay = 1f; // espera antes de patear
    [SerializeField] private float _randomDelayVariation = 0.3f; // +/- variación

    [Header("Team Info")]
    [SerializeField] private Team _team = Team.AI;

    private bool _isKicking = false;
    private bool _subscribedToTurn = false;

    private void Awake()
    {
        if (_kickable == null)
        {
            Debug.LogError($"{nameof(AIPenaltyKicker)} en '{gameObject.name}' necesita un componente que implemente IKickable en _kickableObject.");
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        TrySubscribeToAITurn();
    }

    private void OnDisable()
    {
        if (_subscribedToTurn && EventManager.instance != null)
        {
            EventManager.instance.OnAIKickingTurn -= HandleAITurn;
            _subscribedToTurn = false;
        }
    }

    private void TrySubscribeToAITurn()
    {
        if (EventManager.instance != null)
        {
            EventManager.instance.OnAIKickingTurn += HandleAITurn;
            _subscribedToTurn = true;
        }
        else
        {
            StartCoroutine(WaitAndSubscribe());
        }
    }

    private IEnumerator WaitAndSubscribe()
    {
        float wait = 0f;
        while (EventManager.instance == null && wait < 5f)
        {
            wait += Time.deltaTime;
            yield return null;
        }

        if (EventManager.instance != null)
        {
            EventManager.instance.OnAIKickingTurn += HandleAITurn;
            _subscribedToTurn = true;
        }
    }

    private void HandleAITurn()
    {
        if (_isKicking) return;

        PenaltyGameManager gm = FindObjectOfType<PenaltyGameManager>();
        if (gm == null)
        {
            Debug.LogWarning("AIPenaltyKicker: PenaltyGameManager no encontrado.");
            return;
        }

        if (gm.GetCurrentTurn() != _team) return;

        StartCoroutine(DoKick());
    }

    private IEnumerator DoKick()
    {
        _isKicking = true;

        float variation = Random.Range(-_randomDelayVariation, _randomDelayVariation);
        yield return new WaitForSeconds(Mathf.Max(0f, _kickDelay + variation));

        if (_kickable == null) 
        {
            _isKicking = false;
            yield break;
        }

        // Decidir tiro directamente, sin moverse hacia la pelota
        _kickable.KickDirection = Random.Range(-1, 2);
        _kickable.CurrentPower = Random.Range(_minPower, _maxPower);

        Debug.Log($"[AI Kicker] {gameObject.name} patea. Dir: {_kickable.KickDirection}, Power: {_kickable.CurrentPower}");

        KickCommand kickCmd = new KickCommand(_kickable);
        if (EventQueueManager.Instance != null)
            EventQueueManager.Instance.AddCommand(kickCmd);
        else
            kickCmd.Execute();

        _isKicking = false;
    }
}
