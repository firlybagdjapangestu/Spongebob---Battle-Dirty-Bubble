using UnityEngine;
using System.Collections;

public class EagleImpactDamage : MonoBehaviour
{
    [SerializeField] private float damage = 100f;
    [SerializeField] private float lifetime = 0.5f; 
    [SerializeField] private float forceAmount = 2f; 

    private bool hasHit = false;
    private PoolingManager pool;

    private void OnEnable()
    {
        hasHit = false;

        if (pool == null)
            pool = GameObject.FindFirstObjectByType<PoolingManager>();

        StartCoroutine(AutoDisable());
    }

    private IEnumerator AutoDisable()
    {
        yield return new WaitForSeconds(lifetime);

        if (!hasHit)
        {
            pool.DeactivateObject(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            Debug.Log($"EagleImpactDamage hit {other.gameObject.name} for {damage} damage.");
            damageable.TakeDamage(damage);
            hasHit = true;

            // Dorong player jika ada Rigidbody
            if (other.TryGetComponent<Rigidbody>(out var rb))
            {
                Debug.Log($"Applying force to {other.gameObject.name}.");
                Vector3 direction = (other.transform.position - transform.position).normalized;
                rb.AddForce(direction * forceAmount, ForceMode.Impulse);
            }

            pool.DeactivateObject(gameObject);
        }
    }

}
