using UnityEngine;
using System.Collections;

namespace PenaltyShootout.Player
{
    public class KeeperController : MonoBehaviour
    {
        [SerializeField] private float _diveDistance = 2.5f;
        [SerializeField] private float _diveSpeed = 8f;
        [SerializeField] private Transform _centerPosition;

        private bool _isPlayerControlled = false;
        private bool _isPreparing = false;
        private Vector3 _targetDivePos;

        public void PrepareForSave(bool playerControlled)
        {
            _isPlayerControlled = playerControlled;
            _isPreparing = true;
        }

        public void PlayerDiveInput(int direction)
        {
            // direction: -1 left, 0 center, 1 right
            if (!_isPlayerControlled || !_isPreparing) return;
            StartCoroutine(Dive(direction));
            _isPreparing = false;
        }

        public void AIDive(int direction)
        {
            if (_isPlayerControlled || !_isPreparing) return;
            StartCoroutine(Dive(direction));
            _isPreparing = false;
        }

        private IEnumerator Dive(int direction)
        {
            Vector3 target = _centerPosition.position + (Vector3.right * (_diveDistance * direction));
            float t=0f;
            Vector3 start = transform.position;
            while (t<1f)
            {
                t += Time.deltaTime * _diveSpeed;
                transform.position = Vector3.Lerp(start, target, t);
                yield return null;
            }
            // stay a little then return
            yield return new WaitForSeconds(0.5f);
            // volver centro
            t=0;
            while (t<1f)
            {
                t += Time.deltaTime * _diveSpeed;
                transform.position = Vector3.Lerp(target, start, t);
                yield return null;
            }
        }
    }
}
