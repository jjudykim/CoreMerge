using UnityEngine;

public enum EventType
{
    DamageEvent,
    HealEvent,
}

public struct CombatEvent
{
    public EventType Type { get; set; }
    public int Amount { get; set; }
    public Vector3 Position { get; set; }
    public bool IsCritical { get; set; }
}