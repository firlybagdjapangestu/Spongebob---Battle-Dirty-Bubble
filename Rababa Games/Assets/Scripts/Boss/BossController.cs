using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform[] patrolPoints;
    public Transform groundPoint;
    public float flySpeed;
    [Range(0f, 1f)] public float landingChance;

    [Header("Attack Settings")]
    public AttackStrategySO[] attackStrategies;
    public Transform firePoint;

    private IBossState currentState;
    private FlyState flyState;

    private void Start()
    {
        flyState = new FlyState(this);
        TransitionToState(flyState);
    }

    private void Update()
    {
        currentState?.OnUpdate();
    }

    public void TransitionToState(IBossState newState)
    {
        if (newState == null) return;

        currentState?.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }

    public void OnMovementFinished()
    {
        if (attackStrategies == null || attackStrategies.Length == 0)
        {
            Debug.LogWarning("No attack strategies assigned to the boss.");
            return;
        }

        if (!HasAlivePlayers())
        {
            Debug.Log("❌ Semua player sudah mati. Boss tidak akan menyerang.");
            return;
        }

        var selectedAttack = attackStrategies[Random.Range(0, attackStrategies.Length)];
        TransitionToState(new AttackState(this, selectedAttack));
    }

    public void OnAttackFinished()
    {
        TransitionToState(flyState);
    }

    private bool HasAlivePlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p != null && p.activeInHierarchy)
                return true;
        }

        return false;
    }
}
