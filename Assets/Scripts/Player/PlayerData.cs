using System;
using System.Collections.Generic;
using UnityEngine;

//Script para almacenar de forma persistente la informacion del Player
[Serializable]
public class PlayerData
{
    //Tamaño maximo de la party activa en combate
    public const int MAX_ACTIVE_PARTY = 5;
    public const int INITIAL_RESERVE_CAPACITY = 25;

    public string playerName;
    public float playTime;
    public KnowledgeSaveData knowledge;

    //Monsters de la party activa
    public List<MonsterSaveData> activeParty;
    public int reserveCapacity;
    //Monsters en reserva
    public List<MonsterSaveData> reserve;

    //Inventario de items del jugador
    public List<InventoryItemSaveData> inventory;

    //IDs de las Essence Runes desbloqueadas permanentemente por el jugador
    public List<string> unlockedRuneIDs;

    public PlayerData(string name)
    {
        playerName = name;
        playTime = 0f;
        knowledge = new KnowledgeSaveData();
        activeParty = new List<MonsterSaveData>();
        reserve = new List<MonsterSaveData>();
        inventory = new List<InventoryItemSaveData>();
        reserveCapacity = INITIAL_RESERVE_CAPACITY;
        unlockedRuneIDs = new List<string>();
    }
}
