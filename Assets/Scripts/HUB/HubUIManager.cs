using TMPro;
using UnityEngine;

//Gestiona todos los paneles UI de la HubScene.
public class HubUIManager : MonoBehaviour
{
    //Creamos la Instancia del Manager
    public static HubUIManager Instance { get; private set; }

    [Header("Tooltip de zona")]
    [SerializeField] private GameObject zoneTooltipPanel;
    [SerializeField] private TMP_Text zoneTooltipText;

    [Header("Panel bloqueado")]
    //Se usa tanto para zonas bloqueadas como para edificios bloqueados como para edificios bloqueados
    [SerializeField] private GameObject lockedPanel;
    [SerializeField] private TMP_Text lockedTitleText;
    [SerializeField] private TMP_Text lockedBodyText;

    private void Awake()
    {
        //Comprobacion de seguridad para no duplicar la Instance
        if(Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        /*zoneTooltipPanel.SetActive(false);
        lockedPanel.SetActive(false);*/
    }

    //---- Tooltip ----
    public void ShowZoneTooltip(string zoneName, bool isUnlocked)
    {
        //Cambiamos el texto de la Zone Tooltip segun isUnlocked
        zoneTooltipText.text = isUnlocked ? zoneName : zoneName + " [bloqueada]";
        //Mostramos el panel de Zone Tooltip
        zoneTooltipPanel.SetActive(true);
    }

    public void HideZoneTooltip()
    {
        //Ocultamos el panel de Zone Tooltip
        zoneTooltipPanel.SetActive(false);
    }

    //---- Locked Panel ----
    public void ShowLockedMessage(string title, string body)
    {
        //Cambiamos el texto del titulo del mensaje de locked
        lockedTitleText.text = title;
        //Cambiamos el texto del mensaje de locked, añadimos la comprobacion de Is Null or Empty para no generar errores si el mensaje del body esta vacio
        lockedBodyText.text = string.IsNullOrEmpty(body) ? "Necesitas cumplir ciertos requisitos para acceder." : body;
        //Mostramos el panel
        lockedPanel.SetActive(true);
    }

    public void OnLockedPanelClose()
    {
        //Ocultamos el panel
        lockedPanel.SetActive(false);
    }
}
