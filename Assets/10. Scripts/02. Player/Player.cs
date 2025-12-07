using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player LocalPlayer;

    private PlayerController PlayerController { get; set; }

    [Header("Stat")] 
    [SerializeField] private PlayerStat playerStat;
    
    // private Dictionary<string, AttackInfo> AttackInfoDic { get; set; } = new();
    
    private void Awake()
    {
        if (LocalPlayer == null)
            LocalPlayer = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        PlayerController = GetComponent<PlayerController>();

        if (playerStat == null)
        {
            playerStat = new PlayerStat
            {
                MaxHp = 100,
                Attack = 10,
                Defense = 0
            };

            playerStat.CurrentHp = playerStat.MaxHp;
        }
        // AttackInfo 데이터 삽입
    }

    public void DamageToEnemy(Collider2D targetCollider, string attackInfoKey)
    {
        CombatEvent sendEvent = new()
        {
            Type = EventType.DamageEvent,
            Amount = GetRandomDamage(attackInfoKey),
            Position = targetCollider.transform.position,
        };

        CombatSystem.Instance.ToMonster(targetCollider, sendEvent);
    }

    private int GetRandomDamage(string key)
    {
        float baseDamage = playerStat.Attack;
        
        bool isCritical;
        float finalDamage = playerStat.CalculateDamage(baseDamage, out isCritical);
        
        float variance = UnityEngine.Random.Range(0.9f, 1.1f);
        finalDamage *= variance;
        
        int iFinalDamage = Mathf.Max(1, Mathf.RoundToInt(finalDamage));

        Debug.Log($"[Player] Damage Calc ::: base = {baseDamage}, crit = {isCritical}, " +
                     $"variance={variance:F2}, final={iFinalDamage}");

        return iFinalDamage;
    }

    public void TakeDamage(int damage)
    {
        if (PlayerController.CanReceiveDamage() == false)
            return;

        PlayerController.ApplyDamageGating();
        
        int reduced = (int)Mathf.Max(1f, damage - playerStat.Defense);
        playerStat.CurrentHp -= reduced;
        
        Debug.Log($"[Player] TakeDamage :: income={damage}, def={playerStat.Defense}, " +
                  $"final={reduced}, hp={playerStat.CurrentHp}/{playerStat.MaxHp}");

        if (playerStat.IsDead())
            PlayerController.Die();
        else
            PlayerController.Hurt();
    }

    public void TakeHeal(int heal)
    {
        float before = playerStat.CurrentHp;
        playerStat.CurrentHp += heal;
        
        Debug.Log($"[Player] TakeHeal :: amount={heal}, hp={before} -> {playerStat.CurrentHp}/{playerStat.MaxHp}");
    }
}
