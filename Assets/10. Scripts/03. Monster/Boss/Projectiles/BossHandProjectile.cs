using System.Collections;
using UnityEngine;

public class BossHandProjectile : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D hitBox;

    [Header("Movement")] [SerializeField] private float moveSpeed = 40f;

    private Boss boss;
    private bool isDropping = false;
    
    private Vector3 startPos;
    private Vector3 targetPos;

    public void Init(Boss boss)
    {
        this.boss = boss;
        animator.Play("Attack");
    }

    private void Update()
    {
        if (isDropping)
        {
            transform.Translate(Vector3.down * (moveSpeed * Time.deltaTime));
        }
    }

    public void OnAttackFinished()
    {
        Destroy(gameObject);
    }

    public void OnAnimEvent_DropStart() => isDropping = true;
    public void OnAnimEvent_DropEnd() => isDropping = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false)
            return;

        CombatEvent ev = new CombatEvent
        {
            Type = EventType.DamageEvent,
            Amount = boss.Stat.Attack,
            Position = other.transform.position
        };
        
        CombatSystem.Instance.ToPlayer(ev);
    }
}