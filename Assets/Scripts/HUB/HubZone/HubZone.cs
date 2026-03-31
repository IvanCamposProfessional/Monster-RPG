using UnityEngine;
using UnityEngine.SceneManagement;

//Componente que va en cada zona clickable del Hub.
//Requiere un Collider2D para detectar clicks y hover.
[RequireComponent(typeof(Collider2D))]
public class HubZone : MonoBehaviour
{
    //Información de la zona a la que vamos a acceder
    [SerializeField] private HubZoneData zoneData;

    private void Start()
    {
        //Al empezar la escena lanzamos la funcion Refresh Visual
        RefreshVisual();
    }

    private void OnMouseDown()
    {
        //Si no está desbloqueada la zona
        if (!IsUnlocked())
        {
            //Lanzamos el mensaje en la UI que indica que la zona está bloqueada
            HubUIManager.Instance.ShowLockedMessage(zoneData.zoneName, zoneData.lockedMessage);
            return;
        }

        //Si no está bloqueada la zona (no ha entrado en el if anterior y ha hecho return de la funcion) lanzamos la escena de la zona
        SceneManager.LoadScene(zoneData.sceneName);
    }

    private void OnMouseEnter()
    {
        //Mostramos el Tooltip de la zona desde Hub UI Manager
        HubUIManager.Instance.ShowZoneTooltip(zoneData.zoneName, IsUnlocked());
    }

    private void OnMouseExit()
    {
        //Ocultamos el Tooltip de la zona desde Hub UI Manager
        HubUIManager.Instance.HideZoneTooltip();
    }

    public bool IsUnlocked()
    {
        //Si la zona está desbloqueada de base devolvemos true
        if(zoneData.unlockedByDefault) return true;

        //Devolvemos true o false segun si el World Knowledge que tenemos contiene la flag necesaria para desbloquear la zona
        return GameManager.Instance.Knowledge.HasWorldKnowledge(zoneData.unlockFlag); 
    }

    //Oscurece el sprite si la zona esta bloqueada (se puede reemplazar con animacion a futuro)
    private void RefreshVisual()
    {
        //Guardamos el sprite renderer del game object de la zona
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        //Comprobación de seguridad
        if(sr == null) return;
        //Si la zona esta desbloqueada el sprite se queda en blanco y si no está desbloqueada se oscurece
        sr.color = IsUnlocked() ? Color.white : new Color(0.35f, 0.35f, 0.35f, 1f);
    }

    //Creamos una variable publica para el HubZoneData en la que guardamos el Scriptable Object
    public HubZoneData Data => zoneData;
}
