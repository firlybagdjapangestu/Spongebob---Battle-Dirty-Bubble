using UnityEngine;
using System.Collections;

public class PickupRocket : MonoBehaviour
{
    public float throwForce = 20f;
    private bool isHeld = false;
    private Transform holder; // 🧍 Referensi player pemegang

    private Rigidbody rb;
    private PoolingManager pool;

    [SerializeField] private Vector3 offset = new Vector3(0, 1f, 0);
    [SerializeField] private float lifetimeIfNotPicked = 10f;

    private Coroutine lifetimeCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        isHeld = false;
        holder = null;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        gameObject.layer = LayerMask.NameToLayer("Enemy");

        if (pool == null)
            pool = GameObject.FindFirstObjectByType<PoolingManager>();

        if (lifetimeCoroutine != null)
            StopCoroutine(lifetimeCoroutine);

        lifetimeCoroutine = StartCoroutine(StartLifetimeCountdown());
    }

    private void OnDisable()
    {
        if (lifetimeCoroutine != null)
            StopCoroutine(lifetimeCoroutine);
    }

    private IEnumerator StartLifetimeCountdown()
    {
        yield return new WaitForSeconds(lifetimeIfNotPicked);

        if (!isHeld && pool != null)
        {
            pool.DeactivateObject(gameObject);
        }
    }

    private void Update()
    {
        if (isHeld && holder)
        {
            transform.position = holder.position + offset;
        }
    }

    public void PickUp(Transform player)
    {
        if (isHeld) return;

        isHeld = true;
        holder = player;

        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
            lifetimeCoroutine = null;
        }

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetParent(holder);
        transform.localPosition = offset;
        gameObject.layer = LayerMask.NameToLayer("Player");

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetHeldRocket(this);
        }
    }

    public void Throw(Vector3 direction)
    {
        if (!isHeld) return;

        isHeld = false;
        transform.SetParent(null);

        rb.isKinematic = false;
        rb.linearVelocity = direction * throwForce;

        // ✅ Masih simpan holder saat dilempar
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isHeld && other.CompareTag("Player"))
        {
            PickUp(other.transform);
            return;
        }

        if (!isHeld && other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(100f);
            Debug.Log("Rocket hit and dealt damage!");

            // ✅ Tambahkan score ke player jika boss terkena via Event
            if (holder != null)
            {
                PlayerStatus status = holder.GetComponent<PlayerStatus>();
                if (status != null)
                {
                    GameEventHub.PlayerScored(status.playerID, 100); // 🎯 Misal score 100
                }
            }

            if (pool != null)
                pool.DeactivateObject(gameObject);
            else
                Debug.LogWarning("No pooling manager found!");

            holder = null; // ✅ Clear setelah kena
        }
    }
}
