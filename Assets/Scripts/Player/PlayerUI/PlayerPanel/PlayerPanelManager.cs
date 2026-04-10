using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Singleton que gestiona el panel principal del jugador, controla las pestañas Monsters, Grimoire e Inventory
public class PlayerPanelManager : MonoBehaviour 
{

    public static PlayerPanelManager Instance { get; private set; }

    [Header("Panel principal")]
    [SerializeField] private GameObject playerPanel;

    [Header("Pestañas")]
    [SerializeField] private GameObject monstersTab;
    //[SerializeField] private GameObject grimoireTab;
    //[SerializeField] private GameObject inventoryTab;

    [Header("Monsters")]
    [SerializeField] private MonstersTabManager monstersTabManager;

    //Variable para saber si el panel está abierto
    private bool isOpen = false;

    //Nombre de escenas donde el player panel no debe funcionar
    private static readonly string[] nonPlayerPanelScenes = { "MainMenuScene" };

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
 
        playerPanel.SetActive(false);
    }

    private void OnEnable()
    {
        //Pasamos la escena a OnSceneLoadeed
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        //Quitamos la escena de OnSceneLoaded
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Al cambiar de escena reseteamos el estado de pausa por seguridad
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Si el panel esta activo lo cierra
        if(isOpen) ClosePanel();
    }

    // Update is called once per frame
    void Update()
    {
        //Si estamos en una escena donde el panel esta bloqueado no procesamos input
        if (IsNonPlayerPanelScene(SceneManager.GetActiveScene().name)) return;

        //Comprobacion de seguridad
        if (Keyboard.current == null) return;

        //Al pulsar E abrimos o cerramos el panel
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            //Si el panel esta abierto
            if (isOpen)
            {
                //Cerramos el panel
                ClosePanel();
            }
            //Si no esta abierto
            else
            {
                //Abrimos el Panel
                OpenPanel();
            }
        }
    }

    public void OpenPanel()
    {
        //Al abrir mostramos siempre la pestaña de Monsters por defecto
        ShowMonstersTab();
        //Activamos el Player Panel
        playerPanel.SetActive(true);
        //Guardamos que el panel esta abierto
        isOpen = true;
    }

    public void ClosePanel()
    {
        //Desactivamos el Player Panel
        playerPanel.SetActive(false);
        //Guardamos que el panel esta cerrado
        isOpen = false;
    }

    // ─────────────────────────────────────────
    // PESTANAS
    // ─────────────────────────────────────────

    public void ShowMonstersTab()
    {
        //Activamos el Tab de Monsters
        monstersTab.SetActive(true);
        //Construimos los slots cada vez que se abre para reflejar el estado actual de la party
        monstersTabManager.BuildSlots();
    }

    // ─────────────────────────────────────────
    // PRIVADOS
    // ─────────────────────────────────────────

    private bool IsNonPlayerPanelScene(string sceneName)
    {
        foreach (string name in nonPlayerPanelScenes)
        {
            if (name == sceneName) return true;
        }
        return false;
    }
}
