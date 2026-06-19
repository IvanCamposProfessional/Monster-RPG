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

    [Header("Botones de Categoria")]
    [SerializeField] private GameObject basicMovesButton;
    [SerializeField] private GameObject essenceMovesButton;

    [Header("Essence Display")]
     //Prefab de una entrada de Essence
    [SerializeField] private GameObject essenceEntryPrefab;
    //Base de datos de iconos por tipo elemental
    [SerializeField] private TypeIconDatabase typeIconDatabase;

    [Header("Navegacion")]
    [SerializeField] private GameObject backButton;

    //Variable para saber la current unit del turno
    private MonsterUnit currentUnit;

    //Lista para guardar los botones que se instancian en el Combat Menu
    public List<GameObject> currentButtons = new List<GameObject>();

    private CanvasGroup canvasGroup;

    // ─────────────────────────────────────────
    // SUSCRIPCIONES
    // ─────────────────────────────────────────

    private void Awake()
    {
        GameEvents.OnCombatStarted += HandleCombatStarted;
        GameEvents.OnPlayerTurnStarted += HandlePlayerTurnStarted;
        GameEvents.OnPlayerTurnEnded += HandlePlayerTurnEnded;

        canvasGroup = GetComponent<CanvasGroup>();
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
        ShowBaseState();
        SetInteractable(false);
    }

    //Recibe la unidad actuva y muestra el panel para que el jugador elija
    private void HandlePlayerTurnStarted(MonsterUnit unit)
    {
        currentUnit = unit;
        ShowBaseState();
        SetInteractable(true);
    }

    //Oculta el menu una vez el jugador ha elegido movimiento
    private void HandlePlayerTurnEnded()
    {
        ShowBaseState();
        SetInteractable(false);
    }

    // ─────────────────────────────────────────
    // ESTADO BASE — BOTON DE MOVES
    // ─────────────────────────────────────────

    //Vuelve el menu al estado base
    private void ShowBaseState()
    {
        ClearMoveButtons();

        movesButton.SetActive(true);
        basicMovesButton.SetActive(false);
        essenceMovesButton.SetActive(false);
        backButton.SetActive(false);
    }

    // ─────────────────────────────────────────
    // SELECTOR DE CATEGORIA
    // ─────────────────────────────────────────

    //Muestra los botones de categoria de Basic y Essence
    public void ShowCategorySelector()
    {
        movesButton.SetActive(false);
        basicMovesButton.SetActive(true);
        essenceMovesButton.SetActive(true);
        backButton.SetActive(false);
    }

    // ─────────────────────────────────────────
    // SUBLISTA DE BASIC MOVES
    // ─────────────────────────────────────────

    public void ShowBasicMoves()
    {
        //Comprobacion de seguridad
        if (currentUnit == null) return;
 
        ClearMoveButtons();

        //Desactivamos los botones de categoria y dejamos solo el de Back
        basicMovesButton.SetActive(false);
        essenceMovesButton.SetActive(false);
        backButton.SetActive(true);

        //Creamos un bucle que recorra los Basic Moves
        foreach(var move in currentUnit.monster.learnedBasicMoves)
        {
            //Llamamos a la funcion que instancia un Basic Move Button
            InstantiateBasicMoveButton(move);
        }
    }

    private void InstantiateBasicMoveButton(MoveData move)
    {
        //Instanciamos el Prefab del Move Button en el Button Container
        GameObject moveBtn = Instantiate(moveButtonPrefab, buttonContainer);

        SetMoveNameText(moveBtn, move.MoveName);
        SetEssenceLabel(moveBtn, "To Pool");

        //Guardamos el EssenceContainer del Button
        Transform essenceContainer = moveBtn.transform.Find("EssenceContainer");

        //Comprobacion de seguridad
        if (essenceContainer != null)
            //Llamamos a la funcion que coloca la Essence Entry
            PopulateEssenceEntries(essenceContainer, move.EssenceAmountToPool);

        //Guardamos la MoveData del Move que se ha instanciado el Button
        MoveData capturedMove = move;

        //Configuramos el Listener del Button añadiendole que lance el GameEvent Raise Move Chosen pasandole el Move seleccionado
        moveBtn.GetComponent<Button>().onClick.AddListener(() => GameEvents.RaiseMoveChosen(capturedMove));

        //Añadimos el boton a la lista de Current Buttons
        currentButtons.Add(moveBtn);
    }

    // ─────────────────────────────────────────
    // SUBLISTA DE ESSENCE MOVES
    // ─────────────────────────────────────────

    public void ShowEssenceMoves()
    {
        //Comprobacion de seguridad
        if (currentUnit == null) return;
 
        ClearMoveButtons();

        //Desactivamos los botones de categoria y dejamos solo el de Back
        basicMovesButton.SetActive(false);
        essenceMovesButton.SetActive(false);
        backButton.SetActive(true);

        //Creamos un bucle que recorra los Essence Moves
        foreach(var move in currentUnit.monster.learnedEssenceMoves)
        {
            //Llamamos a la funcion que instancia un Essence Move Button
            InstantiateEssenceMoveButton(move);
        }
    }

    private void InstantiateEssenceMoveButton(MoveData move)
    {
        //Instanciamos el Prefab del Move Button en el Button Container
        GameObject moveBtn = Instantiate(moveButtonPrefab, buttonContainer);

        SetMoveNameText(moveBtn, move.MoveName);
        SetEssenceLabel(moveBtn, "To Use");

        //Guardamos el EssenceContainer del Button
        Transform essenceContainer = moveBtn.transform.Find("EssenceContainer");

        //Comprobacion de seguridad
        if (essenceContainer != null)
            //Llamamos a la funcion que coloca la Essence Entry
            PopulateEssenceEntries(essenceContainer, move.EssenceAmountToUse);

        //Guardamos el Button del Move
        Button button = moveBtn.GetComponent<Button>();

        //Guardamos si CanAfford del Move es true o false
        bool canAfford = CombatManager.Instance.CanAllyAffordMove(move);

        //Si no puede pagarse coloreamos el boton en rojo
        if (!canAfford)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.8f, 0.2f, 0.2f);
            colors.highlightedColor = new Color(1f, 0.3f, 0.3f);
            button.colors = colors;
        }

        //Guardamos la MoveData del Move que se ha instanciado el Button
        MoveData capturedMove = move;

        button.onClick.AddListener(() => 
        {
            //Validacion final en el momento del click por si la pool cambio
            if (CombatManager.Instance.CanAllyAffordMove(capturedMove))
                //Configuramos el Listener del Button añadiendole que lance el GameEvent Raise Move Chosen pasandole el Move seleccionado
                GameEvents.RaiseMoveChosen(capturedMove);
        });

        //Añadimos el boton a la lista de Current Buttons
        currentButtons.Add(moveBtn);
    }

    // ─────────────────────────────────────────
    // VOLVER AL ESTADO BASE
    // ─────────────────────────────────────────

    public void GoBack()
    {
        ShowBaseState();
    }

    // ─────────────────────────────────────────
    // HELPERS DE DISPLAY
    // ─────────────────────────────────────────

    private void SetMoveNameText(GameObject btn, string name)
    {
        TMP_Text nameText = btn.transform.Find("MoveName")?.GetComponent<TMP_Text>();
        if (nameText != null)
            nameText.text = name;
    }
    
    private void SetEssenceLabel(GameObject btn, string label)
    {
        TMP_Text labelText = btn.transform.Find("EssenceLabel")?.GetComponent<TMP_Text>();
        if (labelText != null)
            labelText.text = label;
    }

    //Instancia una entrada por cada EssenceAmount en el contenedor del boton
    private void PopulateEssenceEntries(Transform container, List<EssenceAmount> entries)
    {
        //Comprobacion de seguridad
        if (entries == null) return;

        //Creamos un bucle que recorre las entries de EssenceAmount del Move
        foreach (var entry in entries)
        {
            //Instanciamos la Entry en el Essence Container del Button
            GameObject entryObj = Instantiate(essenceEntryPrefab, container);

            //Guardamos la Imagen del Type del Prefab
            Image icon = entryObj.GetComponentInChildren<Image>();
            //Comprobacion de seguridad
            if (icon != null)
            {
                //Ponemos el icon del Type de la Entry
                Sprite typeSprite = typeIconDatabase.GetIconByType(entry.Type);
                //Comprobacion de seguridad
                if (typeSprite != null)
                    icon.sprite = typeSprite;
            }

            //Guardamos el texto del amount
            TMP_Text amountText = entryObj.GetComponentInChildren<TMP_Text>();
            //Comprobacion de seguridad
            if (amountText != null)
                //Cambiamos el texto del Amount
                amountText.text = entry.Amount.ToString();
        }
    }

    // ─────────────────────────────────────────
    // UTILIDADES
    // ─────────────────────────────────────────

    private void ClearMoveButtons()
    {
        //Creamos un bucle que recorra la lista de Current Buttons
        foreach (var button in currentButtons)
            //Destruimos el Game Object del Button
            Destroy(button);
        //Limpiamos la lista de Current Buttons
        currentButtons.Clear();
    }
    
    //Funcion para ocultar el Combat Menu
    /*public void HideMenu()
    {
        //Ocultamos el panel
        gameObject.SetActive(false);
    }*/

    //Funcion para hacer interactable o no el CombatMenu
    private void SetInteractable(bool interactable)
    {
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
        canvasGroup.alpha = interactable ? 1f : 0.4f;
    }
}
