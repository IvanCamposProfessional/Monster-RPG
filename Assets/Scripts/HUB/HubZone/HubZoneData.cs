using UnityEngine;

//Scriptable object con todos los datos de una zona del HUB
[CreateAssetMenu(fileName = "HubZoneData", menuName = "Hub/ZoneData")]
public class HubZoneData : ScriptableObject
{
    [Header("Identidad")]
    public string zoneID;
    public string zoneName;
    [TextArea] public string zoneDescription;
    public Sprite zoneIcon;

    [Header("Escena")]
    //Nombre exacto de la escena de Unity que se carga al entrar
    public string sceneName;

    [Header("Desbloqueo")]
    //Si es true la zona esta disponible desde el inicio y se ignora unlockFlag
    public bool unlockedByDefault;
    //Flag de KnowledgeFlags.World que desbloquea esta zona
    public string unlockFlag;
    //Mensaje que se muestra cuando cuando el jugador intenta entrar y la zona esta bloqueada
    [TextArea(1, 3)]
    public string lockedMessage;
}
