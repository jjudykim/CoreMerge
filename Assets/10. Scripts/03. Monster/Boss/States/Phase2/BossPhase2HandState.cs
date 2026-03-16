using System.Collections;
using UnityEngine;

public class BossPhase2HandState : BossStateBase
{
    private Coroutine patternCo;
    
    public BossPhase2HandState(BossController boss, BossStateMachine stateMachine) : base(boss, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        animator.SetTrigger(bosscontroller.AnimKeyPh2Attack);
        patternCo = bosscontroller.StartCoroutine(HandPatternSequence());
    }

    public override void Exit()
    {
        animator.ResetTrigger(bosscontroller.AnimKeyPh2Attack);
        if (patternCo != null)
        {
            bosscontroller.StopCoroutine(patternCo);
            patternCo = null;
        }
    }

    private IEnumerator HandPatternSequence()
    {
        int count = Random.Range(bosscontroller.MinHandCount, bosscontroller.MaxHandCount + 1);

        for (int i = 0; i < count; ++i)
        {
            SpawnHand();

            yield return new WaitForSeconds(bosscontroller.SpawnInterval);
        }

        yield return new WaitForSeconds(1.0f);
        stateMachine.ChangeState(bosscontroller.Ph2IdleState);
    }

    private void SpawnHand()
    {
        float randomX = Random.Range(bosscontroller.SpawnMinX, bosscontroller.SpawnMaxX);
        Vector3 spawnPos = new Vector3(randomX, bosscontroller.SpawnY, 0f);
        
        BossHandProjectile proj =
            GameObject.Instantiate(bosscontroller.HandPrefab, spawnPos, Quaternion.identity);
        proj.Init(bosscontroller.Boss);
    }
}