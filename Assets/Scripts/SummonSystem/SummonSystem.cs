using UnityEngine;

public class SummonSystem
{
    private PlayerData playerData;
    private InventorySystem inventory;
    private KnowledgeSystem knowledge;

    //Creamos el constructor
    public SummonSystem(PlayerData playerData, InventorySystem inventory, KnowledgeSystem knowledge)
    {
        this.playerData = playerData;
        this.inventory = inventory;
        this.knowledge = knowledge;
    }

    // ─────────────────────────────────────────
    // VALIDACION
    // ─────────────────────────────────────────

    //Comprueba si el jugador puede invocar el monster de la receta, valida el conocimiento e ingredientes sin consumir nada
    public bool CanSummon(SummonRecipe recipe)
    {
        //Comprobacion de seguridad
        if (recipe == null || recipe.outputMonster == null) return false;

        //Comprobamos si el jugador tiene el nivel de conocimiento requerido
        if(knowledge.GetKnowledgeLevel(recipe.outputMonster.MonsterID) < recipe.requiredKnowledgeLevel)
        {
            Debug.Log("Conocimiento insuficiente para invocar " + recipe.outputMonster.MonsterName);
            return false;
        }

        //Comprobaciones de seguridad del ingrediente principal
        if(recipe.mainIngredient != null && recipe.mainIngredient.item != null)
        {
            //Si el inventario no contiene la cantidad necesaria o el item principal necesario devuelve false
            if (!inventory.HasItem(recipe.mainIngredient.item.ItemID, recipe.mainIngredient.quantity))
            {
                Debug.Log("Falta ingrediente principal: " + recipe.mainIngredient.item.ItemName);
                return false;
            }
        }

        //Creamos un bucle que recorre los ingredientes secundarios
        foreach(RecipeIngredient ingredient in recipe.secondaryIngredients)
        {
            //Comprobacion de seguridad de ingrediente secundario
            if (ingredient == null || ingredient.item == null) continue;

            //Si el inventario no contiene la cantidad necesaria o el item secundario necesario devuelve false
            if(!inventory.HasItem(ingredient.item.ItemID, ingredient.quantity))
            {
                Debug.Log("Falta ingrediente secundario: " + ingredient.item.ItemName);
                return false;
            }
        }

        //Si ha conseguido pasar todas estas comprobaciones devuelve true ya que si que se puede invocar
        return true;
    }

    // ─────────────────────────────────────────
    // INVOCACION
    // ─────────────────────────────────────────

    //Intenta invocar el Monster de la receta, devuelve true si tiene exito. Si la party del player tiene hueco lo añade a esta y si no va a la reserve
    public bool TrySummon(SummonRecipe recipe)
    {
        //Comprobamos si se puede hacer Summon
        if(!CanSummon(recipe)) return false;

        //Comprobacion de seguridad del Main Item
        if(recipe.mainIngredient != null && recipe.mainIngredient.item != null)
        {
            //Descontamos el ingrediente principal del inventario
            inventory.RemoveItem(recipe.mainIngredient.item.ItemID, recipe.mainIngredient.quantity);
        }

        //Creamos un bucle que recorre los ingredientes secundarios
        foreach(RecipeIngredient ingredient in recipe.secondaryIngredients)
        {
            //Comprobacion de seguridad
            if(ingredient == null || ingredient.item == null) continue;

            //Descontamos el ingrediente secundario del inventario
            inventory.RemoveItem(ingredient.item.ItemID, ingredient.quantity);
        }

        //Creamos el MonsterSaveData del nuevo monster con valores iniciales
        MonsterSaveData newMonster = MonsterSerializer.CreateNew(recipe.outputMonster);

        //Si hay hueco en la party activa lo añadimos ahi
        if(playerData.activeParty.Count < PlayerData.MAX_ACTIVE_PARTY)
        {
            playerData.activeParty.Add(newMonster);
            Debug.Log("Monster invocado: " + recipe.outputMonster.MonsterName + " → party activa");
        }
        //Si no, va a la reserva
        else
        {
            playerData.reserve.Add(newMonster);
            Debug.Log("Monster invocado: " + recipe.outputMonster.MonsterName + " → reserva");
        }
 
        return true;
    }
}
