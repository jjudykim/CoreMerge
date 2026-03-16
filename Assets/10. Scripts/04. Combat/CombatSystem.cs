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
                //if (monster.Type != MonsterType.BOSS)
                //{
                //    
                //}
                int actualDamage = monster.TakeDamage(combatEvent.Amount);
                Debug.Log($"[CombatSystem] {monster.gameObject.name}이(가) {actualDamage}의 데미지를 입었습니다.");

                if (actualDamage > 0 && damagePopupPrefab != null)
                {
                    var ftObj = Instantiate(damagePopupPrefab, monster.HeadUpPivot.position, Quaternion.identity);
                    var ft = ftObj.GetComponent<FloatingText>();

                    Color textColor = combatEvent.IsCritical ? Color.yellow : Color.white;
                    string message = actualDamage.ToString();

                    if (combatEvent.IsCritical)
                        message = "CRIT!\n" + message;
                    
                    ft.Show(message, textColor, combatEvent.Position);
                }
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
