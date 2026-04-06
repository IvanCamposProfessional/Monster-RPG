using System;
using System.Collections.Generic;
using UnityEngine;

//Script para almacenar de forma persistente la informacion del Player
[Serializable]
public class PlayerData
{
    //Tamaño maximo de la party activa en combate
    public const int MAX_ACTIVE_PARTY = 5;

    public string playerName;
    public float playTime;
    public KnowledgeSaveData knowledge;

    //Monsters de la party activa
    public List<MonsterSaveData> activeParty;
    //Monsters en reserva
    public List<MonsterSaveData> reserve;

    //Inventario de items del jugador
    public List<InventoryItemSaveData> inventory;

    public PlayerData(string name)
    {
        playerName = name;
        playTime = 0f;
        knowledge = new KnowledgeSaveData();
        activeParty = new List<MonsterSaveData>();
        reserve = new List<MonsterSaveData>();
        inventory = new List<InventoryItemSaveData>();
    }
}
