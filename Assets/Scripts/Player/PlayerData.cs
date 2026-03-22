using System;
using System.Collections.Generic;
using UnityEngine;

//Script para almacenar de forma persistente la informacion del Player
[Serializable]
public class PlayerData
{
    public string playerName;
    public float playTime;
    public KnowledgeSaveData knowledge;
    public List<MonsterSaveData> party;

    public PlayerData(string name)
    {
        playerName = name;
        playTime = 0f;
        knowledge = new KnowledgeSaveData();
        party = new List<MonsterSaveData>();
    }
}
