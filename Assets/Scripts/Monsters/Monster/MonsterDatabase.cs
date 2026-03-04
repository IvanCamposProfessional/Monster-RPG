using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterDatabase", menuName = "Database/Monster Database")]
public class MonsterDatabase : ScriptableObject
{
    public List<MonsterData> allMonsters;

    public MonsterData GetMonsterByID(string id){
        return allMonsters.Find(m => m.MonsterID == id);
    }
}
