using System;
using UnityEngine;

public class PlayerBodyHitDetector : MonoBehaviour
{
    public PlayerController owner;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster") || other.CompareTag("MonsterProjectile"))
        {
            Debug.Log("PlayerBodyHitDetector Call!!!");
            owner.OnBodyHit(other);
        }
    }
}
