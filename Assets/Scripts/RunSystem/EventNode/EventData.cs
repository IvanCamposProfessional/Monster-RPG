using System.Collections.Generic;
using UnityEngine;

//ScriptableObject que define un evento individual de la run
[CreateAssetMenu(fileName = "EventData", menuName = "Run/Event Data")]
public class EventData : ScriptableObject
{
    [Header("Identidad")]
    public string eventId;
    public EventType eventType;

    // ─────────────────────────────────────────
    // SELECCION PONDERADA
    // ─────────────────────────────────────────

    //Peso relativo en la seleccion aleatoria entre eventos elegibles
    [Min(0)] public float eventWeight = 1f;

    // ─────────────────────────────────────────
    // CONDICIONES DE APARICION
    // ─────────────────────────────────────────

    //Flag que DEBE tener el jugador para que este evento pueda aparecer, si está vacio no hay requisito
    public string requiredFlag;
    //Flag que BLOQUEA este evento si el jugador ya la tiene, si está vacio no hay bloqueo
    public string blockedByFlag;

    // ─────────────────────────────────────────
    // RECOMPENSAS
    // ─────────────────────────────────────────

    //Flag que se otorga al jugador al completar este evento, si esta vacio no se otorga ninguna Flag
    public string flagToGrant;
    //Item que se entrega al jugador al completar este evento, si es null no se entrega ningun Item
    public ItemData itemReward;
    //Cantidad del item que se otorga, solo relevante si el itemReward no es null
    [Min(1)] public int itemRewardQuantity = 1;

    // ─────────────────────────────────────────
    // DATOS DE NPC (solo si eventType == NPC)
    // ─────────────────────────────────────────

    //ID del NPC involucrado en este evento, debe coincidir con NPCData.npcId
    public string npcId;
    //Lineas de dialogo que se muestran durante el evento
    public List<string> dialogueLines;
}
