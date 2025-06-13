using System.IO;
using UnityEngine;

public class SimpleAudioSettingManager : MonoBehaviour
{
    public static SimpleAudioSettingManager Instance { get; private set; }

    private string savePath;
    private AudioSettingData audioData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "audio_settings.json");
        LoadAudioSettings();
        ApplyAudioSettings();
    }

    public bool IsMusicEnabled()
    {
        return audioData?.isMusicEnabled ?? true;
    }

    public void SetMusicEnabled(bool isEnabled)
    {
        audioData.isMusicEnabled = isEnabled;
        SaveAudioSettings();
        ApplyAudioSettings();
    }

    private void LoadAudioSettings()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            audioData = JsonUtility.FromJson<AudioSettingData>(json);
        }
        else
        {
            audioData = new AudioSettingData(); // default true
            SaveAudioSettings();
        }
    }

    private void SaveAudioSettings()
    {
        string json = JsonUtility.ToJson(audioData, true);
        File.WriteAllText(savePath, json);
    }

    public void ApplyAudioSettings()
    {
        bool mute = !audioData.isMusicEnabled;
        AudioSource[] sources = Object.FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (AudioSource source in sources)
        {
            source.mute = mute;
        }
    }
}
