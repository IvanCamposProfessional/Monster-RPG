using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

//Singleton central que persiste entre escenas y mantiene el PlayerData activo en memoria, todos los sistemas acceden al KnowledgeSystem a traves de el
public class GameManager : MonoBehaviour
{
    //Creamos la instancia del GameManager
    public static GameManager Instance { get; private set; }

    //Datos de la partida activa en memoria
    public PlayerData CurrentPlayer { get; private set; }

    //El slot activo
    private int currentSlot = -1;

    //Referencia a la MonsterDatabase para pasarsela al KnowledgeSystem
    [SerializeField] private MonsterDatabase monsterDatabase;

    //El KnowledgeSystem opera sobre el PlayerData activo
    public KnowledgeSystem Knowledge { get; private set; }

    private bool isTrackingTime = false;

    private void Awake()
    {
        //Si ya existe una instancia del Game Manager la destruimos
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        //Guardamos la instancia e indicamos que no se destruya el GameObject entre escenas
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        //Acumulamos el tiempo jugado mientras hay partida activa
        if(isTrackingTime && CurrentPlayer != null)
        {
            CurrentPlayer.playTime += Time.deltaTime;
        }
    }

    //Crea una nueva partida en el slot indicado
    public void NewGame(int slot, string playerName = "Player")
    {
        //Guardamos el slot donde creamos la partida
        currentSlot = slot;
        //Guardamos el Player Data que le pasamos a la funcion
        CurrentPlayer = new PlayerData(playerName);
        //Creamos el Knowledge System
        Knowledge = new KnowledgeSystem(CurrentPlayer.knowledge, monsterDatabase);
        //Cambiamos el bool para empezar a contar el tiempo de partida
        isTrackingTime = true;
        Debug.Log("Nueva partida creada en slot " + slot);
    }

    //Carga la partida existente del slot indicado
    public void LoadGame(int slot)
    {
        //Guardamos el PlayerData del slot indicado
        PlayerData loaded = SaveSystem.Load(slot);

        //Si devuelve nulo la partida cargada hacemos return
        if(loaded == null)
        {
            Debug.LogWarning("No hay partida en slot " + slot);
            return;
        }

        //Guardamos el slot
        currentSlot = slot;
        //Guardamos el Player Data
        CurrentPlayer = loaded;
        //Creamos el Knowledge Data del Player
        Knowledge = new KnowledgeSystem(CurrentPlayer.knowledge, monsterDatabase);
        //Cambiamos el bool para empezar a contar el tiempo de partida
        isTrackingTime = true;
        Debug.Log("Partida cargada del slot " + slot);
    }

    public void SaveGame()
    {
        //Comprobaciones de seguridad
        if(CurrentPlayer == null || currentSlot < 0)
        {
            Debug.LogWarning("No hay partida activa para guardar");
            return;
        }

        //Guardamos la data del player en el slot actual
        SaveSystem.Save(CurrentPlayer, currentSlot);
    }

    //Guarda la partida, para el tracking del tiempo y vuelve al menu principal
    public void ReturnToMainMenu()
    {
        SaveGame();
        isTrackingTime = false;
        SceneManager.LoadScene("MainMenuScene");
    }
}
