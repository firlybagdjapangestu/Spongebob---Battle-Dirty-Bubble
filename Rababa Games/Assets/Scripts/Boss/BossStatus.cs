using System.Collections;
using UnityEngine;

public class BossStatus : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [SerializeField] private AudioClip bossHurt;
    [SerializeField] private Material hurtMaterial;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    private AudioSource audioSource;

    private void Awake()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }


    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Boss took {amount} damage. Remaining HP: {currentHealth}");

        GameEventHub.CameraShake(1f);

        if (bossHurt != null && audioSource != null)
        {
            audioSource.PlayOneShot(bossHurt);
        }

        StartCoroutine(BlinkMaterial());

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private IEnumerator BlinkMaterial()
    {
        int blinkCount = 4;
        float blinkDuration = 0.1f;

        for (int i = 0; i < blinkCount; i++)
        {
            meshRenderer.material = hurtMaterial;
            yield return new WaitForSeconds(blinkDuration);
            meshRenderer.material = normalMaterial;
            yield return new WaitForSeconds(blinkDuration);
        }
    }

    private void Die()
    {
        Debug.Log("Boss defeated!");
        GameEventHub.BossDefeated();
        Destroy(gameObject);
    }
}
