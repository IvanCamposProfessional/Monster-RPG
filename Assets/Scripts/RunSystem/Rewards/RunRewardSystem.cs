using System.Collections.Generic;
using UnityEngine;

//Clase que genera los rewards de la run segun el nodo, el piso y el estado del jugador
public class RunRewardSystem
{
    private EssenceRuneDatabase runeDatabase;

    public RunRewardSystem(EssenceRuneDatabase runeDatabase)
    {
        this.runeDatabase = runeDatabase;
    }

    // ─────────────────────────────────────────
    // PUNTO DE ENTRADA PUBLICO
    // ─────────────────────────────────────────

    //Genera la lista de rewards para un nodo de combate
    public void GenerateCombatReward(NodeType nodeType, int floorIndex, RunTypeData runTypeData, PlayerData playerData)
    {
        //Generamos y guardamos los pesos de los Rewards
        RewardWeightsData weights = runTypeData.GetRewardWeightsForNode(nodeType);

        //Guardamos la LootTable del node
        LootTableData lootTable = runTypeData.GetLootTableForNode(nodeType);

        //Generamos los Rewards con los pesos y llamamos al Game Event
        RewardPackage package = GeneratePackage(weights, lootTable, floorIndex, nodeType, playerData);
        GameEvents.RaiseRewardsReady(package);
    }

    //Genera la lista de rewards para un nodo Chest
    public void GenerateChestReward(int floorIndex, RunTypeData runTypeData, PlayerData playerData)
    {
        //Generamos y guardamos los pesos de los Rewards
        RewardWeightsData weights = runTypeData.GetRewardWeightsForNode(NodeType.Chest);

        //Guardamos la LootTable del node
        LootTableData lootTable = runTypeData.GetLootTableForNode(NodeType.Chest);

        //Generamos los Rewards con los pesos y llamamos al Game Event
        RewardPackage package = GeneratePackage(weights, lootTable, floorIndex, NodeType.Chest, playerData);
        GameEvents.RaiseRewardsReady(package);
    }

    // ─────────────────────────────────────────
    // GENERACION INTERNA
    // ─────────────────────────────────────────

    //Ejecuta el roll de RewardType y construye el Reward Package
    private RewardPackage GeneratePackage(RewardWeightsData weights, LootTableData lootTable, int floorIndex, NodeType nodeType, PlayerData playerData)
    {
        //Creamos el package de Rewards
        RewardPackage package = new RewardPackage();

        //Roll independiente de Rune, solo si el nodo tiene RewardWeightsData configurado
        if (weights != null)
            package.Rune = BuildRuneReward(weights, floorIndex, nodeType, playerData);

        //Roll independiente de Items, solo si el nodo tiene LootTableData configurado
        if (lootTable != null)
            package.Items = lootTable.RollLoot(floorIndex);
 
        return package;
    }

    // ─────────────────────────────────────────
    // CONSTRUCCION DE RUNE REWARD
    // ─────────────────────────────────────────

    private EssenceRune BuildRuneReward(RewardWeightsData weights, int floorIndex, NodeType nodeType, PlayerData playerData)
    {
        //Roll de probabilidad de drop de Rune
        if (!weights.RollRuneDrop())
            return null;
 
        //Roll de rareza
        RarityType? rolledRarity = weights.RollRarity(floorIndex);

        //Comprobacion de seguridad
        if (rolledRarity == null)
        {
            Debug.LogWarning("RunRewardSystem: no se pudo determinar rareza para Rune reward en nodo " + nodeType);
            return null;
        }

        //Rarezas permitidas para este nodo segun la rareza ganadora del roll, el roll ya filtra los pesos configurados en el Scriptable Object del nodo
        List<RarityType> allowedRarities = new List<RarityType> { rolledRarity.Value };

        //Tipos presentes en el equipo activo del jugador
        List<MonsterType> teamTypes = GetTeamTypes(playerData);

        //IDs de Runes ya desbloqueadas
        List<string> unlockedIDs = playerData.unlockedRuneIDs ?? new List<string>();

        //Pool de candidatas tras aplicar todos los filtros
        List<EssenceRune> candidates = runeDatabase.GetElegibleRunes(allowedRarities, unlockedIDs, teamTypes);

        //Comprobacion de seguridad
        if (candidates.Count == 0)
        {
            Debug.Log("RunRewardSystem: no hay Runes elegibles para el equipo actual en nodo " + nodeType);
            return null;
        }

        //Devuelve las candidates generando un random entre ellas
        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }

    // ─────────────────────────────────────────
    // UTILIDADES
    // ─────────────────────────────────────────

    //Extrae los MonsterType unicos presentes en el equipo activo del jugador
    private List<MonsterType> GetTeamTypes(PlayerData playerData)
    {
        //Creamos una lista de los types
        List<MonsterType> types = new List<MonsterType>();
 
        //Comprobacion de seguridad
        if (playerData.activeParty == null) return types;

        //Creamos un bucle que recorre los Monster Save Data de la Active Party
        foreach (MonsterSaveData monsterSave in playerData.activeParty)
        {
            //Comprobacion de seguridad
            if (!types.Contains(monsterSave.monsterType))
                //Añadimos le Monster Type a la lista
                types.Add(monsterSave.monsterType);
        }

        return types;
    }
}
