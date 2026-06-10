using System;
using System.Collections.Generic;
using UnityEngine;

//Entrada individual de loot, cada entrada tiene su propia probabilidad de drop independiente del resto
[Serializable]
public class LootEntry
{
    public ItemData item;
    //Probabilidad de que este item aparezca en el reward (0 = nunca, 1 = siempre)
    [Range(0f, 1f)] public float dropChance;
    [Min(1)] public int minQuantity;
    [Min(1)] public int maxQuantity;
}

//Override de loot para un piso concreto, si existe un override para el floorIndex actual sustituye al default
[Serializable]
public class FloorLootOverride
{
    //Indice del piso al que aplica este override, 0 = piso 1
    public int floorIndex;
    public List<LootEntry> entries;
}

//ScriptableObject que define la tabla de loot de un nodo concreto
[CreateAssetMenu(fileName = "LootTableData", menuName = "Run/Loot Table")]
public class LootTableData : ScriptableObject
{
    [Header("Entradas por defecto")]
    public List<LootEntry> defaultEntries;

    [Header("Overrides por piso (opcional)")]
    public List<FloorLootOverride> perFloorOverrides;

    //Devuelve las entradas activas para el piso indicado
    public List<LootEntry> GetEntriesForFloor(int floorIndex)
    {
        //Si existe override
        if (perFloorOverrides != null)
        {
            //Buscamos el Floor Loot Override del piso
            FloorLootOverride match = perFloorOverrides.Find(o => o.floorIndex == floorIndex);
            //Si ha encontrado override para el piso devolvemos esta lista
            if (match != null && match.entries != null && match.entries.Count > 0)
                return match.entries;
        }

        //Si no se cumple la condicion anterior devolvemos las default entries
        return defaultEntries;
    }

    //Genera la lista de ItemRewardEntry haciendo roll independiente por cada entrada de un piso
    public List<ItemRewardEntry> RollLoot(int floorIndex)
    {
        //Creamos la lista de results
        List<ItemRewardEntry> results = new List<ItemRewardEntry>();

        //Creamos una lista de Entries que pueden salir por floor
        List<LootEntry> entries = GetEntriesForFloor(floorIndex);

        //Comprobacion de seguridad
        if (entries == null) return results;

        //Creamos un bucle que recorra las entries
        foreach (LootEntry entry in entries)
        {
            //Comprobacion de seguridad
            if (entry.item == null) continue;

            //Roll independiente por cada entrada
            if (UnityEngine.Random.value <= entry.dropChance)
            {
                //Guardamos una cantidad random del item entre el minimo y el maximo que hemos definido
                int quantity = UnityEngine.Random.Range(entry.minQuantity, entry.maxQuantity + 1);

                //Añadimos el result a la lista de Item Rewards
                results.Add(new ItemRewardEntry(entry.item, quantity));
            }
        }

        //Devolvemos los rewards
        return results;
    }
}
