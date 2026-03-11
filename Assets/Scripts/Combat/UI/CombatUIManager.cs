using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour
{
    public static CombatUIManager UIManager;
    [SerializeField]
    private GameObject MonsterPanel;

    [SerializeField]
    private Image MonsterIcon;
    [SerializeField]
    private TextMeshProUGUI MonsterNameText;
    [SerializeField]
    private TextMeshProUGUI MonsterHPText;
    [SerializeField]
    private TextMeshProUGUI MonsterBPText;

    [Header("StatesUI")]
    //Contenedor donde instanciaremos los iconos del estado
    [SerializeField]
    private Transform statesContainer;
    //Prefab del icono de estado
    [SerializeField]
    private GameObject stateIconPrefab;
    //Lista para poder limpiarlos al actualizar
    private List<GameObject> activeStateIcons = new List<GameObject>();
    

    private void Awake()
    {
        UIManager = this;
        HideAllyPanel();
    }

    public void ShowAllyPanel(Monster monster)
    {
        //Monstramos el panel
        MonsterPanel.SetActive(true);
        //Modificamos el texto del nombre del panel al del monster que se pasa desde el prefab con la data ya que es un campo no variable
        MonsterNameText.text = monster.data.MonsterName;
        //Modificamos la imagen del icono del panel al del monster que se pasa desde el prefab con la data ya que es un campo no variable
        MonsterIcon.sprite = monster.data.MonsterSprite;
        //Modificamos el texto de la HP del panel con 2 datos, el currentHP del monster ya que es variable y el BaseHP del monster
        MonsterHPText.text = "HP: " + monster.currentHP + "/" + monster.maxHP;
        //Modificamos el texto de la BP del panel con 2 datos, el currentBP del monster ya que es variable y el BaseBP del monster
        MonsterBPText.text = "BP: " + monster.currentBP + "/" + monster.maxBP;

        //Actualizamos los iconos de estado
        RefreshStateIcons(monster);
    }

    public void HideAllyPanel()
    {
        MonsterPanel.SetActive(false);
    }

    //Funcion para refrescar los state icons
    private void RefreshStateIcons(Monster monster)
    {
        //Limpiamos los iconos anteriores
        foreach(var icon in activeStateIcons)
        {
            //Destruimos el game object
            Destroy(icon);
        }
        //Limpiamos la lista
        activeStateIcons.Clear();

        //Instanciamos un icono por cada altered state activo
        foreach (var state in monster.alteredStates)
        {
            //Instanciamos el icon
            GameObject obj = Instantiate(stateIconPrefab, statesContainer, false);
            //Hacemos setup al altered state
            obj.GetComponent<MonsterStateIcon>().SetupAlteredState(state);
            //Añadimos el icon a la lista
            activeStateIcons.Add(obj);
        }

        // Instanciamos un icono por cada stat modifier activo
        foreach (var modifier in monster.statModifiers)
        {
            GameObject obj = Instantiate(stateIconPrefab, statesContainer, false);
            obj.GetComponent<MonsterStateIcon>().SetupStatModifier(modifier);
            activeStateIcons.Add(obj);
        }
    }
}
