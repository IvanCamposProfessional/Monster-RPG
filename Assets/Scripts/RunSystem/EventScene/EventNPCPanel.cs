using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

//Subsistema de la EventEscen para eventos de tipo NPC, muestra el dialogo del evento y al terminar notifica al EventManager
public class EventNPCPanel : MonoBehaviour
{
    [Header("Base de datos")]
    [SerializeField] private NPCDatabase npcDatabase;

    [Header("Referencias UI")]
    [SerializeField] private Image portrait;
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text dialogueText;
    
    //Estado interno
    private EventData eventData;
    private int currentLine;

    private void Update()
    {
        if (!gameObject.activeSelf) return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
            AdvanceLine();
    }

    // ─────────────────────────────────────────
    // SETUP
    // ─────────────────────────────────────────

    //Llamado por EventManager al activar este subsistema
    public void Setup(EventData data)
    {
        eventData = data;
        currentLine = 0;

        //Cargar datos del NPC desde la base de datos
        NPCData npcData = npcDatabase.GetNPCByID(data.npcId);

        if(npcData != null){
            if (portrait != null)    portrait.sprite  = npcData.portrait;
            if (npcNameText != null) npcNameText.text = npcData.npcName;
        }
        else
        {
            Debug.LogWarning("EventNPCPanel: NPCData no encontrado para ID " + data.npcId);
        }

        ShowCurrentLine();
    }

    // ─────────────────────────────────────────
    // PRIVADOS
    // ─────────────────────────────────────────

    private void ShowCurrentLine()
    {
         // Si no hay lineas terminamos directamente
        if (eventData.dialogueLines == null || eventData.dialogueLines.Count == 0)
        {
            OnFinish();
            return;
        }

        dialogueText.text = eventData.dialogueLines[currentLine];
    }

    private void AdvanceLine()
    {
        currentLine++;

        //Si hemos llegado al final notificamos al EventManager
        if (currentLine >= eventData.dialogueLines.Count) { OnFinish(); return; }

        ShowCurrentLine();
    }

    //Notifica al EventManager que el jugador ha terminado el evento
    private void OnFinish()
    {
        GameEvents.RaisePlayerFinishedEvent();
    }
}
