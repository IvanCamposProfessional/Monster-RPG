using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject pauseMenuPanel;

    //Variable para guardar el estado actual del menu
    private bool isPaused = false;

    // Update is called once per frame
    void Update()
    {
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
}
