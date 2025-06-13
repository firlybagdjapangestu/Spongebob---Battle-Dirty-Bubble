using System;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, IDamageable
{
    public CharacterData CharacterData;

    [HideInInspector] public GameObject characterPrefab;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public float maxHealth = 300f;
    [HideInInspector] public AudioClip hurtSFX;
    [HideInInspector] public AudioClip attackSFX;
    [HideInInspector] public AudioClip[] idleSFX;

    private float currentHealth;
    private int score = 0; // ✅ Tambahkan score

    public int playerID = 1;

    private void Awake()
    {
        if (CharacterData != null && CharacterData.characterPrefabs != null)
        {
            maxHealth = CharacterData.health > 0 ? CharacterData.health : maxHealth;
            characterPrefab = Instantiate(CharacterData.characterPrefabs, transform);
            characterPrefab.transform.localPosition = Vector3.zero;
            characterPrefab.transform.localRotation = Quaternion.identity;

            hurtSFX = CharacterData.hurtSFX;
            attackSFX = CharacterData.attackSFX;
            idleSFX = CharacterData.idleSFX;

            Animator = characterPrefab.GetComponent<Animator>();
            if (Animator == null)
            {
                Debug.LogWarning("Animator not found on character prefab!");
            }
        }
        else
        {
            Debug.LogError("CharacterData or prefab is missing on " + gameObject.name);
        }

        currentHealth = maxHealth;
        BroadcastHealth();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"{gameObject.name} took {amount} damage. Remaining: {currentHealth}");

        GameEventHub.PlaySFX(playerID, CharacterData.hurtSFX);
        BroadcastHealth();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void BroadcastHealth()
    {
        float percent = currentHealth / maxHealth;
        GameEventHub.PlayerDamaged(currentHealth, playerID);
        GameEventHub.CameraShake(0.5f);
    }

    private void Die()
    {
        GameEventHub.PlayerDied(playerID);
        Destroy(gameObject);
    }

    // ✅ Tambahkan sistem score
    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log($"[Player {playerID}] +{amount} score! Total: {score}");
    }

    public int GetScore()
    {
        return score;
    }
}
