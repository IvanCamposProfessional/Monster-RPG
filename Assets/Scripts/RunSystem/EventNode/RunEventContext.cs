
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

    //Resultado del evento: true si el jugador lo completó
    public static bool EventCompleted { get; private set; }
    public static bool HasResult { get; private set; }
    //Posicion Y de la camara al salir de RunScene, para restaurarla al volver
    public static float CameraY { get; private set; }

    //Escribe el contexto antes de cargar la EventScene
    public static void Set(EventData selectedEvent, string nodeId, float cameraY)
    {
        SelectedEvent = selectedEvent;
        NodeId = nodeId;
        IsSet = true;
        CameraY = cameraY;
    }

    //Limpia el contexto una vez el EventManager lo ha leido
    public static void Clear()
    {
        SelectedEvent = null;
        NodeId        = null;
        IsSet         = false;
    }

    //Escribe el resultado antes de volver a RunScene
    public static void SetResult(bool completed)
    {
        EventCompleted = completed;
        HasResult = true;
    }

    // Limpia el resultado una vez RunManager lo ha procesado
    public static void ClearResult()
    {
        HasResult = false;
    }
}
