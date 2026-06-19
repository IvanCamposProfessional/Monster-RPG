using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Panel fijo por Spot que muestra info basica del monster (Type, Name, Level, HP, Altered States)
public class MonsterPanel : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image typeIcon;
    [SerializeField] private TMP_Text monsterNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Slider hpSlider;

    [Header("Altered States")]
    //Contenedor donde instanciamos un icono por cada estado activo
    [SerializeField] private Transform statesContainer;
    //Prefab del icono de estado
    [SerializeField] private GameObject stateIconPrefab;
    private List<GameObject> activeStateIcons = new List<GameObject>();

    [Header("Posicionamiento")]
    [SerializeField] private Vector2 screenOffset = new Vector2(0, -40f);

    private RectTransform panelRt;
    private Transform spot;
    private Camera cam;
    private RectTransform canvasRt;
    private MonsterUnit unit;

    private void Awake()
    {
        panelRt = GetComponent<RectTransform>();
    }

    public void Setup(MonsterUnit monsterUnit, Transform spotTransform, Camera combatCamera)
    {
        unit = monsterUnit;
        spot = spotTransform;
        cam = combatCamera;
 
        //Nos suscribimos para refrescar HP/estados cuando cambien
        GameEvents.OnMonsterStateChanged += RefreshIfMatches;
 
        RefreshInfo();
    }

    private void OnDestroy()
    {
        GameEvents.OnMonsterStateChanged -= RefreshIfMatches;
    }

    private void LateUpdate()
    {
        RectTransform parentRt = panelRt.parent as RectTransform;
        if (parentRt == null) return;

        //Convertimos la posicion del Spot (mundo) a posicion de pantalla
        Vector2 screenPos = cam.WorldToScreenPoint(spot.position);

        //Convertimos la posicion de pantalla a posicion local del contenedor padre del panel (no del Canvas)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRt, screenPos, null, out Vector2 localPos);

    panelRt.anchoredPosition = localPos + screenOffset;
    }

    // ─────────────────────────────────────────
    // REFRESCO DE DATOS
    // ─────────────────────────────────────────

    private void RefreshIfMatches(Monster monster)
    {
        //Solo refrescamos si el evento corresponde a nuestro monster
        if (unit != null && unit.monster == monster)
            RefreshInfo();
    }

    private void RefreshInfo()
    {
        //Comprobacion de seguridad
        if (unit == null || unit.monster == null) return;
 
        //Guardamos el monster al que corresponde el panel
        Monster monster = unit.monster;

        if (typeIcon != null)
        {
            Sprite typeSprite = GameManager.Instance.TypeIconDatabase.GetIconByType(monster.data.Type);
            if (typeSprite != null)
                typeIcon.sprite = typeSprite;
        }
 
        if (monsterNameText != null)
            monsterNameText.text = monster.data.MonsterName;
 
        if (levelText != null)
            levelText.text = "Lvl. " + monster.level;
 
        if (hpText != null)
        hpText.text = monster.currentHP + "/" + monster.maxHP;

        if (hpSlider != null)
            hpSlider.value = monster.maxHP > 0 ? (float)monster.currentHP / monster.maxHP : 0f;
 
        RefreshStateIcons(monster);
    }

    private void RefreshStateIcons(Monster monster)
    {
        //Limpiamos los state icons al empezar el script
        foreach (var icon in activeStateIcons)
            Destroy(icon);
        activeStateIcons.Clear();

        //Comprobacion de seguridad
        if (statesContainer == null || stateIconPrefab == null) return;

        //Creamos un bucle que recorre los altered states del Monster
        foreach (var state in monster.alteredStates)
        {
            //Instanciamos el prefab del state en el container y lo configuramos
            GameObject obj = Instantiate(stateIconPrefab, statesContainer, false);
            obj.GetComponent<MonsterStateIcon>().SetupAlteredState(state);
            activeStateIcons.Add(obj);
        }

        //Creamos un bucle que recorre los stat modifiers del monster
        foreach (var modifier in monster.statModifiers)
        {
            //Instanciamos el prefab del state en el container y lo configuramos
            GameObject obj = Instantiate(stateIconPrefab, statesContainer, false);
            obj.GetComponent<MonsterStateIcon>().SetupStatModifier(modifier);
            activeStateIcons.Add(obj);
        }
    }
}
