using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PenaltyShootout.Utils;

namespace PenaltyShootout.Core
{
    public enum Team { Player, AI }

    public class GameManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _initialShotsPerTeam = 5;
        [SerializeField] private float _betweenShotsDelay = 1.2f;

        [Header("References")]
        [SerializeField] private Player.PlayerInputController _playerController;
        [SerializeField] private AI.AIController _aiController;
        [SerializeField] private Ball.PenaltyBall _ball;
        [SerializeField] private Player.KeeperController _playerKeeper;
        [SerializeField] private Player.KeeperController _aiKeeper;
        [SerializeField] private UI.UIManager _uiManager;

        private int _playerScore, _aiScore;
        private int _playerShotsTaken, _aiShotsTaken;
        private int _currentShotIndex = 0;
        private bool _isPlayerKickingFirst = true;
        private Team _currentTurn;

        private void Start()
        {
            ResetMatch();
            StartCoroutine(RunShootout());
        }

        private void ResetMatch()
        {
            _playerScore = _aiScore = 0;
            _playerShotsTaken = _aiShotsTaken = 0;
            _currentShotIndex = 0;
            _isPlayerKickingFirst = true; // podrías dar opción al menu
            _currentTurn = _isPlayerKickingFirst ? Team.Player : Team.AI;
            _uiManager.UpdateScore(_playerScore, _aiScore);
            Events.OnRoundMessage?.Invoke("Comienza la tanda");
        }

        private IEnumerator RunShootout()
        {
            // Alterna hasta completar o ventaja irreversible
            while (true)
            {
                if (IsShootoutOver()) break;

                if (_currentTurn == Team.Player)
                {
                    // Prepara controles para patear; esperar la acción (block)
                    Events.OnRoundMessage?.Invoke($"Tiro {_currentShotIndex + 1} - Podés patear (SPACE)");
                    _playerController.EnableKicking(true);
                    _aiKeeper.PrepareForSave(false); // AI como arquero
                    yield return StartCoroutine(WaitForKickAndResolve(Team.Player));
                    _playerController.EnableKicking(false);
                }
                else
                {
                    Events.OnRoundMessage?.Invoke($"Tiro {_currentShotIndex + 1} - Rival pateando...");
                    _aiController.EnableShooting(true);
                    _playerKeeper.PrepareForSave(true); // jugador controla arquero
                    yield return StartCoroutine(WaitForKickAndResolve(Team.AI));
                    _aiController.EnableShooting(false);
                }

                _currentTurn = (_currentTurn == Team.Player) ? Team.AI : Team.Player;
                _currentShotIndex++;
                yield return new WaitForSeconds(_betweenShotsDelay);
            }

            // Resultado final
            string winner;
            if (_playerScore > _aiScore) winner = "Ganaste!";
            else if (_aiScore > _playerScore) winner = "Perdiste";
            else winner = "Empate (debería resolverse en muerte súbita si se implementa)";

            Events.OnRoundMessage?.Invoke($"Tanda finalizada - {winner}");
            _uiManager.ShowFinal(_playerScore, _aiScore);
        }

        private IEnumerator WaitForKickAndResolve(Team kickingTeam)
        {
            bool shotCompleted = false;
            bool wasGoal = false;

            void OnShotResultHandler(int idx, bool goal)
            {
                if (idx != _currentShotIndex) return;
                shotCompleted = true;
                wasGoal = goal;
            }

            Events.OnShotResult += OnShotResultHandler;

            // Esperar hasta que el evento diga que terminó el tiro
            while (!shotCompleted)
            {
                yield return null;
            }

            Events.OnShotResult -= OnShotResultHandler;

            // Sumar puntaje
            if (kickingTeam == Team.Player)
            {
                _playerShotsTaken++;
                if (wasGoal) _playerScore++;
            }
            else
            {
                _aiShotsTaken++;
                if (wasGoal) _aiScore++;
            }

            _uiManager.UpdateScore(_playerScore, _aiScore);
        }

        private bool IsShootoutOver()
        {
            // Regla de ventaja irreversible
            int shotsRemainingPlayer = _initialShotsPerTeam - _playerShotsTaken;
            int shotsRemainingAI = _initialShotsPerTeam - _aiShotsTaken;

            if (_playerShotsTaken >= _initialShotsPerTeam && _aiShotsTaken >= _initialShotsPerTeam)
            {
                if (_playerScore != _aiScore) return true;
                // Si empate, pasar a muerte súbita (opcional)
                _initialShotsPerTeam = 1; // cambiar para muerte súbita (simple hack)
            }

            if (_playerScore - _aiScore > shotsRemainingAI) return true;
            if (_aiScore - _playerScore > shotsRemainingPlayer) return true;

            return false;
        }

        // Método público para que Ball notifique resultado
        public void NotifyShotResult(bool wasGoal)
        {
            Events.OnShotResult?.Invoke(_currentShotIndex, wasGoal);
        }
    }
}
