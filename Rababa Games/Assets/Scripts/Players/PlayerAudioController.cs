using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudioController : MonoBehaviour
{
    private AudioSource audioSource;
    private PlayerStatus status;
    private bool isPlayingIdleSFX = false;

    [Header("Idle SFX Settings")]
    [SerializeField] private float idleSFXMinDelay = 3f;
    [SerializeField] private float idleSFXMaxDelay = 7f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        status = GetComponent<PlayerStatus>();
    }

    private void OnEnable()
    {
        GameEventHub.OnPlaySFX += HandlePlaySFX;
        GameEventHub.OnPlayRandomSFX += HandlePlayRandomSFX;

        StartCoroutine(IdleSFXRoutine());
    }

    private void OnDisable()
    {
        GameEventHub.OnPlaySFX -= HandlePlaySFX;
        GameEventHub.OnPlayRandomSFX -= HandlePlayRandomSFX;

        StopCoroutine(IdleSFXRoutine());
    }

    private void HandlePlaySFX(int playerId, AudioClip clip)
    {
        if (status.playerID != playerId || clip == null) return;
        audioSource.PlayOneShot(clip);
    }

    private void HandlePlayRandomSFX(int playerId, AudioClip[] clips)
    {
        if (status.playerID != playerId || clips.Length == 0) return;
        var random = clips[Random.Range(0, clips.Length)];
        audioSource.PlayOneShot(random);
    }

    private IEnumerator IdleSFXRoutine()
    {
        isPlayingIdleSFX = true;

        while (isPlayingIdleSFX)
        {
            if (status.CharacterData != null && status.CharacterData.idleSFX.Length > 0)
            {
                var randomClip = status.CharacterData.idleSFX[Random.Range(0, status.CharacterData.idleSFX.Length)];
                if (randomClip != null)
                {
                    audioSource.PlayOneShot(randomClip);
                }
            }

            float delay = Random.Range(idleSFXMinDelay, idleSFXMaxDelay);
            yield return new WaitForSeconds(delay);
        }
    }
}
