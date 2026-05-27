using System;
using System.Collections.Generic;

[Serializable]
public class MonsterSaveData
{
    public string monsterID;
    public int level;
    public int currentHP;

    //ID de los Basic Moves aprendidos
    public List<string> learnedBasicMoveIDs = new List<string>();
    //ID de los Essence Moves aprendidos
    public List<string> learnedEssenceMoveIDs = new List<string>();
}
