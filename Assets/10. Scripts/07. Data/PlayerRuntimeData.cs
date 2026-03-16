using System;

[Serializable]
public class PlayerRuntimeData
{
    public PlayerStat Stat;

    public int CurrentLife;

    public PlayerRuntimeData()
    {
        InitializeDefault();
    }

    public void ResetData()
    {
        InitializeDefault();
    }

    private void InitializeDefault()
    {
        Stat = new PlayerStat
        {
            MaxHp = 3,
            Attack = 10,
            Defense = 0,
            CritChance = 0,
            CritDamageMultiplier = 0,
            SkillCooldownReduction = 0,
        };

        CurrentLife = Stat.MaxHp;
    }
}