using System;
using System.Collections.Generic;
using UnityEngine;

//Par de tiles: tile de la puerta en origen y tile de spawn en destino
[Serializable]
public class DoorTilePair
{
    //Tile donde debe estar el player para activar la transición
    public Vector2Int interactionTile;
    //Tile de spawn en la habitación destino
    public Vector2Int spawnTile;
}

public enum DoorDirection { Up, Down, Left, Right }

[CreateAssetMenu(fileName = "HubDoorData", menuName = "Hub/Door Data")]
public class HubDoorData : ScriptableObject
{
    [Header("Destino")]
    public string destinationRoomId;

    [Header("Tiles")]
    //Dirección desde la que el jugador accede a la puerta
    public DoorDirection entryDirection;
    //Lista de pares tile puerta → tile spawn en destino
    public List<DoorTilePair> tilePairs;

    [Header("Flags")]
    public KnowledgeFlag requiredFlag;
    public KnowledgeFlag blockedByFlag;
    [TextArea(1, 3)]
    public string lockedMessage;

    // ─────────────────────────────────────────
    // CONSULTAS
    // ─────────────────────────────────────────

    //Devuelve el par cuyo interactionTile está más cerca del player
    public DoorTilePair GetClosestPairToWorld(Vector2 worldClick, Vector2 gridOrigin)
    {
        DoorTilePair closest = null;
        float minDist = float.MaxValue;

        foreach (DoorTilePair pair in tilePairs)
        {
            // Convertimos el interactionTile a posición mundial para comparar
            Vector2 tileWorld = new Vector2(
                gridOrigin.x + pair.interactionTile.x + 0.5f,
                gridOrigin.y + pair.interactionTile.y + 0.5f
            );

            float dist = Vector2.Distance(worldClick, tileWorld);
            if (dist < minDist)
            {
                minDist = dist;
                closest = pair;
            }
        }

        return closest;
    }

    //Devuelve el par exacto si el player ya está en un interactionTile, null si no
    public DoorTilePair GetPairAtTile(Vector2Int playerTile)
    {
        foreach (DoorTilePair pair in tilePairs)
            if (pair.interactionTile == playerTile) return pair;

        return null;
    }

    //Comprueba si el jugador cumple las condiciones de flags para pasar
    public bool IsUnlocked(KnowledgeSystem knowledge)
    {
        if (requiredFlag != KnowledgeFlag.None && !knowledge.HasFlag(requiredFlag))
            return false;

        if (blockedByFlag != KnowledgeFlag.None && knowledge.HasFlag(blockedByFlag))
            return false;

        return true;
    }
}
