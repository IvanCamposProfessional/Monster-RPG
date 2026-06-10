using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Card de Rune en la columna de Runes desbloqueadas, muestra el estado equipable (verde) o no equipable (rojo) segun el monster seleccionado
[RequireComponent(typeof(CanvasGroup))]
public class EssenceRuneCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image runeIcon;
    [SerializeField] private TextMeshProUGUI runeNameText;
    [SerializeField] private TextMeshProUGUI moveNameText;
    [SerializeField] private Transform typeIconsContainer;
    [SerializeField] private GameObject typeIconPrefab;
    [SerializeField] private Image cardBackground;
    [SerializeField] private Color neutralColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color equipableColor = new Color(0.6f, 1f, 0.6f, 1f);
    [SerializeField] private Color equippedColor = new Color(0.4f, 0.7f, 1f, 1f);
    [SerializeField] private Color otherMonsterColor = new Color(1f, 0.7f, 0.3f, 1f);
    [SerializeField] private Color incompatibleColor = new Color(1f, 0.4f, 0.4f, 1f);

    private CanvasGroup canvasGroup;
 
    private EssenceRune currentRune;
    private MonsterSaveData selectedMonster;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // ─────────────────────────────────────────
    // SETUP
    // ─────────────────────────────────────────

    public void Setup(EssenceRune rune, EssenceRuneCardState state, MonsterSaveData monster)
    {
        //Guardamos la CurrentRune y el SelectedMonster
        currentRune = rune;
        selectedMonster = monster;
 
        //Asignamos la UI del slot
        runeIcon.sprite = rune.RuneIcon;
        runeNameText.text = rune.RuneID;
        moveNameText.text = rune.MoveData != null ? rune.MoveData.MoveName : string.Empty;
 
        BuildTypeIcons(rune);
 
        //Color segun el state de la card
         switch (state)
        {
            case EssenceRuneCardState.Neutral: cardBackground.color = neutralColor; break;
            case EssenceRuneCardState.Equipable: cardBackground.color = equipableColor; break;
            case EssenceRuneCardState.Equipped: cardBackground.color = equippedColor; break;
            case EssenceRuneCardState.OtherMonster: cardBackground.color = otherMonsterColor; break;
            case EssenceRuneCardState.Incompatible: cardBackground.color = incompatibleColor; break;
        }
    }

    private void BuildTypeIcons(EssenceRune rune)
    {
        //Destruimos todos los TypeIcon anteriores
        foreach (Transform child in typeIconsContainer)
            Destroy(child.gameObject);
 
        //Comrpbacion de seguridad
        if (rune.MoveData == null || rune.MoveData.EssenceAmountToUse == null) return;
 
        //Creamos un bucle que recorre la essence amount to use del move data de la rune
        foreach (EssenceAmount ea in rune.MoveData.EssenceAmountToUse)
        {
            //Instanciamos el icon del type
            GameObject icon = Instantiate(typeIconPrefab, typeIconsContainer);
            //Asignamos el sprite del tipo via TypeIconDatabase
            Sprite typeSprite = GameManager.Instance.TypeIconDatabase.GetIconByType(ea.Type);
            if (typeSprite != null)
                icon.GetComponent<Image>().sprite = typeSprite;
        }
    }
    
    // ─────────────────────────────────────────
    // DRAG (source)
    // ─────────────────────────────────────────
 
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.45f;
        //Informamos al Manager UI de que ha comenzado el drag
        EssenceRuneManagerUI.Instance.BeginDrag(currentRune.RuneID, currentRune.RuneIcon, RuneDragSource.Unlocked, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Informamos al Manager UI que se esta updateando el drag
        EssenceRuneManagerUI.Instance.UpdateDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        //Informamos al Manager UI que ha acabado el drag
        EssenceRuneManagerUI.Instance.EndDrag();
    }

    // ─────────────────────────────────────────
    // DROP (target — desequipar si la Rune viene de equipada)
    // ─────────────────────────────────────────

    public void OnDrop(PointerEventData eventData)
    {
        //Informamos al Manager UI que se ha hecho el drop
        EssenceRuneManagerUI.Instance.HandleDropOnUnlocked();
    }
}
