using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed = 12f;
    int damage;
    Vector2 direction;

    public void Init(Vector2 dir, int dmg)
    {
        direction = dir;
        damage = dmg;
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyStats>()?.ReceiveDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}