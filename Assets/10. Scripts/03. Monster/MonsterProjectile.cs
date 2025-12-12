using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 3f;

    private Vector2 direction;
    private float timer;

    public void Init(Vector2 dir, Monster owner)
    {
        direction = dir.normalized;

        var hitBox = GetComponent<MonsterHitBox>();
        if (hitBox != null && owner != null)
            hitBox.SetOwner(owner);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * (speed * Time.deltaTime));
        
        timer += Time.deltaTime;
        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}
