using UnityEngine;
using UnityEngine.EventSystems;

//Interactable del edificio de gestion de Runes en el Hub
[RequireComponent(typeof(Collider2D))]
public class EssenceRuneInteractable : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GameEvents.RaiseRunePanelRequested();
    }
}
