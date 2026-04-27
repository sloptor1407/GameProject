using SQLite;

[Table("Jugador")]
public class DB_Jugador
{
    [PrimaryKey, AutoIncrement]
    public int codJugador { get; set; }
    public string nombreJugador { get; set; }
    public int vidaMax { get; set; }
    public float tiempoTotal { get; set; }
}

[Table("Nivel")]
public class DB_Nivel
{
    [PrimaryKey]
    public int codNivel { get; set; }
    public string nombreNivel { get; set; }
    public int ordenNivel { get; set; }
    public string descripcion { get; set; }
}

[Table("Arma")]
public class DB_Arma
{
    [PrimaryKey]
    public int codArma { get; set; }
    public string nombre { get; set; }
    public string tipo { get; set; }
    public int daño { get; set; }
    public float cooldown { get; set; }
}

[Table("Equipa")]
public class DB_Equipa
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int codJugador { get; set; }
    public int codArma { get; set; }
}

[Table("Juega")]
public class DB_Juega
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int codJugador { get; set; }
    public int codNivel { get; set; }
    public float tiempoCompletado { get; set; }
    public int muertes { get; set; }
}

[Table("Objeto")]
public class DB_Objeto
{
    [PrimaryKey]
    public int codObjeto { get; set; }
    public string nombreObjeto { get; set; }
    public string tipoObjeto { get; set; }
}

[Table("Tiene")]
public class DB_Tiene
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int codJugador { get; set; }
    public int codObjeto { get; set; }
    public bool obtenido { get; set; }
}

[Table("Contiene")]
public class DB_Contiene
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int codNivel { get; set; }
    public int codObjeto { get; set; }
    public int cantidad { get; set; }
}

[Table("TipoEnemigo")]
public class DB_TipoEnemigo
{
    [PrimaryKey]
    public int codTipoEnemigo { get; set; }
    public string nombreTipo { get; set; }
    public string descripcion { get; set; }
    public int dañoBase { get; set; }
    public int vidaBase { get; set; }
}

[Table("Enemigo")]
public class DB_Enemigo
{
    [PrimaryKey, AutoIncrement]
    public int codEnemigo { get; set; }
    public float posX { get; set; }
    public float posY { get; set; }
    public int codNivel { get; set; }
    public int codTipoEnemigo { get; set; }
}

[Table("Checkpoint")]
public class DB_Checkpoint
{
    [PrimaryKey]
    public int codCheckpoint { get; set; }
    public float posX { get; set; }
    public float posY { get; set; }
    public int codNivel { get; set; }
    public bool activado { get; set; }
}

[Table("Plataforma")]
public class DB_Plataforma
{
    [PrimaryKey, AutoIncrement]
    public int codPlataforma { get; set; }
    public string tipo { get; set; }
    public float posX { get; set; }
    public float posY { get; set; }
    public int codNivel { get; set; }
}