using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public static Player LocalPlayer;

    private PlayerController PlayerController { get; set; }

    private PlayerRuntimeData runtimeData;
    public PlayerStat PlayerStat => runtimeData.Stat;
    public int CurrentHp => runtimeData.CurrentLife;

    private const int DEFAULT_BASE_MAX_HP = 3;
    private int baseMaxHp;

    private QuickSlotData QuickSlots => Managers.Instance.QuickSlots;
    private CoreDBManager CoreDB => Managers.Instance.CoreDB;

    private CoreStatModifier coreBonus;

    public int FinalMaxHp => PlayerStat.MaxHp + coreBonus.maxHpBonus;
    public int FinalAttack => PlayerStat.Attack + coreBonus.attackBonus;
    public int FinalDefense => PlayerStat.Defense + coreBonus.defenseBonus;
    public float FinalCritChance => Mathf.Clamp01(PlayerStat.CritChance + coreBonus.critChanceBonus);
    public float FinalCritDamageMultiplier => Mathf.Max(1f, PlayerStat.CritDamageMultiplier + coreBonus.critDamageBonus);
    public float FinalSkillCooldownReduction => Mathf.Clamp(PlayerStat.SkillCooldownReduction + coreBonus.skillCooldownRate, 0f, 0.9f);

    public event Action<int, int> OnLifeChanged;

    private void Awake()
    {
        if (LocalPlayer == null)
        {
            LocalPlayer = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        PlayerController = GetComponent<PlayerController>();
        coreBonus = new CoreStatModifier();

        runtimeData = Managers.Instance.PlayerData;

        baseMaxHp = DEFAULT_BASE_MAX_HP;
        
        RecalculateCoreBonus();
        
        if (runtimeData.CurrentLife <= 0)
            runtimeData.CurrentLife = PlayerStat.MaxHp;

        NotifyLifeChanged();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnEnable()
    {
        if (QuickSlots != null)
            QuickSlots.OnChanged += RecalculateCoreBonus;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        if (QuickSlots != null)
            QuickSlots.OnChanged -= RecalculateCoreBonus;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RestoreFullHealth();
        InitPosition();
    }

    private void RestoreFullHealth()
    {
        if (runtimeData == null || PlayerStat == null)
            return;

        runtimeData.CurrentLife = FinalMaxHp;
        
        NotifyLifeChanged();
    }

    private void InitPosition()
    {
        var spawnPoint = FindFirstObjectByType<PlayerSpawnPoint>();

        if (spawnPoint != null)
        {
            var rb = PlayerController.Rigidbody;
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.position = spawnPoint.transform.position;
            }
            
            transform.position = spawnPoint.transform.position;
        }
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
            coreBonus.Add(new CoreStatModifier
            {
                maxHpBonus = m.maxHpBonus,
                attackBonus = m.attackBonus,
                defenseBonus = m.defenseBonus,
                critChanceBonus = m.critChanceBonus,
                critDamageBonus = m.critDamageBonus,
                skillCooldownRate = m.skillCooldownRate
            });
        }
        
        NotifyLifeChanged();
    }

    private void NotifyLifeChanged()
    {
        if (OnLifeChanged != null) 
            OnLifeChanged.Invoke(runtimeData.CurrentLife, FinalMaxHp);
    }

    public void DamageToEnemy(Collider2D targetCollider, string attackInfoKey)
    {
        bool isCritical;
        int finalDamage = GetCalculatedDamage(attackInfoKey, out isCritical);

        CombatEvent sendEvent = new()
        {
            Type = EventType.DamageEvent,
            Amount = finalDamage,
            Position = targetCollider.transform.position,
            IsCritical = isCritical
        };

        CombatSystem.Instance.ToMonster(targetCollider, sendEvent);
    }

    private int GetCalculatedDamage(string key, out bool isCritical)
    {
        float baseDamage = FinalAttack;
        
        float damageAfterCrit = CalculateDamageWithCore(baseDamage, out isCritical);

        float variance = Random.Range(0.9f, 1.1f);
        float finalDamage = damageAfterCrit * variance;

        return Mathf.Max(1, Mathf.RoundToInt(finalDamage));
    }

    private int GetRandomDamage(string key)
    {
        float baseDamage = FinalAttack;
        
        bool isCritical;
        float finalDamage = CalculateDamageWithCore(baseDamage, out isCritical);
        
        float variance = UnityEngine.Random.Range(0.9f, 1.1f);
        finalDamage *= variance;
        
        int iFinalDamage = Mathf.Max(1, Mathf.RoundToInt(finalDamage));

        //Debug.Log($"[Player] Damage Calc ::: base = {baseDamage}, crit = {isCritical}, " +
        //             $"variance={variance:F2}, final={iFinalDamage}");

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

        runtimeData.CurrentLife -= 1;
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
