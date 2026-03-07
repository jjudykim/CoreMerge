using System.Collections;
using UnityEngine;

public class BossDeadState : BossStateBase
{
    private Coroutine deadCo;
    public BossDeadState(BossController boss, BossStateMachine stateMachine) : base(boss, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (bosscontroller.Collider != null)
            bosscontroller.Collider.enabled = false;
        
        FollowCamera followCam = Camera.main.GetComponent<FollowCamera>();
        if (followCam != null)
        {
            followCam.FocusOnTarget(bosscontroller.transform, 2f, 5f);
        }

        bosscontroller.Animator.SetTrigger(bosscontroller.AnimKeyDead);
        deadCo = bosscontroller.StartCoroutine(DeadSequence());
    }

    public override void Exit()
    {
        base.Exit();
        if (deadCo != null)
        {
            bosscontroller.StopCoroutine(deadCo);
            deadCo = null;
        }
    }

    private IEnumerator DeadSequence()
    {
        yield return new WaitForSeconds(2.0f);

        Managers.Instance.Game.OnBossDead();
        Object.Destroy(bosscontroller.gameObject);
    }
}