using UnityEngine;
using UnityEngine.EventSystems;

//MonoBehaviour que va en cada NPC interactuable del HUB, comprueba si el jugador tiene la flag requerida y muestra u oculta el NPC, al hacer click abre el panel de dialogo
public class NPCInteractable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private NPCData npcData;
    [SerializeField] private NPCDialogueUI dialogueUI;

    // ─────────────────────────────────────────
    // INICIALIZACION
    // ─────────────────────────────────────────

    private void Awake()
    {
        GameEvents.OnFlagGranted += OnFlagGranted;
    }
 
    private void Start()
    {
        RefreshVisibility();
    }

    private void OnDestroy()
    {
        GameEvents.OnFlagGranted -= OnFlagGranted;
    }

    // ─────────────────────────────────────────
    // VISIBILIDAD
    // ─────────────────────────────────────────

    //Muestra u ocula el MPC segun si el jugador cumple el requisito de flag
    private void RefreshVisibility()
    {
        //Comprobacion de seguridad
        if (npcData == null) return;

        //Revisamos si el NPCData tiene flag none o si el Player tiene la flag desbloqueada
        bool visible = npcData.requiredFlag == KnowledgeFlag.None || GameManager.Instance.Knowledge.HasFlag(npcData.requiredFlag);

        //Activamos el NPC segun el resultado de la comprobacion anterior de flags
        gameObject.SetActive(visible);
    }

    private void OnFlagGranted(KnowledgeFlag flag)
    {
        RefreshVisibility();
    }

    // ─────────────────────────────────────────
    // INPUT
    // ─────────────────────────────────────────

    public void OnPointerClick(PointerEventData eventData)
    {
        //Comprobacion de seguridad
        if (dialogueUI == null)
        {
            Debug.LogWarning("NPCInteractable: dialogueUI no asignado en " + npcData.npcId);
            return;
        }

        dialogueUI.Open(npcData);
    }
}
