using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Attack/EagleAttack")]
public class EagleAttackSO : AttackStrategySO
{
    [SerializeField] private float ascendHeight = 30f;
    [SerializeField] private float ascendSpeed = 10f;
    [SerializeField] private float diveDelay = 1.5f;
    [SerializeField] private float diveSpeed = 40f;
    [SerializeField] private GameObject shadowPrefab;
    [SerializeField] private GameObject impactVFX;
    [SerializeField] private float groundY = 0f;
    [SerializeField] private GameObject impactZonePrefab;

    private PoolingManager pool;

    public override void Execute(BossController controller, System.Action onComplete)
    {
        controller.StartCoroutine(EagleAttackRoutine(controller, onComplete));
    }

    private IEnumerator EagleAttackRoutine(BossController controller, System.Action onComplete)
    {
        if (pool == null)
            pool = GameObject.FindFirstObjectByType<PoolingManager>();

        Transform boss = controller.transform;

        // 1. Ascend  
        while (boss.position.y < ascendHeight)
        {
            boss.position += Vector3.up * ascendSpeed * Time.deltaTime;
            yield return null;
        }

        // 2. Lock Target  
        Transform target = FindClosestPlayer(controller);
        if (target == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        Vector3 targetPos = new Vector3(target.position.x, groundY, target.position.z);

        // 3. Show Shadow  
        GameObject shadow = pool.ActiveObject(shadowPrefab, targetPos, Quaternion.identity);
        yield return new WaitForSeconds(diveDelay);

        // 4. Position above player  
        boss.position = new Vector3(targetPos.x, ascendHeight, targetPos.z);

        // 5. Slam down  
        while (boss.position.y > groundY)
        {
            boss.position += Vector3.down * diveSpeed * Time.deltaTime;
            yield return null;
        }


        if (impactVFX != null)
        {
            GameObject.Instantiate(impactVFX, boss.position, Quaternion.identity);
        }
        GameEventHub.CameraShake(2f);
        pool.DeactivateObject(shadow);
        GameObject impact = pool.ActiveObject(impactZonePrefab, boss.position, Quaternion.identity);

        onComplete?.Invoke();
    }

    private Transform FindClosestPlayer(BossController controller)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDist = Mathf.Infinity;
        Transform closest = null;
        foreach (GameObject p in players)
        {
            float dist = Vector3.Distance(p.transform.position, controller.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = p.transform;
            }
        }
        return closest;
    }

}
