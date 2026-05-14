using UnityEngine;
using UnityEngine.SceneManagement;

//Singleton de la EventScene
public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [Header("Nombre de escena")]
    [SerializeField] private string runScene = "RunScene";

    [Header("Subsistema de evento")]
    //Panel para eventos de tipo NPC
    [SerializeField] private EventNPCPanel npcPanel;

    //Evento activo
    private EventData currentEvent;

    // ─────────────────────────────────────────
    // INICIALIZACION
    // ─────────────────────────────────────────

    private void Awake()
    {
        //Codigo de seguridad por si hemos duplicado la instancia
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        // Comprobacion de seguridad
        if (!RunEventContext.IsSet)
        {
            Debug.LogWarning("EventManager: no hay RunEventContext activo — la escena se cargo sin contexto");
            return;
        }

        //Leemos el contexto y lo limpiamos
        currentEvent = RunEventContext.SelectedEvent;
        RunEventContext.Clear();

        ActivateSubsystems();
    }

    // ─────────────────────────────────────────
    // SUBSISTEMAS
    // ─────────────────────────────────────────

    //Desactiva todos los paneles y activa el que corresponde al tipo de evento actual
    private void ActivateSubsystems()
    {
        //Desactivar todos los subsistemas antes de activar el correcto
        if(npcPanel != null) npcPanel.gameObject.SetActive(false);

        switch (currentEvent.eventType)
        {
            case EventType.NPC:
                if(npcPanel != null)
                {
                    npcPanel.gameObject.SetActive(true);
                    npcPanel.Setup(currentEvent);
                }
                else
                {
                    Debug.LogWarning("EventManager: npcPanel no asignado en el inspector");
                }
                break;
            case EventType.Item:
            case EventType.Lore:
            case EventType.Minigame:
                Debug.Log("EventManager: tipo de evento pendiente de implementar — " + currentEvent.eventType);
                break;
            default:
                Debug.LogWarning("EventManager: tipo de evento desconocido — " + currentEvent.eventType);
                break;
        }
    }

    // ─────────────────────────────────────────
    // COMPLETAR EVENTO
    // ─────────────────────────────────────────

    //Llamado por el subsistema activo cuando el jugador termina el evento
    public void OnEventCompleted()
    {
        // Otorgar flag si el evento la concede
        if (currentEvent.flagToGrant != KnowledgeFlag.None)
            GameManager.Instance.Knowledge.AddFlag(currentEvent.flagToGrant);
        
        //Otorga item si el evento lo concede
        if(currentEvent.itemReward != null)
            GameManager.Instance.Inventory.AddItem(currentEvent.itemReward.ItemID, currentEvent.itemRewardQuantity);

         //Volver a la run
        SceneManager.LoadScene(runScene);
    }
}
