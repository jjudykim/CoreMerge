using System;
using UnityEngine;

public enum HitBoxType
{
    DashAttack,
    SlideAttack,
    NormalAttack,
}

public class PlayerHitBox : MonoBehaviour
{
    public HitBoxType HitBoxType;
    public PlayerController Owner { get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            Owner.OnHitBoxTriggered(this, other);
        }
    }
}
