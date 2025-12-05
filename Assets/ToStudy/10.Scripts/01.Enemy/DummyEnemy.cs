using UnityEngine;

public class DummyEnemy : Enemy
{
    protected override void Awake()
    {
        //base는 상속 대상으 함수를 특정할때 사용합니다.
        //주로 부모객체라고 생각하시면 됨.
        base.Awake();
        
    }
}

