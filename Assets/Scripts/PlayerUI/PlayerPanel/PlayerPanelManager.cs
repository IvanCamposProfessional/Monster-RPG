using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

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

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
 
        playerPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
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
}
