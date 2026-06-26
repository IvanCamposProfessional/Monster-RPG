using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Detecta el input real de scroll del jugador via IScrollHandler y notifica al CombatFeedbackPanel.
[RequireComponent(typeof(ScrollRect))]
public class ScrollInputDetector : MonoBehaviour, IScrollHandler
{
    [SerializeField] private CombatFeedbackPanel feedbackPanel;
 
    private ScrollRect scrollRect;
 
    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    public void OnScroll(PointerEventData eventData)
    {
        //Dejamos que el ScrollRect procese el evento normalmente
        scrollRect.OnScroll(eventData);
 
        //Notificamos al panel la direccion del scroll del jugador
        if (feedbackPanel != null)
            feedbackPanel.OnPlayerScroll(eventData.scrollDelta.y);
    }
}
