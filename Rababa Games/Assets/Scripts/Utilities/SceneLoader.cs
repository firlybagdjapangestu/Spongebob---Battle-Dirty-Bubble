public static class SceneLoader
{
    public static string targetScene;

    public static void Load(string sceneName)
    {
        targetScene = sceneName;
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
    }
}
