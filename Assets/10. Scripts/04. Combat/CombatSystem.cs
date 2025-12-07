using Jay;
using UnityEngine;

public class CombatSystem : SingletonBase<CombatSystem>
{
    public static CombatSystem Instance => instance;

    public FloatingText damagePopupPrefab;
    
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
    }

    public void ToMonster(Collider2D targetCollider, CombatEvent combatEvent)
    {
        Monster monster = Monster.GetMonsterOrNull(targetCollider);

        if (monster == null)
            return;
        
        switch(combatEvent.Type)
        {
            case EventType.DamageEvent:
            {
                if (monster.Type != MonsterType.BOSS)
                {
                    FloatingText popUp = Instantiate(damagePopupPrefab);
                    popUp.Show($"{combatEvent.Amount}", Color.orange, monster.HeadUpPivot.position);
                }
                monster.TakeDamage(combatEvent.Amount);
                break;
            }
            default:
                break;
        }
    }
    
    public void ToPlayer(CombatEvent combatEvent)
    {
        Player player = Player.LocalPlayer;

        switch (combatEvent.Type)
        {
            case EventType.DamageEvent:
            {
                player.TakeDamage(combatEvent.Amount);
                break;
            }
            case EventType.HealEvent:
            {
                player.TakeHeal(combatEvent.Amount);
                break;
            }
            default:
                break;
        }
    }
}
