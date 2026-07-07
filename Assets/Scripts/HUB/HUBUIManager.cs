using TMPro;
using UnityEngine;

//Centraliza todos los paneles UI del HUB.
public class HUBUIManager : MonoBehaviour
{
    public static HUBUIManager Instance { get; private set; }

    [Header("Panel de mensaje")]
    [SerializeField] private GameObject _messagePanel;
    [SerializeField] private TMP_Text _messageText;

    [Header("Diálogo NPC")]
    [SerializeField] private NPCDialogueUI _npcDialogueUI;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        _messagePanel.SetActive(false);
    }

    // ─────────────────────────────────────────
    // MENSAJE
    // ─────────────────────────────────────────

     public void ShowMessage(string message)
    {
        _messageText.text = string.IsNullOrEmpty(message) ? "Necesitas cumplir ciertos requisitos." : message;
        _messagePanel.SetActive(true);
        HubManager.Instance.BlockInput();
    }

    public void CloseMessagePanel()
    {
        _messagePanel.SetActive(false);
        HubManager.Instance.UnblockInput();
    }

    // ─────────────────────────────────────────
    // NPC
    // ─────────────────────────────────────────

    public void OpenNPCDialogue(NPCData npcData)
    {
        if (npcData == null) return;
        HubManager.Instance.BlockInput();
        _npcDialogueUI.Open(npcData);
    }

    //Llamado por NPCDialogueUI al cerrar el panel
    public void OnNPCDialogueClosed()
    {
        HubManager.Instance.UnblockInput();
    }
}
