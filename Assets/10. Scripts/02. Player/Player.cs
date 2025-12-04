using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player LocalPlayer;

    private PlayerController PlayerController { get; set; }
    // private Dictionary<string, AttackInfo> AttackInfoDic { get; set; } = new();

    private void Awake()
    {
        PlayerController = GetComponent<PlayerController>();
        
        // AttackInfo 데이터 삽입
    }

    public void DamageToEnemy(Collider2D targetCollider, string attackInfoKey)
    {
    }

    public void TakeDamage()
    {
    }

    public void TakeHeal()
    {
    }
}
