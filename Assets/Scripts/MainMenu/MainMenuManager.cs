using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject slotsSelectionPanel;
    [SerializeField] private GameObject createPlayerPanel;

    [Header("Slot Buttons")]
    [SerializeField] private SlotButton slot0Button;
    [SerializeField] private SlotButton slot1Button;
    [SerializeField] private SlotButton slot2Button;

    [Header("Create Player Panel")]
    [SerializeField] private TMP_InputField playerNameInput;

    //Guardamos el slot seleccionado para usarlo al crear la partida
    private int selectedSlot;

    private void Start()
    {
        //Al iniciar mostramos el menu principal y ocultamos los slots
        ShowMainMenu();
    }
    
    public void OnStartGameClicked()
    {
        //Actualizamos la info de los slots antes de mostrarlos
        RefreshSlots();
        mainMenuPanel.SetActive(false);
        slotsSelectionPanel.SetActive(true);
    }

    public void OnExitClicked()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego");
    }

    public void OnSlotsBackClicked()
    {
        ShowMainMenu();
    }

    public void OnCreatePlayerBackClicked()
    {
        ShowSlotsMenu();
    }

    public void OnSlotClicked(int slot)
    {
        //Si el slot tiene partida guardada
        if (SaveSystem.SlotExists(slot))
        {
            //Cargamos la partida
            GameManager.Instance.LoadGame(slot);
            //Cargamos la escena de Hub
            SceneManager.LoadScene("HubScene");
        }
        //Si el slot no tiene partida guardada
        else
        {
            //Guardamos el slot seleccionado y mostramos el panel de creacion
            selectedSlot = slot;
            slotsSelectionPanel.SetActive(false);
            createPlayerPanel.SetActive(true);
        }
    }

    public void OnCreatePlayerClicked()
    {
        //Si el Input esta vacio usamos "Player" por defecto
        string playerName = string.IsNullOrEmpty(playerNameInput.text) ? "Player" : playerNameInput.text;

        //Creamos un New Game
        GameManager.Instance.NewGame(selectedSlot, playerName);
        //Guardamos la partida para crear PlayerData con el JSON del Slot
        GameManager.Instance.SaveGame();
        //Lanzamos la escena de Hub
        SceneManager.LoadScene("HubScene");
    }

    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        slotsSelectionPanel.SetActive(false);
    }

    private void ShowSlotsMenu()
    {
        slotsSelectionPanel.SetActive(true);
        createPlayerPanel.SetActive(false);
    }

    private void RefreshSlots()
    {
        //Creamos una lista de Slot Info en la que guardamos los Slots del Save System
        SlotInfo[] slots = SaveSystem.GetAllSlotsInfo();
        //Hacemos setup de los slots en el boton
        slot0Button.Setup(slots[0], this);
        slot1Button.Setup(slots[1], this);
        slot2Button.Setup(slots[2], this);
    }
}
