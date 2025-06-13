public interface IAttackStrategy
{
    void Execute(BossController controller, System.Action onComplete);
}