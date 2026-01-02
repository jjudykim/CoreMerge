using UnityEngine;

public class BossAnimEventReceiver : MonoBehaviour
{
    [Header("Owner")]
    [SerializeField] private BossController boss; 

    private void Reset()
    {
        boss = GetComponentInParent<BossController>();
    }

    private void Awake()
    {
        if (boss == null)
            boss = GetComponentInParent<BossController>();

        if (boss == null)
            Debug.LogError("[BossAnimEventReceiver] BossController not found in parents.");
    }
    
    
    public void AE_Ph1Range_FireStart()
    {
        if (boss == null) return;
        boss.OnAnimEvent_Ph1Range_FireStart();
    }

    public void AE_Ph1Range_FireEnd()
    {
        if (boss == null) return;
        boss.OnAnimEvent_Ph1Range_FireEnd();
    }
}