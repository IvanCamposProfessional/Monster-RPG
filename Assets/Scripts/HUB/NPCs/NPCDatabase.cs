using System.Collections.Generic;
using UnityEngine;

//ScriptableObject que centraliza todos los NPCData del juego
[CreateAssetMenu(fileName = "NPCDatabase", menuName = "NPC/NPC Database")]
public class NPCDatabase : ScriptableObject
{
    public List<NPCData> npcs;

    //Devuelve el NPCData correspondiente al ID indicado, null si no existe
    public NPCData
     GetNPCByID(string npcId)
    {
        return npcs.Find(n => n.npcId == npcId);
    }
}
