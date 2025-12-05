using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : MonoBehaviour
{
    // Enemy_Static.cs 에는 Enemy 개체들을
    // 관리할 수 있는 함수와 필드들이 작성됩니다.
    // static 필드와 static 함수를 모아놓는 용도로 partial을 사용한겁니다.

    private static Dictionary<Collider2D, Enemy> EnemyDic { get; set; } = new();

    private static void AddEnemy(Enemy enemy)
    {
        if (EnemyDic.ContainsKey(enemy.MainCollider))
        {
            //중복 개체에 대한 처리를 기입하는게 좋음.
            //ex) 삭제시킨다거나, 비활성화 해놓는다거나 등등
            return;
        }
        
        EnemyDic.Add(enemy.MainCollider, enemy);
    }
    
    private static void RemoveEnemy(Enemy enemy)
    {
        if (EnemyDic.ContainsKey(enemy.MainCollider) == false) return;
        EnemyDic.Remove(enemy.MainCollider);
    }

    public static Enemy GetEnemyOrNull(Collider2D collider)
    {
        Enemy reval = null; //reVal = returnValue의 약자 입니다.
        if(EnemyDic.ContainsKey(collider)) reval = EnemyDic[collider];        
        return reval;
    }
}