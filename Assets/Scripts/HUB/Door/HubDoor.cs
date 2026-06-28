using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Componente que va en cada puerta de la mansión, detecta clicks en su Collider2D y gestiona la interacción del jugador.
[RequireComponent(typeof(Collider2D))]
public class HubDoor : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private HubDoorData _data;

    public string DestinationRoomId => _data.destinationRoomId;
    public DoorDirection EntryDirection => _data.entryDirection;
    public bool IsUnlocked() => _data.IsUnlocked(GameManager.Instance.Knowledge);
    public string LockedMessage => _data.lockedMessage;

    // ─────────────────────────────────────────
    // INPUT
    // ─────────────────────────────────────────

    public void OnPointerClick(PointerEventData eventData)
    {
        if (HubManager.Instance.IsInputBlocked) return;

        HubRoom room = GetComponentInParent<HubRoom>();
        if (room == null || room.Data == null) return;

        //Obtenemos el tile actual del player
        Vector2Int playerTile = HubPlayerController.Instance.CurrentTile;

        //¿El player ya está en un tile de interacción de esta puerta?
        DoorTilePair exactPair = _data.GetPairAtTile(playerTile);

        if (exactPair != null)
        {
            if (!IsUnlocked())
            {
                Debug.Log("Puerta bloqueada: " + _data.lockedMessage);
                return;
            }

            //Transición directa
            HubPlayerController.Instance.TriggerDoorTransition(this, exactPair.spawnTile);
            return;
        }

        //Elegimos el par más cercano al punto de click
        Vector2 worldClick = Camera.main.ScreenToWorldPoint(eventData.position);
        DoorTilePair closestPair = _data.GetClosestPairToWorld(worldClick, room.Data.gridOrigin);
        if (closestPair == null) return;

        HubPlayerController.Instance.WalkToDoor(closestPair.interactionTile, this, closestPair.spawnTile);
    }

    // ─────────────────────────────────────────
    // DEBUG — GIZMOS
    // ─────────────────────────────────────────

    #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_data == null || _data.tilePairs == null) return;

            HubRoom room = GetComponentInParent<HubRoom>();
            if (room == null || room.Data == null) return;

            HubRoomData roomData = room.Data;

            foreach (DoorTilePair pair in _data.tilePairs)
            {
                // Tile de interacción en amarillo
                Gizmos.color = new Color(1f, 1f, 0f, 0.6f);
                Vector2 center = new Vector2(
                    roomData.gridOrigin.x + pair.interactionTile.x + 0.5f,
                    roomData.gridOrigin.y + pair.interactionTile.y + 0.5f
                );
                Gizmos.DrawCube(new Vector3(center.x, center.y, 0f), Vector3.one * 0.9f);
            }
        }
    #endif
}
