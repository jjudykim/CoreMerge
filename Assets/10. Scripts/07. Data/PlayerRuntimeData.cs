using System;

[Serializable]
public class PlayerRuntimeData
{
    public PlayerStat Stat;

    public int CurrentLife;

    public PlayerRuntimeData()
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