using UnityEngine;

public class BossHitBox : MonoBehaviour
{
    [Header("Owner")]
    [SerializeField] private Boss owner;
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private bool useOwnerStat = true;

    private void Awake()
    {
        if (owner == null)
            owner = GetComponentInParent<Boss>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false)
            return;

        int finalDamage = owner.Stat.Attack;

        CombatEvent ev = new CombatEvent
        {
            Type = EventType.DamageEvent,
            Amount = finalDamage,
            Position = other.transform.position
        };
        
        CombatSystem.Instance.ToPlayer(ev);
        
        Debug.Log($"[BossHitBox] Hit Player :: damage={finalDamage}");
    }
}