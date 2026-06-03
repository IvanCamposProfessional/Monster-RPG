using System;
using System.Collections.Generic;
using UnityEngine;

//Gestiona la party enemiga de un combate de run, lee el contexto de RunCombatContes para saber que encuentro cargar
public class Enemy : MonoBehaviour
{
    public MonsterDatabase monsterDatabase;
    public MoveDatabase moveDatabase;

    //Lista de combat encounter disponible
    public List<CombatEncounterData>  encounterDataList;
    public List<Monster> party { get; private set; } = new List<Monster>();

    private void Awake()
    {
        LoadPartyFromContext();
    }

    // ─────────────────────────────────────────
    // CARGA
    // ─────────────────────────────────────────

    private void LoadPartyFromContext()
    {
        //Si no hat contexto activo usamos un encuentro de fallback para testing
        if (!RunCombatContext.IsSet)
        {
            Debug.LogWarning("Enemy: no hay RunCombatContext activo — usando fallback de testing");
            LoadFallbackParty();
            return;
        }

        //Buscamos el CombatEncounterData que coincide con el tema de la run
        CombatEncounterData encounterData = encounterDataList.Find(e => e.themeType == RunCombatContext.ThemeType);

        if(encounterData == null)
        {
            Debug.LogWarning("Enemy: no hay CombatEncounterData para el tema " + RunCombatContext.ThemeType);
            LoadFallbackParty();
            return;
        }

        //Obtenemos un encuentro aleatorio para el piso actual
        EncounterGroup encounter = encounterData.GetRandomEncounter(RunCombatContext.FloorIndex);

        if (encounter == null)
        {
            Debug.LogWarning("Enemy: encuentro null para piso " + RunCombatContext.FloorIndex);
            LoadFallbackParty();
            return;
        }

        //Construimos la party a partir del encuentro
        foreach(EncounterEnemyEntry entry in encounter.enemies)
        {
            Monster monster = BuildMonster(entry);
            if (monster != null)
                party.Add(monster);
        }

        Debug.Log("Enemy: party cargada con " + party.Count + " monsters del encuentro " + encounter.encounterId);
    }

    //Construye un Monster runtime a partir de un EncounterEnemyEntry
    private Monster BuildMonster(EncounterEnemyEntry entry)
    {
        MonsterData data = monsterDatabase.GetMonsterByID(entry.monsterId);

        if (data == null)
        {
            Debug.LogWarning("Enemy: MonsterData no encontrada para ID " + entry.monsterId);
            return null;
        }

        Monster monster = new Monster(data, entry.level, data.BaseHP);

        //Añadimos los moves definidos en el encuentro
        foreach(string moveId in entry.moveIDs)
        {
            MoveData move = moveDatabase.GetMoveByID(moveId);

            if (move == null)
            {
                Debug.LogWarning("Enemy: MoveData no encontrada para ID " + moveId);
                continue;
            }

            if(move.ActionType == MoveActionType.Basic)
                monster.learnedBasicMoves.Add(move);
            else
                monster.learnedEssenceMoves.Add(move);
        }

        // Cargamos la AI del monster
        monster.enemyAI = Resources.Load<EnemyAI>("Monsters/EnemyAI/GenericEnemyAI");

        return monster;
    }

    //Fallback para poder testear la CombatScene sin pasar por la RunScene
    private void LoadFallbackParty()
    {
        if (monsterDatabase == null) return;
 
        MonsterData fallbackData = monsterDatabase.GetMonsterByID("1");
        if (fallbackData == null) return;
 
        Monster fallback = new Monster(fallbackData, 1, fallbackData.BaseHP);
 
        // Añadimos el primer move disponible a la lista correcta segun su ActionType
        if (fallbackData.LerneableMoves != null && fallbackData.LerneableMoves.Count > 0)
        {
            MoveData firstMove = fallbackData.LerneableMoves[0].Move;
            if (firstMove != null)
            {
                if (firstMove.ActionType == MoveActionType.Basic)
                    fallback.learnedBasicMoves.Add(firstMove);
                else
                    fallback.learnedEssenceMoves.Add(firstMove);
            }
        }
 
        fallback.enemyAI = Resources.Load<EnemyAI>("Monsters/EnemyAI/GenericEnemyAI");
 
        party.Add(fallback);
        Debug.Log("Enemy: party fallback cargada con " + fallbackData.MonsterName);
    }
}
