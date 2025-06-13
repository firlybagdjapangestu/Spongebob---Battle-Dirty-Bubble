using UnityEngine;
using System.Collections;

public class TrackingRocket : MonoBehaviour
{
    public float speed = 10f;
    public float rotateSpeed = 5f;
    public float damage = 50f;
    public GameObject explosionVFX;
    public float lifetime = 5f; // ⏳ auto return ke pool

    private Transform target;
    private PoolingManager pool;

    private Coroutine autoReturnRoutine;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    private void OnEnable()
    {
        // Cari PoolingManager
        if (pool == null)
            pool = GameObject.FindFirstObjectByType<PoolingManager>();

        // Auto-return ke pool setelah lifetime
        if (autoReturnRoutine != null)
            StopCoroutine(autoReturnRoutine);

        autoReturnRoutine = StartCoroutine(AutoReturnAfterLifetime());
    }

    private void OnDisable()
    {
        // Bersihkan coroutine saat dinonaktifkan
        if (autoReturnRoutine != null)
        {
            StopCoroutine(autoReturnRoutine);
            autoReturnRoutine = null;
        }
    }

    void Update()
    {
        if (!target) return;

        Vector3 dir = (target.position - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Cek jika yang kena punya komponen IDamageable
        if (other.collider.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
            pool.DeactivateObject(gameObject);
        }

        // Efek ledakan
        if (explosionVFX)
        {
            GameObject fx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            // Optional: destroy VFX atau pakai pooling juga
            Destroy(fx, 2f);
        }

        // Return ke pool
        if (pool != null)
            pool.DeactivateObject(gameObject);
        else
            Debug.LogWarning("No pooling manager found!");
    }

    private IEnumerator AutoReturnAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);

        if (pool != null)
            pool.DeactivateObject(gameObject);
        else
            Debug.LogWarning("No pooling manager found!");
    }
}
