using UnityEngine;
using UnityEngine.EventSystems;

public enum InteractionType { Information, Management, NPC }
public enum ManagementType { Exchange, Runes, Summon, None }

//Componente universal de interactuables del HUB.
[RequireComponent(typeof(Collider2D))]
public class HubInteractable : MonoBehaviour, IPointerClickHandler
{
    [Header("Visibilidad")]
    //Flag que debe tener el jugador para que este interactuable sea visible, None = siempre visible
    public KnowledgeFlag visibilityFlag;

    [Header("Flags de interacción")]
    public KnowledgeFlag requiredFlag;
    public KnowledgeFlag blockedByFlag;

    [Header("Tipo de interacción")]
    public InteractionType interactionType;

    [Header("Information")]
    [TextArea(1, 4)]
    public string infoMessage;

    [Header("Management")]
    public ManagementType managementType;

    [Header("NPC")]
    public NPCData npcData;

    // ─────────────────────────────────────────
    // INICIALIZACIÓN
    // ─────────────────────────────────────────

    private void Awake()
    {
        GameEvents.OnFlagGranted += OnFlagGranted;
    }

    private void OnDestroy()
    {
        GameEvents.OnFlagGranted -= OnFlagGranted;
    }

    private void Start()
    {
        RefreshVisibility();
    }

    private void OnFlagGranted(KnowledgeFlag flag)
    {
        RefreshVisibility();
    }

    // ─────────────────────────────────────────
    // VISIBILIDAD
    // ─────────────────────────────────────────

    private void RefreshVisibility()
    {
        //Si no tiene flag de visibilidad siempre está visible
        if (visibilityFlag == KnowledgeFlag.None)
        {
            gameObject.SetActive(true);
            return;
        }

        bool visible = GameManager.Instance != null && GameManager.Instance.Knowledge.HasFlag(visibilityFlag);

        gameObject.SetActive(visible);
    }

    // ─────────────────────────────────────────
    // INPUT
    // ─────────────────────────────────────────

    public void OnPointerClick(PointerEventData eventData)
    {
        if (HubManager.Instance.IsInputBlocked) return;

        if (!CanInteract()) return;

        //Calculamos el tile adyacente más cercano al player
        Vector2Int playerTile = HubPlayerController.Instance.CurrentTile;
        Vector2Int targetTile = GetClosestAdjacentTile(playerTile);

        //Caminamos al tile y ejecutamos la acción al llegar
        HubPlayerController.Instance.WalkToTile(targetTile, () => this.Interact());
    }

    // ─────────────────────────────────────────
    // INTERACCIÓN
    // ─────────────────────────────────────────

    public void Interact()
    {
        switch (interactionType)
        {
            case InteractionType.Information:
                //TODO: mostrar panel de información
                Debug.Log("Info: " + infoMessage);
                HubManager.Instance.UnblockInput();
                break;

            case InteractionType.Management:
                ExecuteManagement();
                break;

            case InteractionType.NPC:
                if (npcData != null)
                {
                    //TODO: abrir NPCDialogueUI
                    Debug.Log("NPC: " + npcData.npcName);
                }
                HubManager.Instance.UnblockInput();
                break;
        }
    }

    private void ExecuteManagement()
    {
        switch (managementType)
        {
            case ManagementType.Exchange:
                GameEvents.RaiseExchangePanelRequested();
                break;

            case ManagementType.Runes:
                GameEvents.RaiseRunePanelRequested();
                break;

            case ManagementType.Summon:
                GameEvents.RaiseSummonPanelRequested();
                break;
        }

        HubManager.Instance.UnblockInput();
    }

    // ─────────────────────────────────────────
    // UTILIDADES
    // ─────────────────────────────────────────

    private bool CanInteract()
    {
        if (GameManager.Instance == null) return true;

        if (requiredFlag != KnowledgeFlag.None && !GameManager.Instance.Knowledge.HasFlag(requiredFlag))
            return false;

        if (blockedByFlag != KnowledgeFlag.None && GameManager.Instance.Knowledge.HasFlag(blockedByFlag))
            return false;

        return true;
    }

    //Devuelve el tile adyacente (arriba, abajo, izquierda, derecha) más cercano al player
    private Vector2Int GetClosestAdjacentTile(Vector2Int playerTile)
    {
        Vector2Int worldTile = GetInteractableTile();

        Vector2Int[] adjacents = {
            worldTile + Vector2Int.up,
            worldTile + Vector2Int.down,
            worldTile + Vector2Int.left,
            worldTile + Vector2Int.right
        };

        Vector2Int closest = adjacents[0];
        float minDist = float.MaxValue;

        foreach (Vector2Int tile in adjacents)
        {
            float dist = Vector2Int.Distance(tile, playerTile);
            if (dist < minDist)
            {
                minDist = dist;
                closest = tile;
            }
        }

        return closest;
    }

    // Convierte la posición mundial del interactuable a coordenadas de grilla
    private Vector2Int GetInteractableTile()
    {
        HubRoom room = GetComponentInParent<HubRoom>();
        if (room == null || room.Data == null) return Vector2Int.zero;

        return new Vector2Int(
            Mathf.FloorToInt(transform.position.x - room.Data.gridOrigin.x),
            Mathf.FloorToInt(transform.position.y - room.Data.gridOrigin.y)
        );
    }
}
