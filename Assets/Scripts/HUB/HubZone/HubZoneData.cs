using UnityEngine;

//Scriptable object con todos los datos de una zona del HUB
[CreateAssetMenu(fileName = "HubZoneData", menuName = "Hub/ZoneData")]
public class HubZoneData : ScriptableObject
{
    [Header("Identidad")]
    public string zoneID;
    public string zoneName;
    [TextArea] public string zoneDescription;
    public Sprite zoneSprite;
    public Sprite zoneIcon;

    [Header("Escena")]
    //Nombre exacto de la escena de Unity que se carga al entrar
    public string sceneName;

    [Header("Desbloqueo")]
    //Si es true la zona esta disponible desde el inicio y se ignora unlockFlag
    public bool unlockedByDefault;
    //Flag que desbloquea la zona, None = sin requisito de flag
    public KnowledgeFlag unlockFlag;
    // Mensaje que se muestra cuando el jugador intenta entrar y la zona esta bloqueada
    [TextArea(1, 3)]
    public string lockedMessage;
}
