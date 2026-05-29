using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExchangeManager : MonoBehaviour
{
    public static ExchangeManager Instance { get; private set; }

    [Header("Panel principal")]
    [SerializeField] private GameObject exchangePanel;

    [Header("Contenedores")]
    [SerializeField] private Transform partySlotsContainer;
    [SerializeField] private Transform reserveSlotsContainer;

    [Header("Prefabs")]
    [SerializeField] private GameObject partySlotPrefab;
    [SerializeField] private GameObject reserveSlotPrefab;

    [Header("Texto contador reserva")]
    [SerializeField] private TMP_Text reserveCountText;

    [Header("Ghost de arrastre")]
    [SerializeField] private RectTransform dragGhost;
    [SerializeField] private Image dragGhostIcon;
    [SerializeField] private RectTransform canvasRect;

    [Header("Confirmación de eliminación")]
    [SerializeField] private GameObject confirmDeletePanel;
    [SerializeField] private TMP_Text confirmDeleteText;

    private List<ExchangeSlot> partySlots = new List<ExchangeSlot>();
    private List<ExchangeSlot> reserveSlots = new List<ExchangeSlot>();

    private ExchangeSlot dragSource;
    private ExchangeSlot pendingDeleteSlot;

    //Variable para saber si el filtro de favorites esta activo
    private bool favoritesFilterActive = false;

    private void Awake()
    {
        //Inicializamos la Instance
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        //Desactivamos los panels y GameObjects que no estamos utilizando
        exchangePanel.SetActive(false);
        confirmDeletePanel.SetActive(false);
        dragGhost.gameObject.SetActive(false);

        //Suscribimos el event OnExchangePanelRequested a la funcion que debe lanzar
        GameEvents.OnExchangePanelRequested += OpenPanel;
    }

    private void OnDestroy()
    {
        GameEvents.OnExchangePanelRequested -= OpenPanel;
    }

    // ─────────────────────────────────────────
    // ABRIR / CERRAR
    // ─────────────────────────────────────────

    public void OpenPanel()
    {
        //Al abrir el panel hacemos build de los slots
        BuildSlots();
        //Activamos el panel de exchange
        exchangePanel.SetActive(true);
        //Hacemos raise del event Panel Toggled
        GameEvents.RaiseExchangePanelToggled(true);
    }

    public void ClosePanel(){
        //Guardamos el juego para que queden guardadas las modificaciones del player en su partida
        GameManager.Instance.SaveGame();
        //Desactivamos el panel
        exchangePanel.SetActive(false);
        //Hacemos raise del event Panel Toggled
        GameEvents.RaiseExchangePanelToggled(false);
    }

    // ─────────────────────────────────────────
    // CONSTRUCCIÓN DE SLOTS
    // ─────────────────────────────────────────

    private void BuildSlots()
    {
        BuildPartySlots();
        BuildReserveSlots();
        RefreshReserveCount();
    }

    private void BuildPartySlots()
    {
        //Creamos un bucle que recorre los party slots
        foreach(ExchangeSlot s in partySlots)
            //Si ya hay algun slot inicializado al construirse lo destruye
            if (s != null) Destroy(s.gameObject);
        //Hacemos clear de la lista de party slots
        partySlots.Clear();

        //Guardamos la info del current player
        PlayerData player = GameManager.Instance.CurrentPlayer;

        //Hacemos un bucle que recorra la active party
        for (int i = 0; i < PlayerData.MAX_ACTIVE_PARTY; i++)
        {
            //Instanciamos el slot y guardamos el script
            ExchangeSlot slot = Instantiate(partySlotPrefab, partySlotsContainer).GetComponent<ExchangeSlot>();
            //Guardamos el SaveData en el slot correspondiente
            MonsterSaveData data = player.activeParty.Find(m => m.slotIndex == i);
            //Hacemos setup del slot indicandole que es de la party, el slot que es dentro de la party y la data del monster que contiene
            slot.Setup(ExchangeSlotType.Party, i, data);
            //Añadimos el slot a la lista de PartySlots
            partySlots.Add(slot);
        }
    }

    private void BuildReserveSlots()
    {
        //Creamos un bucle que recorre los reserve slots
        foreach(ExchangeSlot s in reserveSlots)
            //Si ya hay algun slot inicializado al construirse lo destruye
            if (s != null) Destroy(s.gameObject);
        //Hacemos clear de la lista de reserve slots
        reserveSlots.Clear();

        //Guardamos la info del current player
        PlayerData player = GameManager.Instance.CurrentPlayer;

        //Hacemos un bucle que recorra la reserve capacity del player
        for (int i = 0; i < player.reserveCapacity; i++)
        {
            //Instanciamos el slot y guardamos el script
            ExchangeSlot slot = Instantiate(reserveSlotPrefab, reserveSlotsContainer).GetComponent<ExchangeSlot>();
            //Guardamos el SaveData en el slot correspondiente
            MonsterSaveData data = player.reserve.Find(m => m.slotIndex == i);
            //Hacemos setup del slot indicandole que es de la reserve, el slot que es dentro de la reserve y la data del monster que contiene
            slot.Setup(ExchangeSlotType.Reserve, i, data);
            //Añadimos el slot a la lista de ReserveSlots
            reserveSlots.Add(slot);
        }
    }

    private void RefreshAllSlots()
    {
        //Guardamos la info del current player
        PlayerData player = GameManager.Instance.CurrentPlayer;

        //Creamos un bucle que recorra los party slots
        for (int i = 0; i < partySlots.Count; i++)
        {
            //Guardamos el SaveData en el slot correspondiente
            MonsterSaveData data = player.activeParty.Find(m => m.slotIndex == i);
            //Hacemos setup del slot indicandole que es de la party, el slot que es dentro de la party y la data del monster que contiene
            partySlots[i].Setup(ExchangeSlotType.Party, i, data);
        }

        //Creamos un bucle que recorra los reserve slots
        for (int i = 0; i < reserveSlots.Count; i++)
        {
            //Guardamos el SaveData en el slot correspondiente
             MonsterSaveData data = player.reserve.Find(m => m.slotIndex == i);
            //Hacemos setup del slot indicandole que es de la reserve, el slot que es dentro de la reserve y la data del monster que contiene
            reserveSlots[i].Setup(ExchangeSlotType.Reserve, i, data);
        }

        RefreshReserveCount();
        ApplyFavoriteFilter();
    }

    private void RefreshReserveCount()
    {
        //Comprobacion de seguridad
        if (reserveCountText == null) return;
        //Guardamos el PlayerData
        PlayerData p = GameManager.Instance.CurrentPlayer;
        //Cambiamos el texto para mostrar la reserva que tenemos utilizada y el total
        reserveCountText.text = p.reserve.Count + " / " + p.reserveCapacity;
    }

    // ─────────────────────────────────────────
    // DRAG & DROP
    // ─────────────────────────────────────────

    public void BeginDrag(ExchangeSlot source, PointerEventData eventData)
    {
        //Guardamos el source slot que estamos drageando
        dragSource = source;

        //Guardamos la data del monster que empezamos a hacer drag
        MonsterData data = GameManager.Instance.MonsterDatabase.GetMonsterByID(source.SaveData.monsterID);
        //Si el slot contiene Monster Data cambiamos el icon del Drag Ghost al del monster
        if (data != null) dragGhostIcon.sprite = data.MonsterIcon;

        //Activamos el drag ghost
        dragGhost.gameObject.SetActive(true);
        //Pasamos el event data a Update Drag
        UpdateDrag(eventData);
    }

    public void UpdateDrag(PointerEventData eventData)
    {
        //Actualizamos la posicion del drag ghost calculandolo con el event data
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 pos);
        dragGhost.anchoredPosition = pos;
    }

    public void EndDrag()
    {
        //Desactivamos el drag ghost
        dragGhost.gameObject.SetActive(false);
        //Ponemos a null la drag source
        dragSource = null;
    }

    public void HandleDrop(ExchangeSlot target)
    {
        //Si hemos soltado en un espacio vacio o hemos dejado el slot en la misma posicion hacemos return
        if (dragSource == null || dragSource == target) return;

        //No se puede intercambiar con un monster bloqueado
        if (!target.IsEmpty && target.SaveData.isLocked) return;

         //Ejecutamos el swap or move del slot y si devuelve true, lo que quiere decir que se ha hecho correctamente, refresca los slots
        if(ExecuteSwapOrMove(dragSource, target))
            RefreshAllSlots();
    }

    // ─────────────────────────────────────────
    // LÓGICA DE INTERCAMBIO
    // ─────────────────────────────────────────

    private bool ExecuteSwapOrMove(ExchangeSlot source, ExchangeSlot target)
    {
        //Guardamos la info del current player
        PlayerData player = GameManager.Instance.CurrentPlayer;

        //Guardamos el MonsterSaveData del Source Slot
        MonsterSaveData srcMonster = source.SaveData;
        //Guardamos el MonsterSaveData del Target Slot, null si esta vacio
        MonsterSaveData tgtMonster = target.SaveData;

        //Mismo panel, solo se intercambia slot index
        if(source.SlotType == target.SlotType)
        {
            //Si el slot de target contiene un monster intercambiamos los slot index
            if (tgtMonster != null)
            {
                int temp = srcMonster.slotIndex;
                srcMonster.slotIndex = tgtMonster.slotIndex;
                tgtMonster.slotIndex = temp;
            }
            //Si el slot de target no contiene monster asignamos el slot index del target al source
            else
            {
                srcMonster.slotIndex = target.SlotIndex;
            }
        }
        //Si son paneles distintos
        else
        {
            //Guardamos las listas de source slot monsters  y target slot monsters dependiendo del tipo de slot source y target (si son de party o reserve)
            List<MonsterSaveData> srcList = source.SlotType == ExchangeSlotType.Party ? player.activeParty : player.reserve;
            List<MonsterSaveData> tgtList = target.SlotType == ExchangeSlotType.Party ? player.activeParty : player.reserve;

            //Borramos el monster de la source list
            srcList.Remove(srcMonster);

            //Si el target slot contiene un monster
            if (tgtMonster != null)
            {
                //Eliminamos el monster del target slot de la lista, le asignamos el slot index del source y lo añadimos a la lista de monsters del source
                tgtList.Remove(tgtMonster);
                tgtMonster.slotIndex = source.SlotIndex;
                srcList.Add(tgtMonster);
            }

            //Asignamos el index al source monster del target y lo añadimos a la lista de target
            srcMonster.slotIndex = target.SlotIndex;
            tgtList.Add(srcMonster);
        }

        return true;
    }

    // ─────────────────────────────────────────
    // LOCK Y FAVORITO
    // ─────────────────────────────────────────

    public void ToggleLock(ExchangeSlot slot)
    {
        //Cambiamos el estado a bloqueado o no bloqueado, lo contrario de lo que ya estaba
        slot.SaveData.isLocked = !slot.SaveData.isLocked;
        //Refrescamos el visual del slot
        slot.RefreshVisual();
    }

    public void ToggleFavorite(ExchangeSlot slot)
    {
        //Cambiamos el estado a favorito o no favorito, lo contrario de lo que ya estaba
        slot.SaveData.isFavorite = !slot.SaveData.isFavorite;
        //Refrescamos el visual del slot
        slot.RefreshVisual();
    }

    public void ToggleFavoriteFilter()
    {
        //Cambiamos el booleano que indica si está activo al valor contrario (si es true a false y viceversa)
        favoritesFilterActive = !favoritesFilterActive;
        ApplyFavoriteFilter();
    }

    private void ApplyFavoriteFilter()
    {
        //Creamos un bucle que recorra los slots de la reserve
        foreach (ExchangeSlot slot in reserveSlots)
        {
            //Si el filtro no esta activo se muestran todos los slots
            if (!favoritesFilterActive)
            {
                slot.gameObject.SetActive(true);
            }
            //Si el filtro esta activo
            else
            {
                //Muestra solo los slots ocupados con monster favorito
                bool show = !slot.IsEmpty && slot.SaveData.isFavorite;
                slot.gameObject.SetActive(show);
            }
        }
    }

    // ─────────────────────────────────────────
    // ELIMINACIÓN
    // ─────────────────────────────────────────

    public void RequestDelete(ExchangeSlot slot)
    {
        //Comprobacion de seguridad
        if (slot.IsEmpty || slot.SaveData.isLocked) return;

        //Guardamos el slot que esta pendiente de eliminarse
        pendingDeleteSlot = slot;

        //Guardamos la monster data del monster que queremos eliminar
        MonsterData data = GameManager.Instance.MonsterDatabase.GetMonsterByID(slot.SaveData.monsterID);
        //Guardamos el name del monster
        string name = data != null ? data.MonsterName : "este monster";
        //Mostramos el panel de eliminacion con el texto anterior
        confirmDeleteText.text = "¿Eliminar a " + name + "?\nEsta acción es permanente.";
        confirmDeletePanel.SetActive(true);
    }

    public void ConfirmDelete()
    {
        //Comprobacion de seguridad
        if (pendingDeleteSlot == null) return;

        //Guardamos la info del current player
        PlayerData player = GameManager.Instance.CurrentPlayer;

        //Dependiendo del Exchange Slot Type guardamos la party o la reserve del player
        List<MonsterSaveData> list = pendingDeleteSlot.SlotType == ExchangeSlotType.Party ? player.activeParty : player.reserve;

        //Borramos la monster data de la lista correspondiente
        list.Remove(pendingDeleteSlot.SaveData);

        //Ponemos el slot pendiente de eliminarse a null
        pendingDeleteSlot = null;
        //Desactivamos el panel
        confirmDeletePanel.SetActive(false);
        //Hacemos refresh de todos los slots
        RefreshAllSlots();
    }

    public void CancelDelete()
    {
        //Ponemos el slot pendiente de eliminarse a null
        pendingDeleteSlot = null;
        //Desactivamos el panel
        confirmDeletePanel.SetActive(false);
    }
}
