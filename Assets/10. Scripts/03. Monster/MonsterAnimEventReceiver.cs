using UnityEngine;

public class MonsterAnimEventReceiver: MonoBehaviour
{
    [SerializeField] private MonsterController monsterController;
    [SerializeField] private MonsterHitBox hitBox;

    public void Anim_EnableHitBox()
    {
        if (monsterController.CurrentState != MonsterState.Attack)
            return;
        
        hitBox.EnableHitBox();
    }

    public void Anim_DisableHitBox()
    {
        hitBox.DisableHitBox();
    }
}