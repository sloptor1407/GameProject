using UnityEngine;
using SQLite;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    SQLiteConnection db;
    string dbPath;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeDatabase();
    }

    void InitializeDatabase()
    {
        dbPath = Path.Combine(Application.persistentDataPath, "gamedata.db");
        db = new SQLiteConnection(dbPath);
        CreateTables();
        Debug.Log($"Base de datos iniciada en: {dbPath}");
    }

    void CreateTables()
    {
        db.CreateTable<DB_Jugador>();
        db.CreateTable<DB_Nivel>();
        db.CreateTable<DB_Arma>();
        db.CreateTable<DB_Equipa>();
        db.CreateTable<DB_Juega>();
        db.CreateTable<DB_Objeto>();
        db.CreateTable<DB_Tiene>();
        db.CreateTable<DB_Contiene>();
        db.CreateTable<DB_TipoEnemigo>();
        db.CreateTable<DB_Enemigo>();
        db.CreateTable<DB_Checkpoint>();
        db.CreateTable<DB_Plataforma>();

        SeedData();
    }

    // Seed: datos iniciales

    void SeedData()
    {
        // Solo inserta si las tablas están vacías
        if (db.Table<DB_Nivel>().Count() == 0)
        {
            db.InsertAll(new[] {
                new DB_Nivel { codNivel = 1, nombreNivel = "Tutorial",    ordenNivel = 1, descripcion = "Nivel introductorio" },
                new DB_Nivel { codNivel = 2, nombreNivel = "Normal",      ordenNivel = 2, descripcion = "Nivel intermedio" },
                new DB_Nivel { codNivel = 3, nombreNivel = "Final",       ordenNivel = 3, descripcion = "Nivel final con jefe" },
            });
        }

        if (db.Table<DB_Arma>().Count() == 0)
        {
            db.InsertAll(new[] {
                new DB_Arma { codArma = 1, nombre = "Espada Básica", tipo = "MELEE", daño = 2, cooldown = 0.5f },
                new DB_Arma { codArma = 2, nombre = "Arco Básico",   tipo = "RANGE", daño = 1, cooldown = 0.8f },
            });
        }

        if (db.Table<DB_TipoEnemigo>().Count() == 0)
        {
            db.InsertAll(new[] {
                new DB_TipoEnemigo { codTipoEnemigo = 1, nombreTipo = "Patrullero", descripcion = "Va y viene", dañoBase = 1, vidaBase = 3 },
                new DB_TipoEnemigo { codTipoEnemigo = 2, nombreTipo = "Perseguidor", descripcion = "Persigue al jugador", dañoBase = 1, vidaBase = 4 },
                new DB_TipoEnemigo { codTipoEnemigo = 3, nombreTipo = "Disparador", descripcion = "Ataca a distancia", dañoBase = 1, vidaBase = 2 },
            });
        }

        if (db.Table<DB_Objeto>().Count() == 0)
        {
            db.InsertAll(new[] {
                new DB_Objeto { codObjeto = 1, nombreObjeto = "Moneda",          tipoObjeto = "COIN" },
                new DB_Objeto { codObjeto = 2, nombreObjeto = "Mejora de arma",  tipoObjeto = "WEAPON_UPGRADE" },
            });
        }
    }

    // Jugador

    public int CrearJugador(string nombre)
    {
        var jugador = new DB_Jugador
        {
            nombreJugador = nombre,
            vidaMax = 5,
            tiempoTotal = 0f
        };
        db.Insert(jugador);
        return jugador.codJugador;
    }

    public DB_Jugador GetJugador(int codJugador)
    {
        return db.Find<DB_Jugador>(codJugador);
    }

    public void ActualizarTiempoTotal(int codJugador, float tiempo)
    {
        var jugador = db.Find<DB_Jugador>(codJugador);
        if (jugador == null) return;
        jugador.tiempoTotal = tiempo;
        db.Update(jugador);
    }

    // Partida (Juega)

    public void GuardarPartida(int codJugador, int codNivel, float tiempo, int muertes)
    {
        var existing = db.Table<DB_Juega>()
            .Where(j => j.codJugador == codJugador && j.codNivel == codNivel)
            .FirstOrDefault();

        if (existing != null)
        {
            existing.tiempoCompletado = tiempo;
            existing.muertes = muertes;
            db.Update(existing);
        }
        else
        {
            db.Insert(new DB_Juega
            {
                codJugador = codJugador,
                codNivel = codNivel,
                tiempoCompletado = tiempo,
                muertes = muertes
            });
        }
    }

    public DB_Juega GetPartida(int codJugador, int codNivel)
    {
        return db.Table<DB_Juega>()
            .Where(j => j.codJugador == codJugador && j.codNivel == codNivel)
            .FirstOrDefault();
    }

    // Armas equipadas

    public void EquiparArma(int codJugador, int codArma)
    {
        var existing = db.Table<DB_Equipa>()
            .Where(e => e.codJugador == codJugador && e.codArma == codArma)
            .FirstOrDefault();

        if (existing == null)
            db.Insert(new DB_Equipa { codJugador = codJugador, codArma = codArma });
    }

    // Objetos

    public void GuardarObjeto(int codJugador, int codObjeto, bool obtenido)
    {
        var existing = db.Table<DB_Tiene>()
            .Where(t => t.codJugador == codJugador && t.codObjeto == codObjeto)
            .FirstOrDefault();

        if (existing != null)
        {
            existing.obtenido = obtenido;
            db.Update(existing);
        }
        else
        {
            db.Insert(new DB_Tiene
            {
                codJugador = codJugador,
                codObjeto = codObjeto,
                obtenido = obtenido
            });
        }
    }

    void OnDestroy()
    {
        db?.Close();
    }
}