using UnityEngine;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;     // panel awal
    private GameObject currentPanel;     // panel aktif sekarang
    private Stack<GameObject> panelHistory = new Stack<GameObject>();

    void Start()
    {
        currentPanel = mainMenuPanel;
        currentPanel.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackMenu();
        }
    }

    // Panggil ini dari tombol untuk buka panel baru
    public void OpenPanel(GameObject newPanel)
    {
        if (newPanel == null || newPanel == currentPanel) return;

        panelHistory.Push(currentPanel);     // simpan panel sekarang
        currentPanel.SetActive(false);       // nonaktifkan panel sekarang

        currentPanel = newPanel;             // aktifkan panel baru
        currentPanel.SetActive(true);
    }

    public void BackMenu()
    {
        if (panelHistory.Count > 0)
        {
            currentPanel.SetActive(false);
            currentPanel = panelHistory.Pop();
            currentPanel.SetActive(true);
        }
        else
        {
            QuitGame();
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
