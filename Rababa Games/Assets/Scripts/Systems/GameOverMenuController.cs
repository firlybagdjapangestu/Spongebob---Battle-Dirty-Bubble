using UnityEngine;
using UnityEngine.UI;

public class GameOverMenuController : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [SerializeField] private string gameplayScene = "Gameplay";
    [SerializeField] private string mainMenuScene = "MainMenu";

    private void Start()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void RestartGame()
    {
        SceneLoader.Load(gameplayScene); // ← masuk LoadingScene dulu, lalu Gameplay
    }

    private void ReturnToMainMenu()
    {
        SceneLoader.Load(mainMenuScene); // ← masuk LoadingScene dulu, lalu MainMenu
    }
}
