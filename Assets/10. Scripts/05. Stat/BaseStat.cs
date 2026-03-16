using System;
using UnityEngine;

[Serializable]
public class BaseStat
{
    [SerializeField] private int maxHp;
    [SerializeField] private int attack;
    [SerializeField] private int defense;

    [SerializeField] private int currentHp;

    public int MaxHp
    {
        get => maxHp;
        set
        {
            maxHp = (int)Mathf.Max(1f, value);
            currentHp = Mathf.Min(currentHp, maxHp);
        }
    }

    public int Attack
    {
        get => attack;
        set => attack = (int)Mathf.Max(0f, value);
    }

    public int Defense
    {
        get => defense;
        set => defense = (int)Mathf.Max(0f, value);
    }

    public int CurrentHp
    {
        get => currentHp;
        set => currentHp = (int)Mathf.Clamp(value, 0f, MaxHp);
    }

    private void Awake()
    {
        currentHp = MaxHp;
    }

    public bool IsDead() => CurrentHp <= 0;
}
