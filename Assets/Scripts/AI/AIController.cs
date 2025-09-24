using System.Collections;
using UnityEngine;
using PenaltyShootout.Player;

namespace PenaltyShootout.AI
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private Ball.PenaltyBall _ball;
        [SerializeField] private Transform _kickOrigin;
        [SerializeField] private float _minPower = 10f;
        [SerializeField] private float _maxPower = 16f;
        [SerializeField] private float _delayBeforeKick = 0.8f;

        private bool _enabled = false;

        public void EnableShooting(bool en)
        {
            _enabled = en;
            if (en) StartCoroutine(DoShoot());
        }

        private IEnumerator DoShoot()
        {
            yield return new WaitForSeconds(_delayBeforeKick);
            // Decide dirección: -1 left, 0 center, 1 right
            int dir = RandomDirectionWeighted();
            float lateralOffset = 3f * dir;
            Vector3 target = _ball.GoalCenterPosition + new Vector3(lateralOffset, 0.8f, 0f);
            float power = Random.Range(_minPower, _maxPower);
            _ball.Kick(_kickOrigin.position, target, power, isPlayerShot: false);
            _enabled = false;
        }

        private int RandomDirectionWeighted()
        {
            // ejemplo simple: 35% left, 30% center, 35% right
            float r = Random.value;
            if (r < 0.35f) return -1;
            if (r < 0.65f) return 0;
            return 1;
        }
    }
}
