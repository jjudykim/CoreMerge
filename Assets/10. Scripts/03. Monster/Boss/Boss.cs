using System.Collections;
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

    public override int TakeDamage(int damage)
    {
        if (bossController != null && bossController.CurrentPhase == BossPhase.Dead)
            return 0;

        if (bossController != null)
        {
            bool isTransition = bossController.StateMachine.CurrentState == bossController.PhaseTransitionState;
            if (isTransition)
                return 0;
            
            bool isIdle = (bossController.StateMachine.CurrentState == bossController.IdleState ||
                           bossController.StateMachine.CurrentState == bossController.Ph2IdleState);
            if (isIdle)
                bossController.Animator.SetTrigger(bossController.AnimKeyHurt);
            else
                return 0;
        }

        int actualDamage = base.TakeDamage(damage);

        if (Stat.CurrentHp <= 0)
        {
            if (bossController.CurrentPhase != BossPhase.Dead)
            {
                bossController.ChangePhase(BossPhase.Dead);
                bossController.StateMachine.ChangeState(bossController.DeadState);
            }
            return actualDamage;
        }

        if (bossController != null && bossController.CurrentPhase == BossPhase.Phase1
                                   && phase2Triggered == false)
        {
            float hpRatio = (float)Stat.CurrentHp / Stat.MaxHp;

            if (hpRatio <= phase2Threshold)
            {
                phase2Triggered = true;
                bossController.StateMachine.ChangeState(bossController.PhaseTransitionState);
            }
        }

        return actualDamage;
    }
}
