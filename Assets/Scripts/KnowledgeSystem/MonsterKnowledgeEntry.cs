using System;
using UnityEngine;

[Serializable]
public class MonsterKnowledgeEntry
{
    //Para saber de que Monster es
    public string monsterID;
    public int knowledgeLevel;
    public int timesDefeated;
    public int timesSummoned;
    public bool maxLeveled;
    public bool encountered;

    public MonsterKnowledgeEntry(string id)
    {
        monsterID = id;
        knowledgeLevel = 0;
        timesDefeated = 0;
        timesSummoned = 0;
    }
}
