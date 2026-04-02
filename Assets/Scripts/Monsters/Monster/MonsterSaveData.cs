using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterSaveData
{
    public string monsterID;
    public int level;
    public int currentHP;
    public int currentBP;
    //Lista de IDs de los Moves que el Monster tiene aprendidos, el MoveDatabase los resuelve a MoveData al cargar
    public List<string> learnedMoveIDs = new List<string>();
}
