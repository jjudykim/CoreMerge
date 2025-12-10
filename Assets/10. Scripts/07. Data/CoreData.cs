using UnityEngine;

public enum ECoreTier : int
{
    Tier1 = 1,
    Tier2 = 2,
    Tier3 = 3,
    Tier4 = 4,
    Tier5 = 5,
    Tier6 = 6,
    Tier7 = 7,
    Tier8 = 8,
}

[System.Serializable]
public class CoreStatModifier
{
    [Header("Base Stat Modifier")]
    public int maxHpBonus;
    public int attackBonus;
    public int defenseBonus;
    public float critChanceBonus;
    public float critDamageBonus;
    public float skillCooldownRate;
    
    [Header("Skill Unlock")]
    public bool canUseSkill;
    public int skillId;
}


[System.Serializable]
public class CoreData
{
    [Header("Identity")]
    public int id;

    public string coreName;
    public ECoreTier tier;

    [Header("Visual")] 
    public Sprite icon;
    
    [Header("Effect")]
    public CoreStatModifier modifier;
}

