using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    [SerializeField] int damage = 3;

    void Start()
    {
        // Escala el slash para que ocupe toda la pantalla horizontalmente
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect * 2;
        transform.localScale = new Vector3(screenWidth, 1.5f, 1f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null && pc.IsGrounded)
                other.GetComponent<PlayerStats>()?.ReceiveDamage(damage);
        }
    }
}