using System;
using UnityEngine;

// partial 키워드란?
// 클래스를 부분적으로 나누어서 구현 가능하게끔 해주는 키워드.
public abstract partial class Enemy : MonoBehaviour
{
    // 추상클래스의 사용
    // 공통 특징, 기능 = (필드, 메서드)등을 정의해놓는 용도
    // 구체화할수 없으며, 상속받는 클래스에게 특정 메서드들의 구현을 강제화 함
    
    public Collider2D MainCollider { get; private set; }

    [field: SerializeField] 
    // [field: SerializeField] 키워드를 입력하면 프로퍼티여도 
    // 인스펙터에서 노출이 가능하다
    public Transform HeadUpPivot { get; private set; }

    protected virtual void Awake()
    {
        MainCollider = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        Enemy.AddEnemy(this);
    }

    private void OnDestroy()
    {
        Enemy.RemoveEnemy(this);
    }
    
    public void TakeDamage(int damage)
    {
        
    }
}