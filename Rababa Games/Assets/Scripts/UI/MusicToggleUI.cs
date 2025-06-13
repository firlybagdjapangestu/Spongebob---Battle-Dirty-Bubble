using UnityEngine;
using UnityEngine.UI;

public class MusicToggleUI : MonoBehaviour
{
    [SerializeField] private Toggle musicToggle;

    private void Start()
    {
        if (musicToggle != null)
        {
            musicToggle.isOn = SimpleAudioSettingManager.Instance.IsMusicEnabled();
            musicToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        SimpleAudioSettingManager.Instance.ApplyAudioSettings();
    }

    private void OnToggleValueChanged(bool isOn)
    {
        SimpleAudioSettingManager.Instance.SetMusicEnabled(isOn);
    }
}
