using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

//Componente que va en cada zona clickable del Hub.
//Requiere un Collider2D para detectar clicks y hover y un SpriteRenderer.
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class HubZone : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    //Información de la zona a la que vamos a acceder
    [SerializeField] private HubZoneData zoneData;

    private SpriteRenderer sr;

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
        //Inicializamos el SpriteRenderer
        sr = GetComponent<SpriteRenderer>();
        //Aplicamos el Sprite al Game Object
        ApplySprite();
        //Aplicamos el Collider a la forma del Sprite
        ApplyCollider();
        //Al empezar la escena lanzamos la funcion Refresh Visual
        RefreshVisual();
    }

    private void OnFlagGranted(KnowledgeFlag flag)
    {
        RefreshVisual();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Si no está desbloqueada la zona
        if (!IsUnlocked())
        {
            //Lanzamos el mensaje en la UI que indica que la zona está bloqueada
            GameEvents.RaiseZoneLockedClicked(zoneData.zoneName, zoneData.lockedMessage);
            return;
        }

        //Si no está bloqueada la zona (no ha entrado en el if anterior y ha hecho return de la funcion) lanzamos la escena de la zona
        SceneManager.LoadScene(zoneData.sceneName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Notificamos que el cursor ha entrado en esta zona
        GameEvents.RaiseZoneHoverEnter(zoneData.zoneName, IsUnlocked());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Notificamos que el cursor ha salido de esta zona
        GameEvents.RaiseZoneHoverExit();
    }

    public bool IsUnlocked()
    {
        //Si la zona está desbloqueada de base devolvemos true
        if(zoneData.unlockedByDefault) return true;

        //Devolvemos true o false segun si el World Knowledge que tenemos contiene la flag necesaria para desbloquear la zona
        return GameManager.Instance.Knowledge.HasFlag(zoneData.unlockFlag); 
    }

    private void ApplySprite()
    {
        //Comprobacion de seguridad
        if(zoneData.zoneSprite != null)
        {
            //Asignamos al sprite del Game Object el Sprite del data
            sr.sprite = zoneData.zoneSprite;
        }
    }

    private void ApplyCollider()
    {
        //Comprobacion de seguridad
        if(sr.sprite == null) return;

        //Guardamos el polygon collider
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        //Comprobacion de seguridad
        if(col == null) return;

        //Obtenemos los paths de la fisica del sprite y los aplicamos al collider
        col.pathCount = sr.sprite.GetPhysicsShapeCount();
        List<Vector2> path = new List<Vector2>();

        for(int i = 0; i < col.pathCount; i++)
        {
            sr.sprite.GetPhysicsShape(i, path);
            col.SetPath(i, path);
        }
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
