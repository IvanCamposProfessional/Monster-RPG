using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Indica el origen del drag para saber si hay que desequipar al soltar en desbloqueadas
public enum RuneDragSource { Equipped, Unlocked }

//Panel principal de gestion de Essence Runes, gestiona las 4 columnas: Party, Runes del Monster, Runes desbloqueadas y Filtros
public class EssenceRuneManagerUI : MonoBehaviour
{
    public static EssenceRuneManagerUI Instance { get; private set; }
 
    [Header("Panel raiz")]
    [SerializeField] private GameObject runeManagerPanel;
 
    [Header("Columna Party")]
    [SerializeField] private Transform partyContainer;
    [SerializeField] private GameObject runeMonsterCardPrefab;
 
    [Header("Columna Runes del Monster")]
    [SerializeField] private Transform equippedSlotsContainer;
    [SerializeField] private GameObject runeSlotCardPrefab;
 
    [Header("Columna Runes desbloqueadas")]
    [SerializeField] private Transform unlockedRunesContainer;
    [SerializeField] private GameObject runeCardPrefab;
 
    [Header("Filtros")]
    [SerializeField] private Transform filtersContainer;
    [SerializeField] private GameObject filterButtonPrefab;
    [SerializeField] private Toggle equipablesToggle;

    [Header("Panel de error")]
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TMPro.TextMeshProUGUI errorText;

    [Header("Drag ghost")]
    [SerializeField] private RectTransform dragGhost;
    [SerializeField] private Image dragGhostIcon;
    [SerializeField] private RectTransform canvasRect;

    //Monster actualmente seleccionado en la columna Party
    private MonsterSaveData selectedMonster;
    //Rune que se esta arrastrando actualmente
    private string dragRuneID;
    //Origen del drag (para saber si viene de equipada o de desbloqueada)
    private RuneDragSource dragSource;

    //Filtros activos por tipo
    private List<MonsterType> activeTypeFilters = new List<MonsterType>();
    private bool equipablesFilterActive = false;

    //Listas de cards instanciadas
    private List<EssenceRuneMonsterCard> monsterCards = new List<EssenceRuneMonsterCard>();
    private List<EssenceRuneSlotCard> slotCards = new List<EssenceRuneSlotCard>();
    private List<EssenceRuneCard> unlockedCards = new List<EssenceRuneCard>();

    private bool dropHandled = false;

    // ─────────────────────────────────────────
    // CICLO DE VIDA
    // ─────────────────────────────────────────

    private void Awake()
    {
        //Inicializamos la instance
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
 
        //Desactivamos los subpaneles
        runeManagerPanel.SetActive(false);
        errorPanel.SetActive(false);
        dragGhost.gameObject.SetActive(false);
 
        //Publicamos la funcion Open Panel
        GameEvents.OnRunePanelRequested += OpenPanel;
    }

    private void OnDestroy()
    {
        GameEvents.OnRunePanelRequested -= OpenPanel;
    }

    // ─────────────────────────────────────────
    // ABRIR / CERRAR
    // ─────────────────────────────────────────

    public void OpenPanel()
    {
        //Limpiamos las listas necesarias
        selectedMonster = null;
        activeTypeFilters.Clear();
        equipablesFilterActive = false;
 
        //Hacemos las builds de las columnas
        BuildPartyColumn();
        BuildFilterButtons();
        ClearEquippedColumn();
        BuildUnlockedColumn();

        //Reseteamos el toggle de equipables y registramos el listener
        equipablesToggle.isOn = false;
        equipablesToggle.onValueChanged.RemoveAllListeners();
        equipablesToggle.onValueChanged.AddListener(OnEquipablesFilterToggled);
 
        //Activamos el panel de Rune Manager
        runeManagerPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        GameManager.Instance.SaveGame();
        runeManagerPanel.SetActive(false);
    }

    // ─────────────────────────────────────────
    // CONSTRUCCION DE COLUMNAS
    // ─────────────────────────────────────────

    //Funcion que hace build de la columna de la party
    private void BuildPartyColumn()
    {
        //Destruimos los GameObjects de las cards de la party y limpiamos la lista
        foreach (EssenceRuneMonsterCard card in monsterCards)
            if (card != null) Destroy(card.gameObject);
        monsterCards.Clear();

        //Guardamos el Current Player
        PlayerData player = GameManager.Instance.CurrentPlayer;

        //Creamos un bucle que recorra la Party del Player
        foreach (MonsterSaveData monsterSave in player.activeParty)
        {
            //Instanciamos el Rune Monster Card Prefab
            EssenceRuneMonsterCard card = Instantiate(runeMonsterCardPrefab, partyContainer).GetComponent<EssenceRuneMonsterCard>();
            //Guardamos el Monster Data
            MonsterData data = GameManager.Instance.MonsterDatabase.GetMonsterByID(monsterSave.monsterID);
            //Hacemos Setup de la card
            card.Setup(monsterSave, data, OnMonsterSelected);
            //Añadimos la card a la lista de Monster Cards
            monsterCards.Add(card);
        }
    }

    //Funcion que hace build de la columna de las equiped runes
    private void BuildEquippedColumn()
    {
        ClearEquippedColumn();

        if (selectedMonster == null) return;

        //Instanciamos los slots ocupados
        //Creamos un bucle que recorre los slots de las runes del Monster
        for (int i = 0; i < EssenceRuneSystem.MAX_RUNE_SLOTS; i++)
        {
            //Instanciamos la Rune Slot Card
            EssenceRuneSlotCard slot = Instantiate(runeSlotCardPrefab, equippedSlotsContainer).GetComponent<EssenceRuneSlotCard>();
            //Guardamos la Rune ID
            string runeID = i < selectedMonster.equippedRuneIDs.Count ? selectedMonster.equippedRuneIDs[i] : null;
            //Guardamos la Essence Rune del Slot
            EssenceRune rune = runeID != null ? GameManager.Instance.EssenceRuneDatabase.GetRuneByID(runeID) : null;
            //Hacemos setup del Rune Slot Card
            slot.Setup(i, rune, selectedMonster);
            //Añadimos el GameObject a la lista de slots de rune
            slotCards.Add(slot);
        }
    }

    //Funcion que limpia la columna de las equiped runes, la necesitamos ya que se llama cuando no hay monster seleccionado
    private void ClearEquippedColumn()
    {
        //Destruimos los GameObjects de las cards de las Essence Rune y limpiamos la lista
        foreach (EssenceRuneSlotCard slot in slotCards)
            if (slot != null) Destroy(slot.gameObject);
        slotCards.Clear();
    }

    //Funcion que hace build de la columna de unlocked runes
    private void BuildUnlockedColumn()
    {
        //Destruimos los GameObjects de las cards de las Essence Rune y limpiamos la lista
        foreach (EssenceRuneCard card in unlockedCards)
            if (card != null) Destroy(card.gameObject);
        unlockedCards.Clear();

        //Guardamos las EssenceRunes unlocked del Player
        List<EssenceRune> runes = GameManager.Instance.Runes.GetUnlockedRunesSorted();

        //Creamos un bucle que recorra las Essence Runes desbloqueadas
        foreach (EssenceRune rune in runes)
        {
            //Aplicamos filtros
            if (!PassesFilters(rune)) continue;

            //Instanciamos las cards
            EssenceRuneCard card = Instantiate(runeCardPrefab, unlockedRunesContainer).GetComponent<EssenceRuneCard>();

            //Consultamos y guardamos el state de la card
            EssenceRuneCardState state = GameManager.Instance.Runes.GetRuneCardState(rune.RuneID, selectedMonster);
            Debug.Log("BuildUnlockedColumn — runeID: " + rune.RuneID + " | state: " + state);
            //Hacemos Setup de la rune card
            card.Setup(rune, state, selectedMonster);
            //Añadimos el Game Object a la lista de unlocked rune cards
            unlockedCards.Add(card);
        }
    }

    private void BuildFilterButtons()
    {
        //Destruimos los GameObjects de los filtros
        foreach (Transform child in filtersContainer)
            Destroy(child.gameObject);

        //Recogemos todos los tipos presentes en las Runes desbloqueadas
        HashSet<MonsterType> types = new HashSet<MonsterType>();

        //Guardamos las EssenceRunes unlocked del Player
        List<EssenceRune> runes = GameManager.Instance.Runes.GetUnlockedRunesSorted();

        //Creamos un bucle que recorra las Essence Runes desbloqueadas
        foreach (EssenceRune rune in runes)
        {
            //Comprobacion de seguridad
            if (rune.MoveData == null || rune.MoveData.EssenceAmountToUse == null) continue;
            //Añadimos solo el Main Type para los filtros
            types.Add(rune.MainType);
        }

        //Creamos un bucle que recorra los types guardados anteriormente
        foreach (MonsterType type in types)
        {
            //Instanciamos el Button del filtro
            GameObject btn = Instantiate(filterButtonPrefab, filtersContainer);
            //Hacemos setup del button
            EssenceRuneFilterButton filterBtn = btn.GetComponent<EssenceRuneFilterButton>();
            filterBtn.Setup(type, OnTypeFilterToggled);
        }
    }

    // ─────────────────────────────────────────
    // SELECCION DE MONSTER
    // ─────────────────────────────────────────

    //Funcion que se lanza cuando seleccionamos un mosnter de la party
    private void OnMonsterSelected(MonsterSaveData monster)
    {
        //Guardamos el seelcted monster
        selectedMonster = monster;

        //Refrescamos el estado visual de las cards de party
        foreach (EssenceRuneMonsterCard card in monsterCards)
            card.RefreshSelected(selectedMonster);
 
        BuildEquippedColumn();
        BuildUnlockedColumn();
    }

    // ─────────────────────────────────────────
    // FILTROS
    // ─────────────────────────────────────────

    //Funcion que se lanza cuando seleccionamos un filtro de type
    private void OnTypeFilterToggled(MonsterType type, bool active)
    {
        //Si activamos el filtro lo añadimos a la lista de filtros activo y si lo desactivamos lo eliminamos de la lista
        if (active)
            activeTypeFilters.Add(type);
        else
            activeTypeFilters.Remove(type);

        BuildUnlockedColumn();
    }

    //Funcion que se lanza cuando seleccionamos el filtro de Equipables
    private void OnEquipablesFilterToggled(bool active)
    {
        //Guardamos si el filtro esta activado o desactivado
        equipablesFilterActive = active;
        BuildUnlockedColumn();
    }

    //Funcion que decide si una Essence Rune pasa un filtrado
    private bool PassesFilters(EssenceRune rune)
    {
        //Filtro de tipos
        //Si hay algun filtro de tipos activos
        if (activeTypeFilters.Count > 0)
        {
            //Si el tipo principal de la Rune no está en los filtros activos no pasa el filtro
            if (!activeTypeFilters.Contains(rune.MainType)) return false;
        }

        //Filtro de equipables
        //Si esta el filtro de equipables activo y hay un monster seleccionado
        if (equipablesFilterActive)
        {
            //Comrpueba si hay monster seleccionado o si se puede equipar y si no se puede devuelve false
            if (selectedMonster == null) return false;
            if (!GameManager.Instance.Runes.CanEquip(rune.RuneID, selectedMonster, out _))
                return false;
        }
 
        //Si pasa todos los filtros devuelve true
        return true;
    }

    // ─────────────────────────────────────────
    // DRAG AND DROP
    // ─────────────────────────────────────────

    public void BeginDrag(string runeID, Sprite icon, RuneDragSource source, UnityEngine.EventSystems.PointerEventData eventData)
    {
        //Guardamos la Rune ID de la Rune que estamos drageando
        dragRuneID = runeID;
        //Guardamos la source de la rune que estamos drageando (Equipped o Unlocked)
        dragSource = source;
        //Modificamos el sprite del ghost del drag
        dragGhostIcon.sprite = icon;
        //Activamos el drag ghost
        dragGhost.gameObject.SetActive(true);
        UpdateDrag(eventData);
    }

    public void UpdateDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        //Actualiza la posicion del ghost conforme la del raton
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 pos);
        dragGhost.anchoredPosition = pos;
    }

    public void EndDrag()
    {
        //Desactivamos el drag ghost
        dragGhost.gameObject.SetActive(false);
        //Limpiamos la rune que estabamos drageando
        dragRuneID = null;
        // Marcamos el drop como no procesado al acabar el drag
        dropHandled = false;
    }

    // Drop en un slot concreto → equipar en ese slot
    public void HandleDropOnSlot(MonsterSaveData targetMonster, int slotIndex)
    {
        if (string.IsNullOrEmpty(dragRuneID) || dropHandled) return;
        dropHandled = true;

        bool success = GameManager.Instance.Runes.EquipRune(dragRuneID, targetMonster, slotIndex, out string reason);

        if (!success)
            ShowError(reason);
        else
            RefreshAfterChange();
    }

    //Drop en un slot equipado o en el Monster card -> Equipar
    public void HandleDropOnMonster(MonsterSaveData targetMonster)
    {
        //Si no hay Rune siendo arrastrada o el drop ya fue procesado, ignoramos
        if (string.IsNullOrEmpty(dragRuneID)  || dropHandled) return;

        //Marcamos el drop como procesado para evitar llamadas duplicadas
        dropHandled = true;

        //Guardamos el primer slot libre en la lista de runes del Monster
        int firstFreeSlot = targetMonster.equippedRuneIDs.FindIndex(id => string.IsNullOrEmpty(id));
 
        //Guardamos si ha sido success el equip rune
        bool success = GameManager.Instance.Runes.EquipRune(dragRuneID, targetMonster, firstFreeSlot, out string reason);

        if (!success)
            ShowError(reason);
        else
            RefreshAfterChange();
    }

    //Drop en la columna de desbloqueadas o fuera de cualquier target -> Desequipar
    public void HandleDropOnUnlocked()
    {
        //Si no hay Rune siendo arrastrada, no viene de un slot equipado o el drop ya fue procesado, ignoramos
        if (string.IsNullOrEmpty(dragRuneID) || dragSource != RuneDragSource.Equipped  || dropHandled) return;

        //Marcamos el drop como procesado para evitar llamadas duplicadas
        dropHandled = true;
 
        //Guardamos el owner de la rune
        MonsterSaveData owner = GameManager.Instance.Runes.FindRuneOwner(dragRuneID);
        //Comprobacion de seguridad
        if (owner != null)
        {
            //Desequipamos la rune y refrescamos las columnas
            GameManager.Instance.Runes.UnequipRune(dragRuneID, owner);
            RefreshAfterChange();
        }
    }

    // ─────────────────────────────────────────
    // REFRESCO
    // ─────────────────────────────────────────

    private void RefreshAfterChange()
    {
        //Lanzamos coroutine ya que al reconstruir los slots no se ejecuta correctamente OnEndDrag
         StartCoroutine(RefreshNextFrame());
    }

    private IEnumerator RefreshNextFrame()
    {
        yield return null;
        BuildEquippedColumn();
        BuildUnlockedColumn();
    }

    // ─────────────────────────────────────────
    // ERROR
    // ─────────────────────────────────────────

    public void ShowError(string message)
    {
        errorText.text = message;
        errorPanel.SetActive(true);
    }
 
    public void CloseError()
    {
        errorPanel.SetActive(false);
    }
}
