using UnityEngine;
using TMPro;

public class ProfileButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI timeText;

    DB_Jugador jugador;
    ProfileManager profileManager;

    public void Setup(DB_Jugador jugador, ProfileManager manager)
    {
        this.jugador = jugador;
        this.profileManager = manager;

        nameText.text = jugador.nombreJugador;
        timeText.text = FormatTime(jugador.tiempoTotal);
    }

    public void OnSelectButton()
    {
        profileManager.SelectProfile(jugador);
    }

    public void OnDeleteButton()
    {
        profileManager.DeleteProfile(jugador.codJugador);
    }

    string FormatTime(float seconds)
    {
        int h = (int)(seconds / 3600);
        int m = (int)(seconds % 3600 / 60);
        int s = (int)(seconds % 60);
        return $"{h:00}:{m:00}:{s:00}";
    }
}