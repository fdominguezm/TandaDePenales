using UnityEngine;

public class KeeperMovement : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour _keepableObj; // KeeperKeepable
    private IKeepable _keepable => _keepableObj as IKeepable;

    [SerializeField]
    private float jumpDistance = 3f;

    [SerializeField]
    private float jumpSpeed = 5f;

    private Vector3 _startPos;
    private Vector3 _targetPos;

    private void Start()
    {
        _startPos = transform.position;
        _targetPos = _startPos;
    }

    private void Update()
    {
        if (_keepable == null || !_keepable.HasJumped)
            return;

        _targetPos = _startPos + Vector3.right * _keepable.JumpDirection * jumpDistance;
        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPos,
            jumpSpeed * Time.deltaTime
        );
    }

    public void ResetKeeper(IKeepable keepable, Vector3 startPos)
    {
        _startPos = startPos;
        _targetPos = _startPos;
        transform.position = _startPos;

        if (keepable != null)
        {
            keepable.JumpDirection = 0;
            keepable.HasJumped = false; // reset flag
        }
    }
}
