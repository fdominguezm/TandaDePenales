using UnityEngine;
using UnityEngine.UI;
using PenaltyShootout.Utils;

namespace PenaltyShootout.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _messageText;
        [SerializeField] private Transform _historyParent;
        [SerializeField] private GameObject _historyItemPrefab;

        private void OnEnable()
        {
            Events.OnScoreChanged += UpdateScore;
            Events.OnRoundMessage += ShowMessage;
            Events.OnShotResult += AddShotHistory;
        }

        private void OnDisable()
        {
            Events.OnScoreChanged -= UpdateScore;
            Events.OnRoundMessage -= ShowMessage;
            Events.OnShotResult -= AddShotHistory;
        }

        public void UpdateScore(int player, int ai)
        {
            _scoreText.text = $"{player} - {ai}";
            Events.OnScoreChanged?.Invoke(player, ai);
        }

        public void ShowMessage(string msg)
        {
            _messageText.text = msg;
        }

        private void AddShotHistory(int idx, bool wasGoal)
        {
            if (_historyItemPrefab == null || _historyParent == null) return;
            var go = Instantiate(_historyItemPrefab, _historyParent);
            var txt = go.GetComponentInChildren<Text>();
            txt.text = $"Tiro {idx + 1}: {(wasGoal ? "Gol" : "No gol")}";
        }

        public void ShowFinal(int player, int ai)
        {
            ShowMessage($"Final: {player} - {ai}");
        }
    }
}
