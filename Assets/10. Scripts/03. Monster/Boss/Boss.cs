using UnityEngine;

public class Boss : Monster
{
    [Header("Boss Only")]
    [SerializeField] private BossController bossController;
    
    [Header("Phase Setting")]
    [SerializeField, Range(0f, 1f)] private float phase2Threshold = 0.5f;

    private bool phase2Triggered;
    
    public BaseStat Stat => stat;
    
    protected override void Awake()
    {
        base.Awake();
        
        bossController = GetComponent<BossController>();
        contactDamageEnabled = false;
        phase2Triggered = false;
    }

    public override void TakeDamage(int damage)
    {
        if (bossController != null && bossController.CurrentPhase == BossPhase.Dead)
            return;

        if (bossController != null)
        {
            bool isIdle = bossController.StateMachine.CurrentState == bossController.IdleState;
            if (isIdle == false)
                return;
        }

        base.TakeDamage(damage);

        if (bossController != null && bossController.CurrentPhase == BossPhase.Phase1
                                   && phase2Triggered == false)
        {
            float hpRatio = (float)Stat.CurrentHp / Stat.MaxHp;

            if (hpRatio <= phase2Threshold)
            {
                phase2Triggered = true;
                bossController.ChangePhase(BossPhase.Phase2);
                bossController.StateMachine.ChangeState(bossController.PhaseTransitionState);
            }
        }
    }
}
