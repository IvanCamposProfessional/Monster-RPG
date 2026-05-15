using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class SummonInteractable : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        //Notificamos que el jugador quiere abrir el panel de invocacion
        GameEvents.RaiseSummonPanelRequested();
    }
}
