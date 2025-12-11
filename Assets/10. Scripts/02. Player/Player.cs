using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public static Player LocalPlayer;

    private PlayerController PlayerController { get; set; }

    [Header("Stat")] 
    [SerializeField] private PlayerStat playerStat;
    public PlayerStat PlayerStat => playerStat;

    private QuickSlotData QuickSlots => Managers.Instance.QuickSlots;
    private CoreDBManager CoreDB => Managers.Instance.CoreDB;

    private CoreStatModifier coreBonus;

    public int FinalMaxHp { get => playerStat.MaxHp + coreBonus.maxHpBonus; }
    public int FinalAttack { get => playerStat.Attack + coreBonus.attackBonus; }
    public int FinalDefense { get => playerStat.Defense + coreBonus.defenseBonus; }
    public float FinalCritChance { get => Mathf.Clamp01(playerStat.CritChance + coreBonus.critChanceBonus); }
    public float FinalCritDamageMultiplier { get => Mathf.Max(1f, playerStat.CritDamageMultiplier + coreBonus.critDamageBonus); }
    public float FinalSkillCooldownReduction { get => Mathf.Clamp(playerStat.SkillCooldownReduction + coreBonus.skillCooldownRate, 0f, 0.9f); }


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
        coreBonus = new CoreStatModifier();

        if (playerStat == null)
        {
            playerStat = new PlayerStat
            {
                MaxHp = 3,
                Attack = 10,
                Defense = 0,
                CritChance = 0,
                CritDamageMultiplier = 0,
                SkillCooldownReduction = 0,
            };

            playerStat.CurrentHp = playerStat.MaxHp;
        }
        // AttackInfo 데이터 삽입
        RecalculateCoreBonus();
    }

    private void OnEnable()
    {
        if (QuickSlots != null)
            QuickSlots.OnChanged += RecalculateCoreBonus;
    }

    private void RecalculateCoreBonus()
    {
        coreBonus.Clear();

        if (QuickSlots == null || CoreDB == null)
            return;

        var slots = QuickSlots.GetAllSlots();

        foreach (var slotData in slots)
        {
            int itemId = slotData.ItemId;
            if (itemId == 0)
                continue;

            CoreData core = CoreDB.GetCoreDataOrNull(itemId);
            if (core == null)
                continue;

            var m = core.modifier;

            CoreStatModifier bonus = new CoreStatModifier
            {
                maxHpBonus = m.maxHpBonus,
                attackBonus = m.attackBonus,
                defenseBonus = m.defenseBonus,
                critChanceBonus = m.critChanceBonus,
                critDamageBonus = m.critDamageBonus,
                skillCooldownRate = m.skillCooldownRate
            };
            
            coreBonus.Add(bonus);
        }
        
        playerStat.CurrentHp = FinalMaxHp;
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
        float baseDamage = FinalAttack;
        
        bool isCritical;
        float finalDamage = CalculateDamageWithCore(baseDamage, out isCritical);
        
        float variance = UnityEngine.Random.Range(0.9f, 1.1f);
        finalDamage *= variance;
        
        int iFinalDamage = Mathf.Max(1, Mathf.RoundToInt(finalDamage));

        Debug.Log($"[Player] Damage Calc ::: base = {baseDamage}, crit = {isCritical}, " +
                     $"variance={variance:F2}, final={iFinalDamage}");

        return iFinalDamage;
    }

    private float CalculateDamageWithCore(float baseDamage, out bool isCritical)
    {
        float critChance = FinalCritChance;
        float critMul = FinalCritDamageMultiplier;

        isCritical = Random.value < critChance;
        if (isCritical)
            return baseDamage * critMul;

        return baseDamage;
    }

    public void TakeDamage(int damage)
    {
        if (PlayerController.CanReceiveDamage() == false)
            return;

        PlayerController.ApplyDamageGating();
        
        int reduced = (int)Mathf.Max(1f, damage - FinalDefense);
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

        if (playerStat.CurrentHp > FinalMaxHp)
            playerStat.CurrentHp = FinalMaxHp;
        
        Debug.Log($"[Player] TakeHeal :: amount={heal}, hp={before} -> {playerStat.CurrentHp}/{playerStat.MaxHp}");
    }
}
