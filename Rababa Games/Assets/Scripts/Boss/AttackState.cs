public class AttackState : IBossState
{
    private BossController controller;
    private AttackStrategySO attack;

    public AttackState(BossController controller, AttackStrategySO attack)
    {
        this.controller = controller;
        this.attack = attack;
    }

    public void OnEnter()
    {
        attack.Execute(controller, controller.OnAttackFinished);
    }

    public void OnUpdate() { }
    public void OnExit() { }
}
