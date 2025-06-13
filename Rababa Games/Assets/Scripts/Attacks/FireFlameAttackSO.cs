using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Boss/Attack/Fire Flame Attack")]
public class FireFlameAttackSO : AttackStrategySO
{
    public float chaseSpeed = 4f;
    public float attackDistance = 5f;
    public float flyHeight = 7f;
    public float beamDuration = 2f;

    public GameObject fireBeamPrefab;
    [SerializeField] private float trackingSpeed = 2f;

    private PoolingManager pool;

    public override void Execute(BossController controller, System.Action onComplete)
    {
        controller.StartCoroutine(FireFlameRoutine(controller, onComplete));
    }

    private IEnumerator FireFlameRoutine(BossController controller, System.Action onComplete)
    {
        if (pool == null)
            pool = GameObject.FindFirstObjectByType<PoolingManager>();

        Transform target = FindClosestPlayer(controller);
        if (target == null)
        {
            Debug.Log("❌ Tidak ada player yang hidup. Serangan dibatalkan.");
            onComplete?.Invoke();
            yield break;
        }

        Vector3 pos = controller.transform.position;

        // ✈️ Naik dulu kalau belum cukup tinggi
        if (pos.y < flyHeight - 0.1f)
        {
            Debug.Log("⬆ Boss naik dulu sebelum FireFlame.");

            while (controller.transform.position.y < flyHeight - 0.05f)
            {
                Vector3 flyTarget = new Vector3(pos.x, flyHeight, pos.z);
                controller.transform.position = Vector3.MoveTowards(controller.transform.position, flyTarget, controller.flySpeed * Time.deltaTime);
                yield return null;
            }

            controller.transform.position = new Vector3(controller.transform.position.x, flyHeight, controller.transform.position.z);
        }

        float fixedY = controller.transform.position.y;

        // 🏃 Kejar player
        while (
            target != null &&
            target.gameObject.activeInHierarchy &&
            Vector3.Distance(FlatPosition(controller.transform.position), FlatPosition(target.position)) > attackDistance)
        {
            Vector3 dir = (FlatPosition(target.position) - FlatPosition(controller.transform.position)).normalized;
            Vector3 move = new Vector3(dir.x, 0, dir.z);
            controller.transform.position += move * chaseSpeed * Time.deltaTime;

            Vector3 lookDir = new Vector3(dir.x, 0, dir.z);
            if (lookDir != Vector3.zero)
                controller.transform.forward = Vector3.Slerp(controller.transform.forward, lookDir, 5f * Time.deltaTime);

            controller.transform.position = new Vector3(controller.transform.position.x, fixedY, controller.transform.position.z);

            yield return null;
        }

        // ❌ Target hilang saat ngejar
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Debug.Log("❌ Target mati saat dikejar. Serangan dibatalkan.");
            onComplete?.Invoke();
            yield break;
        }

        // 🔥 Fire beam
        GameObject beam = pool.ActiveObject(fireBeamPrefab, controller.firePoint.position, controller.transform.rotation);
        beam.transform.SetParent(controller.firePoint);
        beam.transform.localPosition = Vector3.zero;
        beam.transform.localRotation = Quaternion.identity;

        float t = 0f;
        while (t < beamDuration)
        {
            t += Time.deltaTime;

            if (target == null || !target.gameObject.activeInHierarchy)
            {
                Debug.Log("❌ Target mati saat beam aktif. Stop tracking.");
                break;
            }

            Vector3 dirToPlayer = (target.position - controller.firePoint.position).normalized;

            if (dirToPlayer != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
                controller.firePoint.rotation = Quaternion.Slerp(controller.firePoint.rotation, targetRot, trackingSpeed * Time.deltaTime);
            }

            yield return null;
        }

        beam.transform.SetParent(null);
        pool.DeactivateObject(beam);

        onComplete?.Invoke();
    }

    private Vector3 FlatPosition(Vector3 pos)
    {
        return new Vector3(pos.x, 0, pos.z);
    }

    private Transform FindClosestPlayer(BossController controller)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var p in players)
        {
            if (p == null || !p.activeInHierarchy)
                continue;

            float dist = Vector3.Distance(controller.transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = p.transform;
            }
        }

        return closest;
    }
}
