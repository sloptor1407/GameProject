using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ProfileManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject profilesPanel;
    [SerializeField] GameObject createProfilePanel;

    [Header("Profiles List")]
    [SerializeField] Transform profilesContainer;
    [SerializeField] GameObject profileButtonPrefab;

    [Header("Create Profile")]
    [SerializeField] TMP_InputField nameInputField;

    MainMenuManager menuManager;

    void Awake()
    {
        menuManager = GetComponent<MainMenuManager>();
    }

    void Start()
    {
        HideAllProfilePanels();
        LoadProfiles();
    }

    // Navegación

    public void ShowProfiles()
    {
        profilesPanel.SetActive(true);
        createProfilePanel.SetActive(false);
        LoadProfiles();
    }

    public void ShowCreateProfile()
    {
        profilesPanel.SetActive(false);
        createProfilePanel.SetActive(true);
        nameInputField.text = "";
    }

    public void HideAllProfilePanels()
    {
        profilesPanel.SetActive(false);
        createProfilePanel.SetActive(false);
    }

    // Lógica

    void LoadProfiles()
    {
        foreach (Transform child in profilesContainer)
            Destroy(child.gameObject);

        List<DB_Jugador> jugadores = DatabaseManager.Instance.GetTodosLosJugadores();
        Debug.Log($"Perfiles encontrados: {jugadores.Count}");

        if (jugadores.Count == 0)
        {
            ShowCreateProfile();
            return;
        }

        profilesPanel.SetActive(true);
        foreach (DB_Jugador jugador in jugadores)
        {
            GameObject btn = Instantiate(profileButtonPrefab, profilesContainer);
            ProfileButton profileBtn = btn.GetComponent<ProfileButton>();
            profileBtn.Setup(jugador, this);
        }
    }

    public void SelectProfile(DB_Jugador jugador)
    {
        GameSession.CodJugador = jugador.codJugador;
        GameSession.NombreJugador = jugador.nombreJugador;
        GameSession.MuertesTotales = 0;
        GameSession.MaxNivelDesbloqueado =
            DatabaseManager.Instance.GetNivelMaxDesbloqueado(jugador.codJugador);

        HideAllProfilePanels();
        menuManager.ShowMainMenu();
    }

    public void CreateProfile()
    {
        string nombre = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(nombre))
        {
            Debug.Log("Introduce un nombre válido");
            return;
        }

        int codJugador = DatabaseManager.Instance.CrearJugador(nombre);
        GameSession.CodJugador = codJugador;
        GameSession.NombreJugador = nombre;
        GameSession.MuertesTotales = 0;
        GameSession.MaxNivelDesbloqueado = 1;

        HideAllProfilePanels();
        menuManager.ShowMainMenu();
    }

    public void DeleteProfile(int codJugador)
    {
        DatabaseManager.Instance.EliminarJugador(codJugador);
        LoadProfiles();
    }
}