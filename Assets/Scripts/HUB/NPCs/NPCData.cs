using System;
using System.Collections.Generic;
using UnityEngine;

//Bloque de dialogo condicionado a una flag del jugador, si requiredFlag esta vacio este bloque es el dialogo base
[Serializable]
public class NPCDialogueEntry
{
    //Flag que el jugador debe tener para que este bloque sea el activo, None = este bloque actua como dialogo por defecto
    public KnowledgeFlag requiredFlag;
    public List<string> lines;
}

//ScriptableObject que define los datos de un NPC
[CreateAssetMenu(fileName = "NPCData", menuName = "NPC/NPC Data")]
public class NPCData : ScriptableObject
{
    [Header("Identidad")]
    //Identificador unico del NPC
    public string npcId;
    public string npcName;
    public Sprite portrait;

    [Header("Condicion de aparicion en HUB")]
    //Flag que debe tener el jugador para que este NPC aparezca en el Hub, None = el NPC aparece siempre
    public KnowledgeFlag requiredFlag;

    [Header("Dialogos en HUB")]
    //Lista de bloques de dialogo ordenados de menos a mas avanzado, el primer bloque debe tener requiredFlag vacio
    public List<NPCDialogueEntry> dialogueEntries;

    // ─────────────────────────────────────────
    // CONSULTAS
    // ─────────────────────────────────────────

    //Devuelve las lineas del bloque de dialogo mas avanzado disponible para el jugador
    public List<string> GetCurrrentDialogue()
    {
        //Comprobacion de seguridad
        if (dialogueEntries == null || dialogueEntries.Count == 0) return null;

        //Guardamos el Knowledge del Player
        KnowledgeSystem knowledge = GameManager.Instance.Knowledge;

        //Recorremos en orden inverso para devolver el bloque mas avanzado posible
        for(int i = dialogueEntries.Count -1; i >= 0; i--)
        {
            //Guardamos la entry actual del bucle
            NPCDialogueEntry entry = dialogueEntries[i];

            //Si no tiene requisito o el jugador tiene la flag, este bloque es el activo
            if (entry.requiredFlag == KnowledgeFlag.None || knowledge.HasFlag(entry.requiredFlag))
                return entry.lines;;
        }
 
        return null;
    }
}
