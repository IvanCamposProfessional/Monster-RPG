using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Enum que almacena Party o Reserve para poder indicar de que tipo es el slot
public enum ExchangeSlotType { Party, Reserve }

[RequireComponent(typeof(CanvasGroup))]
public abstract class ExchangeSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    //Propiedades publicas que ExchangeManager necesita leer
    public ExchangeSlotType SlotType { get; private set; }
    public int SlotIndex { get; private set; }
    public MonsterSaveData SaveData { get; private set; }
    public bool IsEmpty => SaveData == null;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        //Guardamos el componente Canvas Grouo del Slot en el Awake
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // ─────────────────────────────────────────
    // SETUP
    // ─────────────────────────────────────────

    public void Setup(ExchangeSlotType type, int index, MonsterSaveData saveData)
    {
        SlotType = type;
        SlotIndex = index;
        SaveData = saveData;

        //Si el slot contiene un monster se refresca el visual del slot
        if (saveData != null) RefreshVisual();
        //Si no contiene un monster se setea el visual como Emptys
        else SetEmpty();
    }

    //Creamos una funcion para refrescar el visual del slot
    public abstract void RefreshVisual();
    protected abstract void SetEmpty();

    // ─────────────────────────────────────────
    // DRAG & DROP
    // ─────────────────────────────────────────

    public void OnBeginDrag(PointerEventData eventData)
    {
        //No se arrastran slots vacíos ni monsters bloqueados
        if (IsEmpty || SaveData.isLocked) return;

        //Hacemos que el GameObject que estamos arrastrando no bloquee el raycast
        canvasGroup.blocksRaycasts = false;
        //Ponemos el slot con opacidad de 0.45f para dar feedback visual
        canvasGroup.alpha = 0.45f;
        //Informamos al Exchange Manager que hemos empezado a Dragear
        ExchangeManager.Instance.BeginDrag(this, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Comprobacion de seguridad
        if (IsEmpty) return;
        //Hacemos que el exchange manager haga update del drag mientras arrastramos el slot
        ExchangeManager.Instance.UpdateDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Hacemos que vuelva a bloquear el raycast al acabar el drag
        canvasGroup.blocksRaycasts = true;
        //Volvemos a cambiar la opacidad del slot a 1
        canvasGroup.alpha = 1f;
        //Notificamos al Exchange Manager que hemos terminado el drag
        ExchangeManager.Instance.EndDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Informamos al ExchangeManager que hemos dropeado el slot
        ExchangeManager.Instance.HandleDrop(this);
    }

    public void OnDeleteButtonClicked()
    {
        if (!IsEmpty) ExchangeManager.Instance.RequestDelete(this);
    }
}
