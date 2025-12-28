using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public static Player LocalPlayer;

    private PlayerController PlayerController { get; set; }

    private PlayerRuntimeData runtimeData;
    public PlayerStat PlayerStat => runtimeData.Stat;
    public int CurrentHp => PlayerStat.CurrentHp;
    
    private int baseMaxHp;

    private QuickSlotData QuickSlots => Managers.Instance.QuickSlots;
    private CoreDBManager CoreDB => Managers.Instance.CoreDB;

    private CoreStatModifier coreBonus;

    public int FinalMaxHp { get => PlayerStat.MaxHp + coreBonus.maxHpBonus; }
    public int FinalAttack { get => PlayerStat.Attack + coreBonus.attackBonus; }
    public int FinalDefense { get => PlayerStat.Defense + coreBonus.defenseBonus; }
    public float FinalCritChance { get => Mathf.Clamp01(PlayerStat.CritChance + coreBonus.critChanceBonus); }
    public float FinalCritDamageMultiplier { get => Mathf.Max(1f, PlayerStat.CritDamageMultiplier + coreBonus.critDamageBonus); }
    public float FinalSkillCooldownReduction { get => Mathf.Clamp(PlayerStat.SkillCooldownReduction + coreBonus.skillCooldownRate, 0f, 0.9f); }

    public event Action<int, int> OnLifeChanged;

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

        runtimeData = Managers.Instance.PlayerData;

        baseMaxHp = PlayerStat.MaxHp;
        
        RecalculateCoreBonus();

        NotifyLifeChanged();
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

        int newMaxLife = baseMaxHp + coreBonus.maxHpBonus;
        PlayerStat.MaxHp = newMaxLife;

        if (runtimeData.CurrentLife > PlayerStat.MaxHp)
            runtimeData.CurrentLife = PlayerStat.MaxHp;

        NotifyLifeChanged();
    }

    private void NotifyLifeChanged()
    {
        if (OnLifeChanged != null) 
            OnLifeChanged.Invoke(runtimeData.CurrentLife, PlayerStat.MaxHp);
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

        //runtimeData.CurrentLife -= 1;
        NotifyLifeChanged();

        if (runtimeData.CurrentLife <= 0)
            PlayerController.Die();
        else
            PlayerController.Hurt();
    }

    public void TakeHeal(int heal)
    {
        runtimeData.CurrentLife += heal;

        if (runtimeData.CurrentLife > FinalMaxHp)
            runtimeData.CurrentLife = FinalMaxHp;

        NotifyLifeChanged();
    }
}
