using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class PlayerStat : BaseStat
{
    [SerializeField] private float critChance;
    [SerializeField] private float critDamageMultiplier;
    [SerializeField] private float skillCooldownReduction;

    public float CritChance
    {
        get => critChance;
        set => critChance = Mathf.Clamp01(value);
    }

    public float CritDamageMultiplier
    {
        get => critDamageMultiplier;
        set => critDamageMultiplier = Mathf.Max(1f, value);
    }

    public float SkillCooldownReduction
    {
        get => skillCooldownReduction;
        set => skillCooldownReduction = Mathf.Clamp(value, 0f, 0.9f);
    }

    public float GetFinalCooldown(float baseCooldown)
    {
        return baseCooldown * (1f - skillCooldownReduction);
    }

    public float CalculateDamage(float baseDamage, out bool isCritical)
    {
        isCritical = Random.value < CritChance;

        if (isCritical)
            return baseDamage * CritDamageMultiplier;

        return baseDamage;
    }
} 