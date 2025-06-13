using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Attack/RocketAttack")]
public class RocketAttackSO : AttackStrategySO
{
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private GameObject passiveRocketPrefab;
    [SerializeField] private float lockOnTime = 1.5f;
    [SerializeField] private float launchDelay = 0.5f;

    [SerializeField] private float targetFlyHeight = 5f;

    private PoolingManager pool;

    public override void Execute(BossController controller, System.Action onComplete)
    {
        controller.StartCoroutine(RocketAttackRoutine(controller, onComplete));
    }

    private IEnumerator RocketAttackRoutine(BossController controller, System.Action onComplete)
    {
        // Cari pool manager kalau belum ada  
        if (pool == null)
            pool = GameObject.FindFirstObjectByType<PoolingManager>();

        Transform boss = controller.transform;

        // Boss terbang dulu  
        if (boss.position.y < targetFlyHeight - 0.1f)
        {
            while (boss.position.y < targetFlyHeight - 0.05f)
            {
                Vector3 flyTarget = new Vector3(boss.position.x, targetFlyHeight, boss.position.z);
                boss.position = Vector3.MoveTowards(boss.position, flyTarget, controller.flySpeed * Time.deltaTime);
                yield return null;
            }
            boss.position = new Vector3(boss.position.x, targetFlyHeight, boss.position.z);
        }

        // Lock on players  
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0)
        {
            onComplete?.Invoke();
            yield break;
        }

        yield return new WaitForSeconds(lockOnTime);

        // Fire tracking rockets  
        int playerCount = Mathf.Min(players.Length, 2);

        for (int i = 0; i < playerCount; i++)
        {
            GameObject rocket = pool.ActiveObject(rocketPrefab, controller.firePoint.position, Quaternion.identity);
            if (rocket != null && rocket.TryGetComponent(out TrackingRocket rocketComp))
            {
                rocketComp.SetTarget(players[i].transform);
            }
            else
            {
                Debug.LogWarning("Rocket missing TrackingRocket component or null.");
            }

            yield return new WaitForSeconds(launchDelay);
        }

        // Drop passive rocket  
        GameObject passiveRocket = pool.ActiveObject(passiveRocketPrefab, controller.firePoint.position, Quaternion.identity);
        if (passiveRocket != null && passiveRocket.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.down * 5f; // Updated to use linearVelocity  
        }

        onComplete?.Invoke();
    }
}
