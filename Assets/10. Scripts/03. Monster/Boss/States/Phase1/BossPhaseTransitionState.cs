using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseTransitionState : BossStateBase
{
    private const float HideDuration = 2.0f;
    private const float AlphaFadeDuration = 1f;

    private Coroutine transitionCo;

    private bool isHideEventTriggered = false;
    private bool isSpawnEventTriggered = false;
    
    public BossPhaseTransitionState(BossController boss, BossStateMachine stateMachine) : base(boss, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        isHideEventTriggered = false;
        isSpawnEventTriggered = false;

        if (bosscontroller.Collider != null)
            bosscontroller.Collider.enabled = false;

        if (transitionCo != null)
        {
            bosscontroller.StopCoroutine(transitionCo);
            transitionCo = null;
        }
        
        animator.SetTrigger(bosscontroller.AnimKeyPhaseChange1);

        transitionCo = bosscontroller.StartCoroutine(PhaseTransitionSequence());
    }

    public override void Exit()
    {
        base.Exit();

        if (transitionCo != null)
        {
            bosscontroller.StopCoroutine(transitionCo);
            transitionCo = null;
        }

        if (bosscontroller.Collider != null)
            bosscontroller.Collider.enabled = true;
        
        animator.ResetTrigger(bosscontroller.AnimKeyPhaseChange1);
        animator.ResetTrigger(bosscontroller.AnimKeyPhaseChange2);

        SetAlpha(1f);
    }

    public void OnAnimEvent_HideStart() => isHideEventTriggered = true;
    public void OnAnimEvent_HideEnd() => isSpawnEventTriggered = true;
    
    private IEnumerator PhaseTransitionSequence()
    {
        while (isHideEventTriggered == false)
            yield return null;

        yield return bosscontroller.StartCoroutine(CoFadeAlpha(0f, AlphaFadeDuration));
        yield return new WaitForSeconds(HideDuration);
        
        bosscontroller.ChangePhase(BossPhase.Phase2);
        bosscontroller.transform.position = bosscontroller.OriginPosition;
        animator.SetTrigger(bosscontroller.AnimKeyPhaseChange2);

        yield return bosscontroller.StartCoroutine(CoFadeAlpha(1f, AlphaFadeDuration));

        while (isSpawnEventTriggered == false)
            yield return null;
        
        stateMachine.ChangeState(bosscontroller.Ph2IdleState);
    }

    private IEnumerator CoFadeAlpha(float targetAlpha, float duration)
    {
        if (bosscontroller.SpriteRenderer == null)
            yield break;
        
        float startAlpha = bosscontroller.SpriteRenderer.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            SetAlpha(currentAlpha);
            yield return null;
        }
        
        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float alpha)
    {
        if (bosscontroller.SpriteRenderer == null)
            return;
        
        Color c = bosscontroller.SpriteRenderer.color;
        c.a = alpha;
        bosscontroller.SpriteRenderer.color = c;
    }
}