using System;
using UnityEngine;

//Clase estatica que centraliza todos los eventos globales del juego (patro Publish / Subscribe)
//Los productores invocan eventos sin saber quieles los escuchan
//Los consumidores se suscriben sin saber quienes los producen
public static class GameEvents
{
    // ─────────────────────────────────────────
    // EVENTOS DE COMBATE — MENU
    // ─────────────────────────────────────────
 
    //Se invoca cuando el combate empieza
    public static event Action OnCombatStarted;
    public static void RaiseCombatStarted() => OnCombatStarted?.Invoke();
 
    //Se invoca cuando comienza el turno de una unidad aliada
    public static event Action<MonsterUnit> OnPlayerTurnStarted;
    public static void RaisePlayerTurnStarted(MonsterUnit unit) => OnPlayerTurnStarted?.Invoke(unit);
 
    //Se invoca cuando termina el turno de una unidad aliada
    public static event Action OnPlayerTurnEnded;
    public static void RaisePlayerTurnEnded() => OnPlayerTurnEnded?.Invoke();
 
    //Se invoca cuando el jugador selecciona un movimiento en el menu de combate
    public static event Action<MoveData> OnMoveChosen;
    public static void RaiseMoveChosen(MoveData move) => OnMoveChosen?.Invoke(move);

    // ─────────────────────────────────────────
    // EVENTOS DE COMBATE — ESSENCE
    // ─────────────────────────────────────────

    //Se invoca cuando cambia el estado de una Essence Pool, el bool indica si la pool es aliada (true) o enemiga (false)
    public static event Action<bool> OnEssencePoolChanged;
    public static void RaiseEssencePoolChanged(bool isAlly) => OnEssencePoolChanged?.Invoke(isAlly);
 
    // ─────────────────────────────────────────
    // EVENTOS DE COMBATE — UNIDADES
    // ─────────────────────────────────────────
 
    //Se invoca cuando el cursor entra en una unidad en combate
    public static event Action<Monster> OnUnitHoverEnter;
    public static void RaiseUnitHoverEnter(Monster monster) => OnUnitHoverEnter?.Invoke(monster);
 
    //Se invoca cuando el cursor sale de una unidad en combate
    public static event Action OnUnitHoverExit;
    public static void RaiseUnitHoverExit() => OnUnitHoverExit?.Invoke();
 
    //Se invoca cuando el jugador hace click en una unidad en combate
    public static event Action<MonsterUnit> OnUnitClicked;
    public static void RaiseUnitClicked(MonsterUnit unit) => OnUnitClicked?.Invoke(unit);
 
    //Se invoca cuando el estado de un Monster cambia
    public static event Action<Monster> OnMonsterStateChanged;
    public static void RaiseMonsterStateChanged(Monster monster) => OnMonsterStateChanged?.Invoke(monster);
 
    // ─────────────────────────────────────────
    // EVENTOS DE COMBATE — TIMELINE
    // ─────────────────────────────────────────
 
    //Se invoca cuando un efecto modifica el progreso de la timeline y necesita refrescar los iconos
    public static event Action OnTimelineNeedsRefresh;
    public static void RaiseTimelineNeedsRefresh() => OnTimelineNeedsRefresh?.Invoke();
 
    // ─────────────────────────────────────────
    // EVENTOS DE SUMMON
    // ─────────────────────────────────────────
 
    //Se invoca cuando el jugador interactua con el objeto de invocacion y quiere abrir el panel
    public static event Action OnSummonPanelRequested;
    public static void RaiseSummonPanelRequested() => OnSummonPanelRequested?.Invoke();
 
    //Se invoca tras intentar una invocacion: true si tuvo exito, string con el nombre del monstruo
    public static event Action<bool, string> OnSummonAttempted;
    public static void RaiseSummonAttempted(bool success, string monsterName) => OnSummonAttempted?.Invoke(success, monsterName);
 
    // ─────────────────────────────────────────
    // EVENTOS DE HUB — ZONAS
    // ─────────────────────────────────────────
 
    //Se invoca cuando el cursor entra en una zona del hub: nombre de zona y si esta desbloqueada
    public static event Action<string, bool> OnZoneHoverEnter;
    public static void RaiseZoneHoverEnter(string zoneName, bool isUnlocked) => OnZoneHoverEnter?.Invoke(zoneName, isUnlocked);
 
    //Se invoca cuando el cursor sale de una zona del hub
    public static event Action OnZoneHoverExit;
    public static void RaiseZoneHoverExit() => OnZoneHoverExit?.Invoke();
 
    //Se invoca cuando el jugador hace click en una zona bloqueada: titulo y mensaje
    public static event Action<string, string> OnZoneLockedClicked;
    public static void RaiseZoneLockedClicked(string title, string body) => OnZoneLockedClicked?.Invoke(title, body);
 
    // ─────────────────────────────────────────
    // EVENTOS DE RUN EVENT
    // ─────────────────────────────────────────
 
    //Se invoca cuando un evento otorga una flag de progreso del jugador
    public static event Action<KnowledgeFlag> OnFlagGranted;
    public static void RaiseFlagGranted(KnowledgeFlag flag) => OnFlagGranted?.Invoke(flag);
 
    //Se invoca cuando un evento otorga un item al jugador
    public static event Action<string, int> OnItemGranted;
    public static void RaiseItemGranted(string itemId, int quantity) => OnItemGranted?.Invoke(itemId, quantity);
 
    //Se invoca cuando el jugador completa la interaccion con un subsistema de evento
    public static event Action OnPlayerFinishedEvent;
    public static void RaisePlayerFinishedEvent() => OnPlayerFinishedEvent?.Invoke();

    // ─────────────────────────────────────────
    // EVENTOS DE EXCHANGE (GESTIÓN DE PARTY Y RESERVA)
    // ─────────────────────────────────────────

    public static event Action OnExchangePanelRequested;
    public static void RaiseExchangePanelRequested() => OnExchangePanelRequested?.Invoke();

    //Se invoca cuando el jugador abre/cierra el panel de gestión, true = abierto
    public static event Action<bool> OnExchangePanelToggled;
    public static void RaiseExchangePanelToggled(bool isOpen) => OnExchangePanelToggled?.Invoke(isOpen);
}
