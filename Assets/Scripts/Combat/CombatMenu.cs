using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CombatMenu : MonoBehaviour
{
     [Header("Referencias UI")]
    //Variable para guardar el Prefab del boton de Move
    [SerializeField] private GameObject moveButtonPrefab;
    //Panel Combat Menu
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject movesButton;

    //Variable para saber la current unit del turno
    private MonsterUnit currentUnit;

    //Lista para guardar los botones que se instancian en el Combat Menu
    public List<GameObject> currentButtons = new List<GameObject>();

    // ─────────────────────────────────────────
    // SUSCRIPCIONES
    // ─────────────────────────────────────────

    private void Awake()
    {
        GameEvents.OnCombatStarted += HandleCombatStarted;
        GameEvents.OnPlayerTurnStarted += HandlePlayerTurnStarted;
        GameEvents.OnPlayerTurnEnded += HandlePlayerTurnEnded;
    }

    private void OnDestroy()
    {
        GameEvents.OnCombatStarted    -= HandleCombatStarted;
        GameEvents.OnPlayerTurnStarted -= HandlePlayerTurnStarted;
        GameEvents.OnPlayerTurnEnded  -= HandlePlayerTurnEnded;
    }

    // ─────────────────────────────────────────
    // HANDLERS
    // ─────────────────────────────────────────

    //Oculta el menu al arrancar el combate
    private void HandleCombatStarted()
    {
        HideMenu();
    }

    //Recibe la unidad actuva y muestra el panel para que el jugador elija
    private void HandlePlayerTurnStarted(MonsterUnit unit)
    {
        currentUnit = unit;
        ResetToInitialState();
        gameObject.SetActive(true);
    }

    //Oculta el menu una vez el jugador ha elegido movimiento
    private void HandlePlayerTurnEnded()
    {
        HideMenu();
    }

    // ─────────────────────────────────────────
    // MENU DE MOVIMIENTOS
    // ─────────────────────────────────────────

    //Funcion para definir lo que ocurre cuando se muestre el menu, tenemos que pasarle una Monster Unit para saber los moves que tiene
    public void ShowMenu()
    {
        //Por seguridad
        if(currentUnit == null)
            return;

        //Limpiar botones anteriores
        foreach (var button in currentButtons)
            Destroy(button);

        //Limpiar la lista de los botones anteriores
        currentButtons.Clear();

        //Desactivamos el boton de moves
        movesButton.SetActive(false);

        //Instancias un boton por cada ataque que tiene aprendido la Monster Unit
        foreach(var move in currentUnit.monster.learnedMoves)
        {
            //Instanciamos un prefab button en el Combat Menu
            GameObject moveBtn = Instantiate(moveButtonPrefab, buttonContainer);
            //Cambiamos el texto del boton al nombre del Move
            moveBtn.GetComponentInChildren<TMP_Text>().text = move.MoveName;

            //Capturamos move en variable local para el closure del listener
            MoveData capturedMove = move;

            //Añadimos un listener al boton
            moveBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                //Publica el movimiento elegido y el CombatManager escucha y reacciona
                GameEvents.RaiseMoveChosen(capturedMove);
            });

            //Guardamos el boton en la lista de los botones activos
            currentButtons.Add(moveBtn);
        }

        //Mostramos el panel
        gameObject.SetActive(true);
    }

    //Funcion para ocultar el Combat Menu
    public void HideMenu()
    {
        //Ocultamos el panel
        gameObject.SetActive(false);
    }

    private void ResetToInitialState()
    {
        //Destruimos los botones de moves del turno anterior
        foreach (var button in currentButtons)
            Destroy(button);
        currentButtons.Clear();

        //Volvemos a mostrar el boton principal de Moves
        movesButton.SetActive(true);
    }
}
