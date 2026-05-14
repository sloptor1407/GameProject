using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] int value = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        AudioManager.Instance?.PlayCoin();
        if (!other.CompareTag("Player")) return;

        HUDManager.Instance?.UpdateCoins(value);

        // Guardar en BD
        if (GameSession.HasActiveSession)
        {
            DatabaseManager.Instance?.GuardarObjeto(
                GameSession.CodJugador,
                1, // codObjeto moneda
                true
            );
        }

        Destroy(gameObject);
    }
}