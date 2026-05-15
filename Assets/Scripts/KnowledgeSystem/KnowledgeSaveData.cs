using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KnowledgeSaveData
{
    //Flags de progresion almacenadas como nombres del enum KnowledgeFlags
    public List<string> flags = new List<string>();
    public List<MonsterKnowledgeEntry> monsterEntries = new List<MonsterKnowledgeEntry>();
}
