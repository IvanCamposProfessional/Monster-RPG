using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Slot de Rune equipada en la columna Runes del Monster
[RequireComponent(typeof(CanvasGroup))]
public class EssenceRuneSlotCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private GameObject occupiedState;
    [SerializeField] private GameObject emptyState;
    [SerializeField] private Image runeIcon;
    [SerializeField] private TextMeshProUGUI runeNameText;
    [SerializeField] private Transform typeIconsContainer;
    [SerializeField] private GameObject typeIconPrefab;

    private CanvasGroup canvasGroup;
 
    private EssenceRune currentRune;
    private MonsterSaveData ownerMonster;

    private int slotIndex;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // ─────────────────────────────────────────
    // SETUP
    // ─────────────────────────────────────────

    public void Setup(int index, EssenceRune rune, MonsterSaveData owner)
    {
        //Guardamos el SlotIndex, la currentRune y el Owner
        slotIndex = index;
        currentRune = rune;
        ownerMonster = owner;
 
        //Guardamos si es Empty
        bool isEmpty = rune == null;
        //Activamos el occupied state o empty state segun is Empty
        occupiedState.SetActive(!isEmpty);
        emptyState.SetActive(isEmpty);
 
        //Si no esta empty asignamos la UI del slot
        if (!isEmpty)
        {
            runeIcon.sprite = rune.RuneIcon;
            runeNameText.text = rune.MoveData != null ? rune.MoveData.MoveName : rune.RuneID;
            BuildTypeIcons(rune);
        }
    }

    private void BuildTypeIcons(EssenceRune rune)
    {
        //Destruimos todos los TypeIcon anteriores
        foreach (Transform child in typeIconsContainer)
            Destroy(child.gameObject);
 
        //Comrpbacion de seguridad
        if (rune.MoveData == null || rune.MoveData.EssenceAmountToUse == null) return;
 
        //Creamos un bucle que recorre la essence amount to use del move data de la rune
        foreach (EssenceAmount ea in rune.MoveData.EssenceAmountToUse)
        {
            //Instanciamos el icon del type
            GameObject icon = Instantiate(typeIconPrefab, typeIconsContainer);
            //Asignamos el sprite del tipo via TypeIconDatabase
            Sprite typeSprite = GameManager.Instance.TypeIconDatabase.GetIconByType(ea.Type);
            if (typeSprite != null)
                icon.GetComponent<Image>().sprite = typeSprite;
        }
    }

    // ─────────────────────────────────────────
    // DRAG (source — solo si tiene Rune)
    // ─────────────────────────────────────────

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentRune == null) return;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.45f;
        //Informamos al Manager UI de que ha comenzado el drag
        EssenceRuneManagerUI.Instance.BeginDrag(currentRune.RuneID, currentRune.RuneIcon, RuneDragSource.Equipped, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentRune == null) return;
        //Informamos al Manager UI que se esta updateando el drag
        EssenceRuneManagerUI.Instance.UpdateDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        GameObject hit = eventData.pointerCurrentRaycast.gameObject;
        bool droppedOnValidTarget = hit != null && hit.GetComponent<IDropHandler>() != null;

        if (!droppedOnValidTarget)
            EssenceRuneManagerUI.Instance.HandleDropOnUnlocked();

        //Informamos al Manager UI que ha acabado el drag
        EssenceRuneManagerUI.Instance.EndDrag();
    }

    // ─────────────────────────────────────────
    // DROP (target — equipa en este slot si tiene Monster)
    // ─────────────────────────────────────────

    public void OnDrop(PointerEventData eventData)
    {
        eventData.Use();
        if (ownerMonster != null)
            //Informamos al Manager UI que se ha hecho el drop
            EssenceRuneManagerUI.Instance.HandleDropOnSlot(ownerMonster, slotIndex);
    }
}
