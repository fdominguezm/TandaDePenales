using UnityEngine;

namespace PenaltyShootout.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(0,5,-8);
        [SerializeField] private float _smooth = 6f;

        void LateUpdate()
        {
            if (_target == null) return;
            Vector3 desired = _target.position + _offset;
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * _smooth);
            transform.LookAt(_target);
        }
    }
}
