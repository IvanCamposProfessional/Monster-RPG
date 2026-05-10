using System;
using System.Collections.Generic;
using UnityEngine;

//Entrada de un Enemy en un Encounter, define que monster es, su nivel y sus moves
[Serializable]
public class EncounterEnemyEntry
{
    //ID del monster en la Monster Database
    public string monsterId;
    public int level;
    //IDs de los moves que tendra este enemy en combate
    public List<string> moveIDs;
}

//Grupo de enemies que aparecen juntos
[Serializable]
public class EncounterGroup
{
    public string encounterId;
    public List<EncounterEnemyEntry> enemies;
}

//Pool de encuentros para un piso concreto
[Serializable]
public class FloorEncounterPool
{
    //Indice del piso al que corresponde esta pool (piso 0 = 1)
    public int floorIndex;
    public List<EncounterGroup> posibleEncounters;
}

//ScriptableObject que define todos los encuentros de un tipo de run, el CombatManager lo lee usando el RunCombatContext
[CreateAssetMenu(fileName = "CombatEncounterData", menuName = "Run/Combat Encounter Data")]
public class CombatEncounterData : ScriptableObject
{
    [Header("Identidad")]
    public MonsterType themeType;

    [Header("Encuentros por piso")]
    public List<FloorEncounterPool> poolsByFloor;

    // ─────────────────────────────────────────
    // CONSULTAS
    // ─────────────────────────────────────────

    //Devuelve el EncounterGroup aleatorio para el piso indicado, si no hay pool devuelve null
    public EncounterGroup GetRandomEncounter(int floorIndex)
    {
        if (poolsByFloor == null) return null;

        //Creamos y guardamos la pool de encounters del floor
        FloorEncounterPool pool = poolsByFloor.Find(p => p.floorIndex == floorIndex);

        //Comorobacion de seguridad
        if (pool == null || pool.posibleEncounters == null || pool.posibleEncounters.Count == 0)
        {
            Debug.LogWarning("CombatEncounterData: no hay encuentros para el piso " + floorIndex + " en tema " + themeType);
            return null;
        }

        //Devolvemos un encounter random dentro de la pool que hemos guardado
        return pool.posibleEncounters[UnityEngine.Random.Range(0, pool.posibleEncounters.Count)];
    }
}
