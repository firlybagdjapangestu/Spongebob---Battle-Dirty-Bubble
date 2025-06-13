using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ReadyToPlay : MonoBehaviour
{
    private bool isPlayer1Ready = false;
    private bool isPlayer2Ready = false;

    [SerializeField] private TextMeshProUGUI player1ReadyText;
    [SerializeField] private TextMeshProUGUI player2ReadyText;

    [SerializeField] private string gameplaySceneName = "Gameplay";
    [SerializeField] private float blinkSpeed = 0.5f;

    private float blinkTimer1 = 0f;
    private float blinkTimer2 = 0f;
    private bool showText1 = true;
    private bool showText2 = true;

    private void Start()
    {
        if (player1ReadyText != null)
        {
            player1ReadyText.text = "Player 1: Press W to Ready";
            player1ReadyText.color = Color.white;
        }
        if (player2ReadyText != null)
        {
            player2ReadyText.text = "Player 2: Press Up Arrow to Ready";
            player2ReadyText.color = Color.white;
        }
    }

    private void Update()
    {
        // Player 1
        if (!isPlayer1Ready)
        {
            BlinkText(player1ReadyText, ref blinkTimer1, ref showText1);
            if (Input.GetKeyDown(KeyCode.W))
            {
                isPlayer1Ready = true;
                player1ReadyText.text = "Player 1: READY!";
                player1ReadyText.color = Color.green;
                player1ReadyText.enabled = true;
                Debug.Log("Player 1 is READY!");
            }
        }

        // Player 2
        if (!isPlayer2Ready)
        {
            BlinkText(player2ReadyText, ref blinkTimer2, ref showText2);
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                isPlayer2Ready = true;
                player2ReadyText.text = "Player 2: READY!";
                player2ReadyText.color = Color.green;
                player2ReadyText.enabled = true;
                Debug.Log("Player 2 is READY!");
            }
        }

        // Check if both ready
        if (isPlayer1Ready && isPlayer2Ready)
        {
            Debug.Log("🎮 Both players ready. Loading gameplay scene...");
            SceneLoader.Load(gameplaySceneName); 
        }
    }

    private void BlinkText(TextMeshProUGUI text, ref float timer, ref bool show)
    {
        if (text == null) return;

        timer += Time.deltaTime;
        if (timer >= blinkSpeed)
        {
            show = !show;
            text.enabled = show;
            timer = 0f;
        }
    }

    private void OnDisable()
    {
        isPlayer1Ready = false;
        isPlayer2Ready = false;

        if (player1ReadyText != null)
        {
            player1ReadyText.text = "Player 1: Press W to Ready";
            player1ReadyText.color = Color.white;
            player1ReadyText.enabled = true;
        }

        if (player2ReadyText != null)
        {
            player2ReadyText.text = "Player 2: Press Up Arrow to Ready";
            player2ReadyText.color = Color.white;
            player2ReadyText.enabled = true;
        }
    }
}
