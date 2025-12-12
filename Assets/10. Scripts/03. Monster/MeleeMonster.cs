public class MeleeMonster : Monster
{
    protected override void Awake()
    {
        base.Awake();
        
        stat.CurrentHp = stat.MaxHp;
    }       
}