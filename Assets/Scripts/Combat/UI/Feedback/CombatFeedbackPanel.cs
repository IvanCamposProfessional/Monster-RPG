using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Panel de feedback de combate: muestra el log cronologico de acciones, efectos y eventos del combate.
public class CombatFeedbackPanel : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Scrollbar scrollbarVertical;

    [Header("Configuracion de scroll")]
    //Umbral de posicion vertical del scroll (0 = arriba, 1 = abajo) por debajo del cual se considera que el jugador esta revisando hacia arriba
    [SerializeField] private float autoScrollThreshold = 0.15f;

    //Si es true, el scroll sigue bajando automaticamente al recibir mensajes nuevos, se desactiva cuando el jugador sube manualmente, se reactiva cuando vuelve al fondo
    private bool autoScrollEnabled = true;
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

        //Nos suscribimos al onValueChanged de la scrollbar para detectar cuando el jugador la arrastra
        if (scrollbarVertical != null)
            scrollbarVertical.onValueChanged.AddListener(OnScrollbarValueChanged);
 
        if (feedbackText != null)
            feedbackText.text = "";
    }

    private void OnDestroy()
    {
        GameEvents.OnCombatLogMessage -= HandleCombatLogMessage;

        if (scrollbarVertical != null)
            scrollbarVertical.onValueChanged.RemoveListener(OnScrollbarValueChanged);
    }

    //Llamado por ScrollInputDetector cuando el jugador usa la rueda del raton
    public void OnPlayerScroll(float scrollDeltaY)
    {
        if (scrollRect == null) return;
 
        //Scroll hacia arriba: pausamos el autoscroll
        if (scrollDeltaY > 0f)
        {
            autoScrollEnabled = false;
        }
        //Scroll hacia abajo: si ya estamos cerca del fondo reactivamos el autoscroll
        else if (scrollDeltaY < 0f && scrollRect.verticalNormalizedPosition <= autoScrollThreshold)
        {
            autoScrollEnabled = true;
            StartCoroutine(ScrollToBottomNextFrame());
        }
    }

    //Llamado cuando cambia el valor de la scrollbar vertical
    private void OnScrollbarValueChanged(float value)
    {
        //Si el scroll lo mueve el codigo, ignoramos el evento
        if (isScrollingByCode) return;
 
        //Si la scrollbar sube por encima del threshold, el jugador esta revisando: pausamos el autoscroll
        if (value > autoScrollThreshold)
            autoScrollEnabled = false;
        //Si la scrollbar vuelve al fondo, reactivamos el autoscroll
        else
            autoScrollEnabled = true;
    }

    private void HandleCombatLogMessage(string message, CombatLogType type)
    {
        //Comprobacion de seguridad
        if (feedbackText == null) return;

        //Guardamos la posicion absoluta en pixeles antes de añadir texto
        float savedPosition = scrollRect.content.anchoredPosition.y;

        //Convertimos el color del tipo a formato hex para usar el rich text de TextMeshPro
        string hex = ColorUtility.ToHtmlStringRGB(GetColorForType(type));

        //Añadimos la nueva linea con su color, usando rich text de TMP, si ya hay texto, añadimos un salto de linea antes
        if (feedbackText.text.Length > 0)
            feedbackText.text += "\n";

        feedbackText.text += "<color=#" + hex + ">" + message + "</color>";

        //Si el autoscroll esta activo, bajamos al ultimo mensaje en el siguiente frame, (necesitamos esperar un frame para que el Content Size Fitter haya recalculado el tamaño)
        if (autoScrollEnabled)
            StartCoroutine(ScrollToBottomNextFrame());
        else
            StartCoroutine(RestoreScrollPositionNextFrame(savedPosition));
    }

    private IEnumerator RestoreScrollPositionNextFrame(float savedPosition)
    {
        isScrollingByCode = true;
        yield return null;
        if (scrollRect != null)
        {
            Vector2 pos = scrollRect.content.anchoredPosition;
            pos.y = savedPosition;
            scrollRect.content.anchoredPosition = pos;
        }
        isScrollingByCode = false;
    }

    //Esperamos un frame para que Unity recalcule el layout antes de hacer scroll al fondo
    private IEnumerator ScrollToBottomNextFrame()
    {
        isScrollingByCode = true;
        yield return null;
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 0f;
        yield return null;
        isScrollingByCode = false;
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
