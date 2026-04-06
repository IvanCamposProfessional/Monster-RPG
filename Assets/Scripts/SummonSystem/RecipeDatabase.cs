using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeDatabase", menuName = "Database/Recipe Database")]
public class RecipeDatabase : ScriptableObject
{
    public List<SummonRecipe> allRecipes;

    //Devuelve la receta de un Monster concreto por su ID
    public SummonRecipe GerRecipeByMonsterID(string monsterID)
    {
        //Devuelve la receta cuando el Output Monster de la receta no es nulo y el ID del Output Monster coincide con el que pasamos a la funcion
        return allRecipes.Find(r => r.outputMonster != null && r.outputMonster.MonsterID == monsterID);
    }

    //Devuelve todas las recetas que el jugador tiene disponibles segun su Monster Knowledge
    public List<SummonRecipe> GetAvailableRecipes(KnowledgeSystem knowledge)
    {
        return allRecipes.FindAll(r =>
        {
            //Comprobacion de seguridad
            if(r.outputMonster == null) return false;

            //Obtenemos el nivel de conocimiento del jugador para este monster (0 si no lo conoce)
            int knowledgeLevel = knowledge.GetKnowledgeLevel(r.outputMonster.MonsterID);

            //La receta esta disponible si el nivel de conocimiento es igual o supera al requerido
            return knowledgeLevel >= r.requiredKnowledgeLevel;
        });
    }
}
