public static class GameSession
{
    public static int CodJugador { get; set; } = -1;
    public static string NombreJugador { get; set; } = "";
    public static int MuertesTotales { get; set; } = 0;
    public static int MaxNivelDesbloqueado { get; set; } = 1;

    public static bool HasActiveSession => CodJugador != -1;

    public static void Reset()
    {
        CodJugador = -1;
        NombreJugador = "";
        MuertesTotales = 0;
        MaxNivelDesbloqueado = 1;
    }
}