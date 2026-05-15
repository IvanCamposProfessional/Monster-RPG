
using UnityEngine;

//Clase estatica que transporta el contexto del evento entre la RunScene y la RunEventScene
public static class RunEventContext
{
    //Evento seleccionado por el RunManager para este nodo
    public static EventData SelectedEvent { get; private set; }

    //ID del nodo de origen para notificar al RunManager al volver
    public static string NodeId { get; private set; }

    //True si hay contexto activo pendiente de ser leido
    public static bool IsSet { get; private set; }

    //Escribe el contexto antes de cargar la EventScene
    public static void Set(EventData selectedEvent, string nodeId)
    {
        SelectedEvent = selectedEvent;
        NodeId = nodeId;
        IsSet = true;
    }

    //Limpia el contexto una vez el EventManager lo ha leido
    public static void Clear()
    {
        SelectedEvent = null;
        NodeId        = null;
        IsSet         = false;
    }
}
