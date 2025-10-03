using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField]
    private TextMeshProUGUI playerMessage;

    [SerializeField]
    private Image[] playerPenaltyScores;

    [Header("AI UI")]
    [SerializeField]
    private TextMeshProUGUI aiMessage;

    [SerializeField]
    private Image[] aiPenaltyScores;

    [Header("Sprites")]
    [SerializeField]
    private Sprite emptyAttempt;

    [SerializeField]
    private Sprite goalAttempt;

    [SerializeField]
    private Sprite missAttempt;

    public void InitializeUI()
    {
        ResetPenaltyScores();
        playerMessage.text = "";
        aiMessage.text = "";
    }

    public void ResetPenaltyScores()
    {
        foreach (var img in playerPenaltyScores)
            img.sprite = emptyAttempt;

        foreach (var img in aiPenaltyScores)
            img.sprite = emptyAttempt;
    }

    public void UpdatePlayerMessage(string message)
    {
        playerMessage.text = message;
    }

    public void UpdateAIMessage(string message)
    {
        aiMessage.text = message;
    }

    public void SetPlayerAttempt(int index, bool isGoal)
    {

        if (index < 0 || index >= playerPenaltyScores.Length)
            return;
        playerPenaltyScores[index].sprite = isGoal ? goalAttempt : missAttempt;
        if (isGoal)
        {
            playerMessage.text = "Player Scores!";
        }
        else
        {
            playerMessage.text = "Player Misses!";
        }
    }

    public void SetAIAttempt(int index, bool isGoal)
    {

        if (index < 0 || index >= aiPenaltyScores.Length)
            return;
        aiPenaltyScores[index].sprite = isGoal ? goalAttempt : missAttempt;
        if (isGoal)
        {
            aiMessage.text = "AI Scores!";
        }
        else
        {
            aiMessage.text = "AI Misses!";
        }
    }

    public void EnableSuddenDeathMode()
    {
        ResetPenaltyScores();

        // Player
        for (int i = 1; i < playerPenaltyScores.Length; i++) // índice 1 = 2do slot
        {
            playerPenaltyScores[i].gameObject.SetActive(false);
        }

        // AI
        for (int i = 1; i < aiPenaltyScores.Length; i++)
        {
            aiPenaltyScores[i].gameObject.SetActive(false);
        }
    }

    public void ResetTexts()
    {
        playerMessage.text = "";
        aiMessage.text = "";
    }
}
