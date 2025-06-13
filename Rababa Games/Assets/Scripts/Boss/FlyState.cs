using UnityEngine;

public class FlyState : IBossState
{
    private readonly BossController controller;
    private float flyTimer;
    private float switchTargetTimer;

    private const float flyDuration = 5f;
    private const float targetSwitchInterval = 2f;

    private Transform currentTarget;
    private bool isLanding = false;

    public FlyState(BossController controller)
    {
        this.controller = controller;
    }

    public void OnEnter()
    {
        Debug.Log("Boss started flying...");

        flyTimer = 0f;
        switchTargetTimer = 0f;

        // Tentukan apakah boss akan mendarat
        isLanding = Random.value < controller.landingChance;
        currentTarget = isLanding ? controller.groundPoint : GetRandomPatrolPoint();

        if (isLanding)
            Debug.Log("Boss decided to land!");
    }

    public void OnUpdate()
    {
        flyTimer += Time.deltaTime;
        switchTargetTimer += Time.deltaTime;

        if (!isLanding && switchTargetTimer >= targetSwitchInterval)
        {
            currentTarget = GetRandomPatrolPoint();
            switchTargetTimer = 0f;
        }

        MoveToTarget();

        if (flyTimer >= flyDuration)
        {
            controller.OnMovementFinished();
        }
    }

    public void OnExit()
    {
        Debug.Log("Boss finished flying.");
    }

    private void MoveToTarget()
    {
        if (currentTarget == null) return;

        Vector3 currentPosition = controller.transform.position;
        Vector3 targetPosition = currentTarget.position;

        controller.transform.position = Vector3.MoveTowards(
            currentPosition,
            targetPosition,
            controller.flySpeed * Time.deltaTime
        );
    }

    private Transform GetRandomPatrolPoint()
    {
        if (controller.patrolPoints == null || controller.patrolPoints.Length == 0)
            return null;

        int randomIndex = Random.Range(0, controller.patrolPoints.Length);
        return controller.patrolPoints[randomIndex];
    }
}
