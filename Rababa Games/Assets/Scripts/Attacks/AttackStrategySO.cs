using UnityEngine;

public abstract class AttackStrategySO : ScriptableObject, IAttackStrategy
{
    public abstract void Execute(BossController controller, System.Action onComplete);
}
