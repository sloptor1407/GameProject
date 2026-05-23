using UnityEngine;

public static class GameSession
{
    public static int CodJugador { get; set; } = -1;
    public static string NombreJugador { get; set; } = "";
    public static int MuertesTotales { get; set; } = 0;
    public static int MaxNivelDesbloqueado { get; set; } = 1;
    public static Vector2 RespawnPoint { get; set; }
    public static bool HasRespawnPoint { get; set; }

    public static bool HasActiveSession => CodJugador != -1;

    public static void Reset()
    {
        CodJugador = -1;
        NombreJugador = "";
        MuertesTotales = 0;
        MaxNivelDesbloqueado = 1;
    }
}