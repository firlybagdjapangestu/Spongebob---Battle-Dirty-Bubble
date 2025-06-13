using System;
using UnityEngine;

public static class GameEventHub
{
    // 📦 Health & Damage
    public static event Action<float, int> OnPlayerDamaged; // float = sisa HP, int = playerId
    public static void PlayerDamaged(float currentHealth, int playerId)
    {
        OnPlayerDamaged?.Invoke(currentHealth, playerId);
    }

    // 📦 Camera Shake
    public static event Action<float> OnCameraShake;
    public static void CameraShake(float intensity = 1f)
    {
        OnCameraShake?.Invoke(intensity);
    }

    // 📦 Boss Stuff
    public static event Action OnBossDamaged;
    public static void BossDamaged()
    {
        OnBossDamaged?.Invoke();
    }

    public static event Action OnBossDefeated; 
    public static void BossDefeated()
    {
        OnBossDefeated?.Invoke();
    }

    public static event Action<int, AudioClip> OnPlaySFX;
    public static void PlaySFX(int playerId, AudioClip clip)
    {
        OnPlaySFX?.Invoke(playerId, clip);
    }

    public static event Action<int, AudioClip[]> OnPlayRandomSFX;
    public static void PlayRandomSFX(int playerId, AudioClip[] clips)
    {
        OnPlayRandomSFX?.Invoke(playerId, clips);
    }

    public static event Action<int> OnPlayerDied;
    public static void PlayerDied(int playerId)
    {
        OnPlayerDied?.Invoke(playerId);
    }

    public static event Action<int, int> OnPlayerScored;
    // int = playerID, int = scoreYangDidapat
    public static void PlayerScored(int playerId, int score)
    {
        OnPlayerScored?.Invoke(playerId, score);
    }


}
