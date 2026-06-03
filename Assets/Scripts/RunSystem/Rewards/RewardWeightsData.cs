using System;
using System.Collections.Generic;
using UnityEngine;

//Par de RarityType + peso relativo
[Serializable]
public class RarityWeight
{
    public RarityType rarityType;
    [Min(0)] public float weight;
}

//Override de pesos para un piso concreto
[Serializable]
public class FloorRewardOverride
{
    //Indice del piso al que aplica este override, el piso 1 es 0
    public int floorIndex;
    public List<RarityWeight> rarityWeights;
}

[CreateAssetMenu(fileName = "RewardWeightsData", menuName = "Run/Reward Weights")]
public class RewardWeightsData : ScriptableObject
{
    [Header("Probabilidad de que aparezca una Rune (0 = nunca, 1 = siempre)")]
    [Range(0f, 1f)] public float runeDropChance;
 
    [Header("Pesos de rareza por defecto")]
    public List<RarityWeight> defaultRarityWeights;

    [Header("Overrides por piso (opcional)")]
    public List<FloorRewardOverride> perFloorOverrides;

    public List<RarityWeight> GetRarityWeightsForFloor(int floorIndex)
    {
        //Si la lista de Override no está vacia cogemos los pesos de Override
        if (perFloorOverrides != null)
        {
            //Buscamos los Reward Override por piso
            FloorRewardOverride match = perFloorOverrides.Find(o => o.floorIndex == floorIndex);
            //Comprobacion de seguridad
            if (match != null && match.rarityWeights != null && match.rarityWeights.Count > 0)
                //Devolvemos los pesos de las rarirty de override
                return match.rarityWeights;
        }

        //Si override está vacio devolvemos los pesos default
        return defaultRarityWeights;
    }

    //Determina si aparece una Rune en este reward
    public bool RollRuneDrop()
    {
        return UnityEngine.Random.value <= runeDropChance;
    }

    //Realiza un roll ponderado sobre una lista de RarityWeights, devuelve la RarityType ganadora o null si l a lista esta vacia o todos los pesos a 0
    public RarityType? RollRarity(int floorIndex)
    {
        //Creamos una lista de pesos
        List<RarityWeight> weights = GetRarityWeightsForFloor(floorIndex);
        //Realizamos el roll de los pesos
        return RollFromRarityWeights(weights);
    }

    // ─────────────────────────────────────────
    // ROLLS PONDERADOS INTERNOS
    // ─────────────────────────────────────────

    private RarityType? RollFromRarityWeights(List<RarityWeight> weights)
    {
        //Comprobacion de seguridad
        if (weights == null || weights.Count == 0) return null;

        //Variable para sacar el total de los pesos
        float total = 0f;
        //Sumamos todos los pesos de la lista
        foreach (var w in weights) total += w.weight;
        if (total <= 0f) return null;
 
        //Sacamos un numero random entre 0 y el total de pesos
        float roll = UnityEngine.Random.Range(0f, total);
        //Creamos la variable para sumar el peso acumulado
        float cumulative = 0f;
 
        //Sumamos el peso acumulado y si el numero roll es menor que el acumulado devolvemos el RewardType
        foreach (var w in weights)
        {
            cumulative += w.weight;
            if (roll < cumulative) return w.rarityType;
        }
 
        //Fallback por precision float
        return weights[weights.Count - 1].rarityType;
    }
}
