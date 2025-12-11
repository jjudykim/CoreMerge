using UnityEngine;

public enum ESkillType
{
    Active,   
    Passive,
}

[System.Serializable]
public class SkillData
{
    [Header("Identity")]
    public int id;
    public string skillName;
    [TextArea]
    public string description;
    
    [Header("Type")]
    public ESkillType skillType;

    [Header("Combat")] 
    public float baseCooldown;
    public float power;
}
