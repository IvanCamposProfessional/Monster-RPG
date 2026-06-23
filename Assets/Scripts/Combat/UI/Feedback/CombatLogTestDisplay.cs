using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
//Script TEMPORAL de testing para verificar que los mensajes de OnCombatLogMessage llegan correctamente y en orden.
//No es el Combat Feedback Panel final (eso es la Fase 3: scroll, estilos por CombatLogType, etc.)
//Muestra un mensaje a la vez, con clear entre cada uno, para poder leerlos con calma mientras se prueba el cableado de eventos.
//Cuando se monte el panel real con scroll, este comportamiento de clear desaparece y vuelve a ser una lista acumulada.
public class CombatLogTestDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private float delayBetweenMessages = 0.5f;
 
    //Cola de mensajes pendientes de mostrar, el evento puede llegar mas rapido de lo que tardamos en mostrarlos
    private Queue<(string message, CombatLogType type)> messageQueue = new Queue<(string, CombatLogType)>();
    //Referencia a la coroutine de consumo, para no lanzar mas de una a la vez
    private Coroutine displayRoutine;
 
    //Nos suscribimos en Awake porque este objeto no se activa/desactiva durante el combate (sigue el mismo criterio
    //ya usado en el proyecto: Awake/OnDestroy para suscripciones, OnEnable/OnDisable solo si el objeto se activa/desactiva en gameplay)
    private void Awake()
    {
        GameEvents.OnCombatLogMessage += HandleCombatLogMessage;
 
        if (logText != null)
            logText.text = "";
    }
 
    private void OnDestroy()
    {
        GameEvents.OnCombatLogMessage -= HandleCombatLogMessage;
    }
 
    private void HandleCombatLogMessage(string message, CombatLogType type)
    {
        //El evento es sincrono, no podemos esperar aqui dentro: encolamos y dejamos que la coroutine consuma a su ritmo
        messageQueue.Enqueue((message, type));
 
        //Si no hay una coroutine de consumo corriendo ya, la lanzamos
        if (displayRoutine == null)
            displayRoutine = StartCoroutine(ConsumeQueue());
    }
 
    //Va sacando mensajes de la cola uno a uno: clear, muestra el mensaje, espera, repite
    private IEnumerator ConsumeQueue()
    {
        while (messageQueue.Count > 0)
        {
            var (message, type) = messageQueue.Dequeue();
 
            if (logText != null)
            {
                //Limpiamos el texto anterior para mostrar solo el mensaje actual
                logText.text = "[" + type + "] " + message;
            }
 
            yield return new WaitForSeconds(delayBetweenMessages);
        }
 
        //La cola se vacio, dejamos la referencia libre para que el siguiente mensaje relance la coroutine
        displayRoutine = null;
    }
}
