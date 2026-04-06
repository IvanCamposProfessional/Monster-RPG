using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KnowledgeSaveData
{
    public List<string> worldFlags = new List<string>();
    public List<string> npcFlags = new List<string>();
    public List<MonsterKnowledgeEntry> monsterEntries = new List<MonsterKnowledgeEntry>();
}
