using System.Collections.Generic;
using UnityEngine;

public partial class Monster : MonoBehaviour
{
    private static Dictionary<Collider2D, Monster> MonsterDic { get; set; } = new();

    private static void AddMonster(Monster monster)
    {
        if (MonsterDic.ContainsKey(monster.MainCollider))
        {
            // 중복 개체에 대한 처리
            return;
        }
        
        MonsterDic.Add(monster.MainCollider, monster);
    }

    private static void RemoveMonster(Monster monster)
    {
        if (MonsterDic.ContainsKey(monster.MainCollider) == false)
            return;
        MonsterDic.Remove(monster.MainCollider);
    }

    public static Monster GetMonsterOrNull(Collider2D collider)
    {
        Monster reval = null;
        if (MonsterDic.ContainsKey(collider)) 
            reval = MonsterDic[collider];
        return reval;
    }
}
