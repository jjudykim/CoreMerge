using UnityEngine;

public class Boss : Monster
{
    [Header("Boss Only")]
    [SerializeField] private BossController bossController;

    public BaseStat Stat => stat;
    
    protected override void Awake()
    {
        base.Awake();
        
        bossController = GetComponent<BossController>();
        contactDamageEnabled = false;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (Stat.IsDead())
            HandlePhaseOrDeath();
        else
        {
            // TODO : 나중에 공격 중이 아닌 상태에서 유효타를 맞았을 경우에 Hurt 상태 적용되도록 Logic 변경
            // bossController.Hurt();
        }
    }

    private void HandlePhaseOrDeath()
    {
        switch (bossController.CurrentPhase)
        {
            case BossPhase.Phase1:
                bossController.ChangePhase(BossPhase.Phase2);
                Stat.CurrentHp = Stat.MaxHp;
                break;
            case BossPhase.Phase2:
                bossController.ChangePhase(BossPhase.Dead);
                bossController.Die();
                break;
        }
    }
}
