using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Card de Monster en la columna de Party de UI
public class EssenceRuneMonsterCard : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    [SerializeField] private Image monsterIcon;
    [SerializeField] private TextMeshProUGUI monsterNameText;
    [SerializeField] private Image monsterTypeIcon;
    [SerializeField] private Image selectedHighlight;

    private MonsterSaveData monsterSave;
    private Action<MonsterSaveData> onSelected;

    // ─────────────────────────────────────────
    // SETUP
    // ─────────────────────────────────────────

    public void Setup(MonsterSaveData save, MonsterData data, Action<MonsterSaveData> selectedCallback)
    {
        //Guardamos el Monster y el Callback de selected
        monsterSave = save;
        onSelected = selectedCallback;

        if (data != null)
        {
            //Cambiamos el sprite del monster y el name
            monsterIcon.sprite = data.MonsterIcon;
            monsterNameText.text = data.MonsterName;
        }

        //Guardamos el TypeSprite y lo asignamos al de la card
        Sprite typeSprite = GameManager.Instance.TypeIconDatabase.GetIconByType(save.monsterType);
        if (typeSprite != null)
            monsterTypeIcon.sprite = typeSprite;

        //Al inicializar como no hay ningun monster seleccionado ponemos el selected highlight en false
        selectedHighlight.gameObject.SetActive(false);
    }

    //Refresca el highlight segun el Monster seleccionado actualmente
    public void RefreshSelected(MonsterSaveData currentSelected)
    {
        selectedHighlight.gameObject.SetActive(currentSelected == monsterSave);
    }

    // ─────────────────────────────────────────
    // INPUT
    // ─────────────────────────────────────────

    public void OnPointerClick(PointerEventData eventData)
    {
        onSelected?.Invoke(monsterSave);
    }

    //Drop en el Monster card, equipar la Rune arrastrada en el primer slot libre
    public void OnDrop(PointerEventData eventData)
    {
        EssenceRuneManagerUI.Instance.HandleDropOnMonster(monsterSave);
    }
}
