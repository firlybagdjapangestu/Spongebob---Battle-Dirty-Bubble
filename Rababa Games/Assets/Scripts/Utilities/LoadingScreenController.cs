using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    [SerializeField] private float minLoadTime = 2f;

    private void Start()
    {
        StartCoroutine(LoadAsync());
    }

    private IEnumerator LoadAsync()
    {
        string targetScene = SceneLoader.targetScene;
        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
        op.allowSceneActivation = false;

        float elapsed = 0f;
        float progressValue = 0f;

        while (!op.isDone)
        {
            elapsed += Time.deltaTime;

            float loadProgress = Mathf.Clamp01(op.progress / 0.9f); // max 0.9f
            float targetProgress = Mathf.Min(loadProgress, 1f);

            // Interpolasi manual progress bar ke 100% selama minLoadTime
            if (elapsed < minLoadTime)
            {
                progressValue = Mathf.Lerp(progressValue, targetProgress, Time.deltaTime * 5f);
            }
            else
            {
                progressValue = targetProgress;
            }

            if (progressBar) progressBar.value = progressValue;
            if (progressText) progressText.text = $"Loading... {progressValue * 100f:0}%";

            // Baru boleh pindah scene kalau udah 90% dan waktu minimal lewat
            if (op.progress >= 0.9f && elapsed >= minLoadTime)
            {
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
