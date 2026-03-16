using UnityEngine;

public class RangeMonster : Monster
{
    [Header("Range Attack")]
    [SerializeField] private MonsterProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    
    protected override void Awake()
    {
        base.Awake();
        stat.CurrentHp = stat.MaxHp;
    }
    
    public override void PerformAttack(Vector2 targetPos)
    {
        FireProjectile(targetPos);
    }
    
    public void FireProjectile(Vector2 targetPos)
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        Vector2 dir = (targetPos - (Vector2)firePoint.position).normalized;
        
        var proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.Init(dir, this);
    }
}