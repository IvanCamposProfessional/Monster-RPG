using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Singleton que gestiona el menu de pausa en todas las escenas excepto el Main Menu
public class PauseMenuManager : MonoBehaviour
{
    //Creamos la Instancia del Manager
    public static PauseMenuManager Instance { get; private set; }

    [Header("Panel")]
    [SerializeField] private GameObject pauseMenuPanel;

    //Variable para guardar el estado actual del menu
    private bool isPaused = false;

    //Nombre de escenas donde el menu de pausa no debe funcionar
    private static readonly string[] nonPausableScenes = { "MainMenuScene" };

    private void Awake()
    {
        //Comprobacion de seguridad para no duplicar la Instance
        if(Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        //Hacemos DontDestroyOnLoad para que se mantenga entre escenas el Pause Menu
        DontDestroyOnLoad(gameObject);

        //Desactivamos el panel
        pauseMenuPanel.SetActive(false);
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
        //Siempre reseteamos al cambiar de escena, independientemente del estado anterior
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        if(isPaused) ResumeGame();
    }

    // Update is called once per frame
    void Update()
    {
        //Comprobamos cada frame si estamos en una escena donde la pausa esta bloqueada
        if(IsNonPausableScene(SceneManager.GetActiveScene().name)) return;

        //Comprobacion de seguridad
        if(Keyboard.current == null) return;

        //Al pulsar escape
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            //Si el juego está pausado volvemos al juego
            if (isPaused)
            {
                ResumeGame();
            }
            //Si el juego no está pausado
            else
            {
                PauseGame();
            }
        }
    }

    //Muestra el menu de pausa y para el tiempo del juego
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    //Oculta el menu de pausa y reanuda el tiempo del juego
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    //Guarda la partida y vuelve al menu principal
    //Se llama desde el boton guardar y salir
    public void OnSaveAndExitClicked()
    {
        //Restauramos el timeScale antes de cambiar de escena
        Time.timeScale = 1f;
        isPaused = false;

        // El GameManager guarda y carga la escena del menú principal
        GameManager.Instance.ReturnToMainMenu();
    }

    private bool IsNonPausableScene(string sceneName)
    {
        //Recorremos la lista de escenas no pausables
        foreach(string name in nonPausableScenes)
        {
            //Si el nombre de la escena actual coincide con algun nombre en la lista devolvemos true
            if(name == sceneName) return true;
        }

        //Si no devolvemos false
        return false;
    }
}
