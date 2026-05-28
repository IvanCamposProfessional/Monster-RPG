using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Enum que almacena Party o Reserve para poder indicar de que tipo es el slot
public enum ExchangeSlotType { Party, Reserve }

[RequireComponent(typeof(CanvasGroup))]
public class ExchangeSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Configuración")]
    [SerializeField] private bool isPartySlot;

    [Header("Visual compartido")]
    [SerializeField] private Image monsterIconImage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject emptyVisual;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private GameObject favoriteActiveIcon;

    [Header("Solo party")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text hpText;

    [Header("Botones")]
    [SerializeField] private GameObject deleteButton;

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
    public void RefreshVisual()
    {
        //Comprobacion de seguridad
        if (SaveData == null) { SetEmpty(); return; }

        //Guardamos la data del monster en una variable buscando el ID del Monster del Save Data que se le ha pasado al slot
        MonsterData data = GameManager.Instance.MonsterDatabase.GetMonsterByID(SaveData.monsterID);
        //Comprobacion de seguridad
        if (data == null) return;

        //Desactivamos el visual que indica que el slot esta empty
        emptyVisual.SetActive(false);
        //Activamos la image y le ponemos el monster icon
        monsterIconImage.gameObject.SetActive(true);
        monsterIconImage.sprite = data.MonsterIcon;
        //Ponemos el texto del level
        levelText.text = "LvL." + SaveData.level;

        //Si el slot contiene un monstruo de la party activa
        if (isPartySlot)
        {
            //Muestra el nombre
            nameText.text = data.MonsterName;
            //Muestra el current HP y el max HP
            hpText.text = SaveData.currentHP + " / " + SaveData.maxHP;
        }

        //Activa el Locked Overlay dependiendo si el Monster esta locked o no
        lockedOverlay.SetActive(SaveData.isLocked);
        //Activa el Delete Button si el Monster no esta locked
        deleteButton.SetActive(!SaveData.isLocked);
        //Activa el icono de favorito leyendo si es favorito del SaveData
        favoriteActiveIcon.SetActive(SaveData.isFavorite);
    }

    //Creamos una funcion para inicializar el visual del slot vacio
    public void SetEmpty()
    {
        SaveData = null;
        emptyVisual.SetActive(true);
        monsterIconImage.gameObject.SetActive(false);
        levelText.text = "";
        lockedOverlay.SetActive(false);
        favoriteActiveIcon.SetActive(false);
        deleteButton.SetActive(false);

        if (isPartySlot) { nameText.text = ""; hpText.text = ""; }
    }

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

    // ─────────────────────────────────────────
    // BOTONES (asignados en el Inspector via OnClick)
    // ─────────────────────────────────────────

    public void OnLockButtonClicked()
    {
        //Si el slot no esta empty notificamos al ExchangeManager que hemos hecho lock del slot
        if (!IsEmpty) ExchangeManager.Instance.ToggleLock(this);
    }

    public void OnFavoriteButtonClicked()
    {
        //Si el slot no esta empty notificamos al ExchangeManager que hemos hecho favorite del slot
        if (!IsEmpty) ExchangeManager.Instance.ToggleFavorite(this);
    }

    public void OnDeleteButtonClicked()
    {
        //Si el slot no esta empty notificamos al ExchangeManager que hemos hecho delete del slot
        if (!IsEmpty) ExchangeManager.Instance.RequestDelete(this);
    }
}
