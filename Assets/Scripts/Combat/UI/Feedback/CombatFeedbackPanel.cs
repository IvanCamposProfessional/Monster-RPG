using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Panel de feedback de combate: muestra el log cronologico de acciones, efectos y eventos del combate.
public class CombatFeedbackPanel : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Configuracion de scroll")]
    //Umbral de posicion vertical del scroll (0 = arriba, 1 = abajo) por debajo del cual se considera que el jugador esta revisando hacia arriba
    [SerializeField] private float autoScrollThreshold = 0.01f;

    //Si es true, el scroll sigue bajando automaticamente al recibir mensajes nuevos, se desactiva cuando el jugador sube manualmente, se reactiva cuando vuelve al fondo
    private bool autoScrollEnabled = true;

    //Flag que indica que el scroll esta siendo movido por codigo, no por el jugador
    private bool isScrollingByCode = false;

    // ─────────────────────────────────────────── Colores por CombatLogType ───────────────────────────────────────────
    private static readonly Color ColorSystem    = new Color(1.00f, 1.00f, 1.00f); // #FFFFFF Blanco puro
    private static readonly Color ColorTurn      = new Color(0.66f, 0.83f, 0.94f); // #A8D4F0 Azul claro
    private static readonly Color ColorAction    = new Color(1.00f, 0.91f, 0.48f); // #FFE87A Amarillo claro
    private static readonly Color ColorMiss      = new Color(0.67f, 0.67f, 0.67f); // #AAAAAA Gris claro
    private static readonly Color ColorDamage    = new Color(1.00f, 0.44f, 0.44f); // #FF7070 Rojo claro
    private static readonly Color ColorHeal      = new Color(0.50f, 0.91f, 0.63f); // #80E8A0 Verde claro
    private static readonly Color ColorStat      = new Color(0.77f, 0.63f, 1.00f); // #C4A0FF Lila claro
    private static readonly Color ColorStatus    = new Color(1.00f, 0.69f, 0.38f); // #FFB060 Naranja claro
    private static readonly Color ColorTimeline  = new Color(0.44f, 0.91f, 0.91f); // #70E8E8 Cian claro
    private static readonly Color ColorKO        = new Color(1.00f, 0.27f, 0.27f); // #FF4444 Rojo brillante

    private void Awake()
    {
        GameEvents.OnCombatLogMessage += HandleCombatLogMessage;
 
        if (feedbackText != null)
            feedbackText.text = "";
    }

    private void OnDestroy()
    {
        GameEvents.OnCombatLogMessage -= HandleCombatLogMessage;
    }

    void Update()
    {
        //Si el jugador habia subido a revisar y vuelve al fondo, reactivamos el autoscroll 
        //(si el autoscroll estaba deshabilitado y la posicion del scroll <= que la posicion que hemos definido para que vuelva el autoscroll)
        if (!autoScrollEnabled && scrollRect != null && scrollRect.verticalNormalizedPosition <= autoScrollThreshold)
            autoScrollEnabled = true;
    }

    private void HandleCombatLogMessage(string message, CombatLogType type)
    {
        //Comprobacion de seguridad
        if (feedbackText == null) return;

        //Convertimos el color del tipo a formato hex para usar el rich text de TextMeshPro
        string hex = ColorUtility.ToHtmlStringRGB(GetColorForType(type));

        //Añadimos la nueva linea con su color, usando rich text de TMP, si ya hay texto, añadimos un salto de linea antes
        if (feedbackText.text.Length > 0)
            feedbackText.text += "\n";

        feedbackText.text += "<color=#" + hex + ">" + message + "</color>";

        //Si el autoscroll esta activo, bajamos al ultimo mensaje en el siguiente frame, (necesitamos esperar un frame para que el Content Size Fitter haya recalculado el tamaño)
        if (autoScrollEnabled)
            StartCoroutine(ScrollToBottomNextFrame());
    }

    //Detectamos si el jugador ha subido manualmente el scroll para pausar el autoscroll
    public void OnScrollValueChanged(Vector2 scrollValue)
    {
        //Si el scroll esta por encima del umbral de fondo, el jugador esta revisando hacia arriba
        if (scrollValue.y > autoScrollThreshold)
            autoScrollEnabled = false;
    }

    //Esperamos un frame para que Unity recalcule el layout antes de hacer scroll al fondo
    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return null;
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 0f;
    }

    //Devuelve el color correspondiente a cada CombatLogType
    private Color GetColorForType(CombatLogType type)
    {
        switch (type)
        {
            case CombatLogType.System: return ColorSystem;
            case CombatLogType.Turn: return ColorTurn;
            case CombatLogType.Action: return ColorAction;
            case CombatLogType.Miss: return ColorMiss;
            case CombatLogType.Damage: return ColorDamage;
            case CombatLogType.Heal: return ColorHeal;
            case CombatLogType.Stat: return ColorStat;
            case CombatLogType.Status: return ColorStatus;
            case CombatLogType.Timeline: return ColorTimeline;
            case CombatLogType.KO: return ColorKO;
            default: return ColorSystem;
        }
    }
}
