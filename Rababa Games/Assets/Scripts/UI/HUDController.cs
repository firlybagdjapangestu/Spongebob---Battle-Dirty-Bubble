using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] private Image[] player1Hearts;
    [SerializeField] private Image[] player2Hearts;

    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;

    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;

    private int player1Score = 0;
    private int player2Score = 0;

    private int totalPlayers = 2;
    private int deadPlayersCount = 0;

    private GameSaveData saveData;

    private void Start()
    {
        saveData = SaveManager.Load();
        player1Score = saveData.player1Score;
        player2Score = saveData.player2Score;

        player1ScoreText.text = "Player 1 : " + player1Score.ToString();
        player2ScoreText.text = "Player 2 : " + player2Score.ToString();
    }

    private void OnEnable()
    {
        GameEventHub.OnPlayerDamaged += UpdateHealthBar;
        GameEventHub.OnBossDefeated += OnBossDefeated;
        GameEventHub.OnPlayerDied += OnPlayerDied;
        GameEventHub.OnPlayerScored += UpdateScoreUI;
    }

    private void OnDisable()
    {
        GameEventHub.OnPlayerDamaged -= UpdateHealthBar;
        GameEventHub.OnBossDefeated -= OnBossDefeated;
        GameEventHub.OnPlayerDied -= OnPlayerDied;
        GameEventHub.OnPlayerScored -= UpdateScoreUI;
    }

    private void UpdateHealthBar(float currentHealth, int playerId)
    {
        Image[] hearts = null;

        if (playerId == 1) hearts = player1Hearts;
        else if (playerId == 2) hearts = player2Hearts;
        else
        {
            Debug.LogWarning($"[UI] Received unknown playerId: {playerId}");
            return;
        }

        float remaining = currentHealth;
        for (int i = 0; i < hearts.Length; i++)
        {
            if (remaining >= 100f)
                hearts[i].fillAmount = 1f;
            else if (remaining > 0)
                hearts[i].fillAmount = remaining / 100f;
            else
                hearts[i].fillAmount = 0f;

            remaining -= 100f;
        }
    }

    private void UpdateScoreUI(int playerId, int score)
    {
        if (playerId == 1)
        {
            player1Score += score;
            player1ScoreText.text = "Player 1 : " + player1Score.ToString();
        }
        else if (playerId == 2)
        {
            player2Score += score;
            player2ScoreText.text = "Player 2 : " + player2Score.ToString();
        }
        else
        {
            Debug.LogWarning($"[UI] Unknown playerId: {playerId} when updating score");
        }

        Debug.Log($"[UI] Player {playerId} scored {score}, total score updated.");
    }

    private void OnBossDefeated()
    {
        Debug.Log("🏆 WIN! Boss defeated!");

        // Simpan skor hanya jika lebih tinggi
        GameSaveData newSave = new GameSaveData
        {
            player1Score = player1Score,
            player2Score = player2Score
        };
        SaveManager.Save(newSave);

        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);
            gameOverText.text = $"You Win!\nP1: {player1Score}  P2: {player2Score}";
            gameOverText.color = Color.green;
        }
    }

    private void OnPlayerDied(int playerId)
    {
        deadPlayersCount++;
        Debug.Log($"☠ Player {playerId} died. Total dead: {deadPlayersCount}");

        if (deadPlayersCount >= totalPlayers)
        {
            ShowGameOver("You Lose! All Players Defeated!", Color.red);
        }
    }

    private void ShowGameOver(string message, Color color)
    {
        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);
            gameOverText.text = message;
            gameOverText.color = color;
        }
    }
}
