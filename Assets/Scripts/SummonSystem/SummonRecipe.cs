using System;
using System.Collections.Generic;
using UnityEngine;

//Clase serializable que representa un ingrediente en la receta y la cantidad
//Se utiliza dentro del Crafting Recipe, no es un Scriptable Object independiente
[Serializable]
public class RecipeIngredient
{
    public ItemData item;
    public int quantity = 1;
}

//Receta de invocacion de un Monster
[CreateAssetMenu(fileName = "SummonRecipe", menuName = "Summon/Recipe")]
public class SummonRecipe : ScriptableObject
{
    [Header("Monster que se invoca")]
    public MonsterData outputMonster;

    [Header("Requisito de conocimiento")]
    //Nivel minimo de MonsterKnowledge que el jugador necesita para desbloquear esta receta
    [Range(1, 3)]
    public int requiredKnowledgeLevel = 1;

    [Header("Ingredientes")]
    public RecipeIngredient mainIngredient;
    public List<RecipeIngredient> secondaryIngredients;
}
