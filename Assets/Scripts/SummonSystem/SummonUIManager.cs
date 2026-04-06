using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

//Singleton que gestiona el panel de invocacion en la escena
public class SummonUIManager : MonoBehaviour
{
    public static SummonUIManager Instance { get; private set; }

    [Header("Panel principal")]
    [SerializeField] private GameObject summonPanel;

    [Header("Cards")]
    //Prefab del componente Summon Recipe Card
    [SerializeField] private GameObject recipeCardPrefab;
    //Contenedor donde se instancian las cards
    [SerializeField] private Transform cardsContainer;

    [Header("Feedback")]
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TMP_Text feedbackText;
    //Color del panel para cuando se puede hacer Summon
    private static readonly Color colorOK = new Color(0.3f, 0.8f, 0.3f, 1f);
    //Color del panel para cuando no se puede hacer Summon
    private static readonly Color colorKO = new Color(0.9f, 0.3f, 0.3f, 1f);

    //Lista de cartas instanciadas para poder refrescarlas
    private List<SummonRecipeCard> activeCards = new List<SummonRecipeCard>();

    private void Awake()
    {
        //Comprobacion de seguridad para no duplicar la instancia
        if(Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        //El panel empieza oculto
        summonPanel.SetActive(false);
        feedbackPanel.SetActive(false);
    }

    // ─────────────────────────────────────────
    // ABRIR / CERRAR SUMMON PANEL
    // ─────────────────────────────────────────

    public void OpenSummonPanel()
    {
        //Construimos las cartas con las recetas disponibles segun el knowledge del jugador
        BuildCards();
        //Ocultamos el Feedback
        feedbackPanel.SetActive(false);
        //Mostramos el panel de Summon
        summonPanel.SetActive(true);
    }

    public void CloseSummonPanel()
    {
        summonPanel.SetActive(false);
    }

    // ─────────────────────────────────────────
    // CONSTRUCCION DE CARDS
    // ─────────────────────────────────────────

    private void BuildCards()
    {
        //Creamos un bucle que recorra las recipe cards anteriores
        foreach(SummonRecipeCard card in activeCards)
        {
            //Destruimos las cards anteriores
            if(card != null) Destroy(card.gameObject);
        }
        //Limpiamos la lista de active cards
        activeCards.Clear();

        //Creamos una lista para guardar las recetas disponibles segun el nivel de Knowledge
        List<SummonRecipe> availableRecipes = GameManager.Instance.RecipeDatabase.GetAvailableRecipes(GameManager.Instance.Knowledge);

        //Creamos un bucle que recorra las recetas disponibles
        foreach(SummonRecipe recipe in availableRecipes)
        {
            //Instanciamos el prefab de la recipe card en el card container
            GameObject cardObject = Instantiate(recipeCardPrefab, cardsContainer);
            //Accedemos al script del prefab
            SummonRecipeCard card = cardObject.GetComponent<SummonRecipeCard>();
            //Hacemos setup de la card
            card.Setup(recipe);
            //Añadimos la card creada a la lista de cards
            activeCards.Add(card);
        }
    }

    // ─────────────────────────────────────────
    // REFRESCO
    // ─────────────────────────────────────────

    //Refresca el color de todas las cartas activas
    public void RefreshAllCards()
    {
        //Creamos un bucle que recorre las active cards
        foreach(SummonRecipeCard card in activeCards)
        {
            //Refrescamos el color de la card
            if(card != null) card.RefreshColor();
        }
    }

    // ─────────────────────────────────────────
    // FEEDBACK
    // ─────────────────────────────────────────

    public void ShowFeedback(string message, bool success)
    {
        //Mostramos el mensaje de Feedback que le pasamos a la funcion
        feedbackText.text = message;
        //Cambiamos el color del texto segun si  ha ido OK o KO el Summon
        feedbackText.color = success ? colorOK : colorKO;
        //Mostramos el panel de feedback
        feedbackPanel.SetActive(true);
    }

    public void CloseFeedbackPanel()
    {
        feedbackPanel.SetActive(false);
    }
}
