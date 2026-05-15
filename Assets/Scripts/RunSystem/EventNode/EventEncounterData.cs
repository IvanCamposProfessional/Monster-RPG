using System;
using System.Collections.Generic;
using UnityEngine;

//Pool de eventos disponibles para un piso concreto
[Serializable]
public class FloorEventPool
{
    //Indice del piso al que corresponde esta Pool, 0 = piso 1
    public int floorIndex;
    public List<EventData> possibleEvents;
}

//ScriptableObject que define todos los eventos de un tipo de run
[CreateAssetMenu(fileName = "EventEncounterData", menuName = "Run/Event Encounter Data")]
public class EventEncounterData : ScriptableObject
{
    [Header("Identidad")]
    public MonsterType themeType;

    [Header("Eventos por piso")]
    public List<FloorEventPool> poolsByFloor;

    // ─────────────────────────────────────────
    // CONSULTAS
    // ─────────────────────────────────────────

    //Devuelve un EventData elegible para el piso indicado usando seleccion ponderada, filtra por requiredFlag y blockedByFlag antes de hacer el roll
    public EventData GetElegibleEvent(int floorIndex, KnowledgeSystem knowledge)
    {
        //Comprobacion de seguridad
        if (poolsByFloor == null) return null;

        //Buscamos la pool del piso actual
        FloorEventPool pool = poolsByFloor.Find(p => p.floorIndex == floorIndex);

        //Comprobacion de seguridad
        if (pool == null || pool.possibleEvents == null || pool.possibleEvents.Count == 0)
        {
            Debug.LogWarning("EventEncounterData: no hay eventos para el piso " + floorIndex + " en tema " + themeType);
            return null;
        }

        //Paso 1: filtrar eventos elegibles segun las flags actuales del jugador
        List<EventData> elegible = new List<EventData>();

        //Recorremos con un bucle los PossibleEvents dentro de la pool del piso
        foreach(EventData ev in pool.possibleEvents)
        {
            //Si tiene requiredFlag el jugador debe tener la flag lo que significa que si HasFlag devuelve False (el jugador no ha desbloqueado la flag)
            //se hace Continue (no se elige el evento)
            if (ev.requiredFlag != KnowledgeFlag.None && !knowledge.HasFlag(ev.requiredFlag))
                continue;

            //Si tiene blockedByFlag el jugador NO debe tenerla lo que significa que si HasFlag devuelve True (el jugador si ha desbloqueado la flag)
            //se hace Continue (no se elige el evento))
            if (ev.blockedByFlag != KnowledgeFlag.None && knowledge.HasFlag(ev.blockedByFlag))
                continue;

            //En caso de haber superado las 2 comprobaciones de flags se añade el evento a la lista de elegibles
            elegible.Add(ev);
        }

        //Comprobacion de seguridad
        if (elegible.Count == 0)
        {
            Debug.LogWarning("EventEncounterData: ningún evento elegible para el piso " + floorIndex + " en tema " + themeType);
            return null;
        }

        //Paso 2: seleccion ponderada de los elegibles
        float total = 0f;

        //Recorremos con un bucle los PossibleEvents dentro de la pool del piso y sumamos el peso de cada evento para sacar el total de los pesos
        foreach(EventData ev in pool.possibleEvents) total += ev.eventWeight;

        //Roleamos un numero random entre 0 y el total de los pesos
        float roll = UnityEngine.Random.Range(0f, total);

        //Creamos una variable para guardar el peso acumulado de los elegible events
        float accumulated = 0f;

        //Recorremos con un bucle los Elegible Events
        foreach (EventData ev in elegible)
        {
            //Sumamos el peso acumulado de los elegible events
            accumulated += ev.eventWeight;
            //Si el peso aleatorio es menor o igual que el acumulado se devuelve el evento
            if (roll <= accumulated) return ev;
        }

        //Fallback: devolver el ultimo elegible
        return elegible[elegible.Count - 1];
    }
}
