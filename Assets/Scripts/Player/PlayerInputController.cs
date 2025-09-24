using System.Collections;
using UnityEngine;
using PenaltyShootout.Utils;
using PenaltyShootout.Core;

namespace PenaltyShootout.Player
{
    public class PlayerInputController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Ball.PenaltyBall _ball;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private Transform _kickOrigin;

        [Header("Settings")]
        [SerializeField] private float _minPower = 6f;
        [SerializeField] private float _maxPower = 18f;
        [SerializeField] private float _powerChargeRate = 6f;

        private bool _canKick = false;
        private float _currentPower;
        private bool _isCharging = false;

        public void EnableKicking(bool enabled)
        {
            _canKick = enabled;
            _currentPower = _minPower;
            _isCharging = false;
        }

        private void Update()
        {
            if (!_canKick) return;

            // Dirección: izquierda/derecha/centro con flechas
            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.LeftArrow)) dir = Vector3.left;
            else if (Input.GetKey(KeyCode.RightArrow)) dir = Vector3.right;
            else dir = Vector3.forward; // centro

            // Comenzar a cargar potencia con mantener tecla Space
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isCharging = true;
                _currentPower = _minPower;
            }

            if (Input.GetKey(KeyCode.Space) && _isCharging)
            {
                _currentPower += _powerChargeRate * Time.deltaTime;
                _currentPower = Mathf.Min(_currentPower, _maxPower);
            }

            if (Input.GetKeyUp(KeyCode.Space) && _isCharging)
            {
                _isCharging = false;
                // Effectuar kick
                _ball.Kick(_kickOrigin.position, GetTargetPosition(dir), _currentPower, isPlayerShot: true);
                _canKick = false; // evitar re-kicks
            }

            // Si el usuario solo presiona Space sin direccion -> centro (arriba se maneja con dir)
        }

        private Vector3 GetTargetPosition(Vector3 dir)
        {
            // calcula objetivo con un offset lateral y altura
            float lateralOffset = 3f * dir.x; // izquierda -3, derecha +3, centro 0
            Vector3 goalCenter = _ball.GoalCenterPosition; // público en Ball
            return goalCenter + new Vector3(lateralOffset, 0.8f, 0f);
        }
    }
}
