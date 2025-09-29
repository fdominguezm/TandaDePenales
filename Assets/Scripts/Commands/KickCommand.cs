using UnityEngine;
using System.Collections;

public class KickCommand : ICommand
{
    private readonly IKickable _kickable;
    private readonly Transform _kickableTransform;
    private readonly Ball _ball;
    private readonly float _approachSpeed = 5f;
    private readonly float _kickDistance = 1f;
    private readonly MonoBehaviour _coroutineRunner;

    public KickCommand(IKickable kickable)
    {
        _kickable = kickable;
        _kickableTransform = (kickable as MonoBehaviour)?.transform;
        _ball = GameObject.FindObjectOfType<Ball>();
        _coroutineRunner = (kickable as MonoBehaviour); // usamos el propio objeto del jugador
    }

    public void Execute()
    {
        if (_kickable == null || _kickableTransform == null || _ball == null || _coroutineRunner == null) return;

        // Iniciar corutina para acercarse y luego patear
        _coroutineRunner.StartCoroutine(MoveAndKick());
    }

    private IEnumerator MoveAndKick()
    {
        // Mover al jugador hasta la pelota
        while (Vector3.Distance(_kickableTransform.position, _ball.transform.position) > _kickDistance)
        {
            Vector3 dir = (_ball.transform.position - _kickableTransform.position).normalized;
            _kickableTransform.position += dir * _approachSpeed * Time.deltaTime;
            yield return null;
        }

        // Patear cuando llegue a la pelota
        _ball.Kick(_kickable);
        Debug.Log($"Ball kicked! Direction: {_kickable.KickDirection}, Power: {_kickable.CurrentPower}");
        EventManager.instance.EventBallKicked(_kickable);
    }
}
