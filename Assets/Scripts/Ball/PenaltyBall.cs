using UnityEngine;
using PenaltyShootout.Core;

namespace PenaltyShootout.Ball
{
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class PenaltyBall : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _maxLifetime = 5f; // tiempo máximo antes de fallar

        private GameManager _gameManager;
        private bool _inMotion = false;
        private bool _resultNotified = false;

        private void Awake()
        {
            if (!_rb) _rb = GetComponent<Rigidbody>();
            _rb.useGravity = true;
            _rb.isKinematic = false;
        }

        public void Initialize(GameManager gm)
        {
            _gameManager = gm;
        }

        /// <summary>
        /// Ejecuta un disparo con fuerza hacia el objetivo.
        /// </summary>
        public void Kick(Vector3 origin, Vector3 target, float power, bool isPlayerShot)
        {
            ResetBall(origin);

            Vector3 dir = (target - origin).normalized;
            _rb.AddForce(dir * power, ForceMode.Impulse);

            _inMotion = true;
            _resultNotified = false;

            // Si en X segundos no pasó nada, se considera fallo
            CancelInvoke();
            Invoke(nameof(NotifyMissIfNoResult), _maxLifetime);
        }

        private void ResetBall(Vector3 origin)
        {
            transform.position = origin;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        private void NotifyMissIfNoResult()
        {
            if (_resultNotified) return;
            Debug.Log("❌ Tiro fallado (se fue afuera / tiempo)");
            _gameManager.NotifyShotResult(false);
            _resultNotified = true;
            _inMotion = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_resultNotified) return;

            // ✅ Gol
            if (other.CompareTag("GoalTrigger"))
            {
                Debug.Log("⚽ ¡Gol!");
                _gameManager.NotifyShotResult(true);
                _resultNotified = true;
                _inMotion = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_resultNotified) return;

            // 🧤 Atajada
            if (collision.collider.CompareTag("Keeper"))
            {
                Debug.Log("🧤 Atajado por el arquero");
                _gameManager.NotifyShotResult(false);
                _resultNotified = true;
                _inMotion = false;
            }
        }
    }
}
