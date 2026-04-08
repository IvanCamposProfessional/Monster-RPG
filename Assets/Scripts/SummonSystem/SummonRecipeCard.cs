using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Componente del prefab de cada carta de receta en el panel de invocacion
public class SummonRecipeCard : MonoBehaviour, IPointerClickHandler
{
    private Image monsterIcon;

    [Header("Ingredientes")]
    //Prefab de una fila de ingredientes
    [SerializeField] private GameObject ingredientRowPrefab;
    //Contenedor donde se instancia la ingredient row
    private Transform ingredientsContainer;

    //Imagen de fondo de la card para colorearla segun si se puede hacer Summon
    [SerializeField] private Image cardBackground;
    //Colores rojo y verde para cambiar el card background segun si se puede hacer summon o no
    private static readonly Color colorOKSummon    = new Color(0.6f, 0.9f, 0.5f, 1f);
    private static readonly Color colorKOSummon = new Color(0.9f, 0.35f, 0.35f, 1f);

    //Receta que representa esta carta
    private SummonRecipe recipe;

    // ─────────────────────────────────────────
    // SETUP
    // ─────────────────────────────────────────

    //Configura la carta con la receta indicada
    public void Setup(SummonRecipe recipe)
    {
        this.recipe = recipe;

        //cardBackground.GetComponent<Image>();

        Transform iconTransform = transform.Find("MonsterIcon");
        if (iconTransform != null)
            monsterIcon = iconTransform.GetComponent<Image>();

        ingredientsContainer = transform.Find("IngredientContainer");

        //Comprobacion de seguridad
        if(recipe.outputMonster != null && recipe.outputMonster.MonsterIcon != null)
        {
            //Asignamos el sprite del Output Monster al Monster Icon de la card
            monsterIcon.sprite = recipe.outputMonster.MonsterIcon;
        }

        //Instanciamos las filas de los ingredientes
        BuildIngredientRows();

        //Coloreamos la carta segun si el jugador puede invocar o no
        RefreshColor();
    }

    //Construye las filas de ingredientes dentro del contenedor
    private void BuildIngredientRows()
    {
        //Creamos un bucle que recorra el ingredients container por si ya tenia alguna row
        foreach(Transform child in ingredientsContainer)
        {
            //Destruimos las rows en escena
            Destroy(child.gameObject);
        }

        //Comprobacion de seguridad
        if(recipe.mainIngredient != null && recipe.mainIngredient.item != null)
        {
            //Creamos la row del ingrediente principal
            CreateIngredientRow(recipe.mainIngredient);
        }

        //Creamos un bucle que recorra los ingredientes secundarios
        foreach(RecipeIngredient ingredient in recipe.secondaryIngredients)
        {
            //Comprobacion de seguridad
            if(ingredient == null || ingredient.item == null) continue;

            //Creamos la row del ingrediente secundario
            CreateIngredientRow(ingredient);
        }
    }

    //Instanciamos una fila de ingrediente con su icono y cantidad
    private void CreateIngredientRow(RecipeIngredient ingredient)
    {
        //Instanciamos la row del ingredient en el ingredients container
        GameObject row = Instantiate(ingredientRowPrefab, ingredientsContainer);

        //Guardamos el icon del item
        Image icon = row.GetComponentInChildren<Image>();
        //Comprobacion de seguridad
        if(icon != null && ingredient.item.ItemSprite != null)
        {
            //Asignamos el icon del item
            icon.sprite = ingredient.item.ItemSprite;
        }

        //Guardamos el texto de la quantity
        TMP_Text quantityText = row.GetComponentInChildren<TMP_Text>();
        //Comprobacion de seguridad
        if(quantityText != null)
        {
            //Guardamos cuanta cantidad tiene el player en el inventario
            int owned = GameManager.Instance.Inventory.GetQuantity(ingredient.item.ItemID);
            //Cambiamos el texto de la cantidad
            quantityText.text = owned + " / " + ingredient.quantity;
            //Cambiamos el color del texto a verde si tenemos la cantidad suficiente y a rojo si no disponemos de la cantidad
            quantityText.color = owned >= ingredient.quantity ? Color.green : Color.red;
        }
    }

    //Refresca los ingredientes y los colores de la card
    public void Refresh()
    {
        BuildIngredientRows();
        RefreshColor();
    }

    public void RefreshColor()
    {
        //Booleano que almacena si podemos hacer summon del monster
        bool canSummon = GameManager.Instance.Summon.CanSummon(recipe);
        //Cambiamos el color de la card segun can summon
        cardBackground.color = canSummon ? colorOKSummon : colorKOSummon;
    }

    // ─────────────────────────────────────────
    // CLICK
    // ─────────────────────────────────────────

    public void OnPointerClick(PointerEventData eventData)
    {
        //Intentamos hacer summon del monster y guardamos si se puede hacer o no
        bool success = GameManager.Instance.Summon.TrySummon(recipe);
        
        //Si success es true
        if (success)
        {
            //Si ha tenido exito refrescamos el color de todas las cartas ya que el inventario ha cambiado
            SummonUIManager.Instance.RefreshAllCards();
            //Mostramos el feedback del summon
            SummonUIManager.Instance.ShowFeedback("¡" + recipe.outputMonster.MonsterName + " invocado con exito!", true);
        }
        //Si success es false
        else
        {
            //Mostramos el feedback del summon
            SummonUIManager.Instance.ShowFeedback("No puedes invocar a " + recipe.outputMonster.MonsterName + ".", false);
        }
    }
}
