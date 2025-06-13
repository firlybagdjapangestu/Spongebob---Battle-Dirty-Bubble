using UnityEngine;

public class FlameImpactDamage : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private LayerMask targetLayer; // ✅ Tambahkan layer mask di Inspector

    private float lastDamageTime;

    private void OnParticleCollision(GameObject other)
    {
        // ✅ Cek interval dulu
        if (Time.time - lastDamageTime < damageInterval)
            return;

        // ✅ Cek apakah layer target sesuai
        if ((targetLayer.value & (1 << other.layer)) == 0)
            return;

        // ✅ Beri damage hanya ke layer yang cocok dan punya IDamageable
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
            lastDamageTime = Time.time;
        }
    }
}
