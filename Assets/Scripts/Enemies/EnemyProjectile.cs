using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    float speed = 8f;
    int damage;
    Vector2 direction;

    public void Init(Vector2 dir, int dmg)
    {
        direction = dir.normalized;
        damage = dmg;
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>()?.ReceiveDamage(damage);
            Destroy(gameObject);
        }
        else if (((1 << other.gameObject.layer) & LayerMask.GetMask("Ground")) != 0)
        {
            Destroy(gameObject);
        }
    }
}