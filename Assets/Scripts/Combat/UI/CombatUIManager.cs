using TMPro;
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
    }

    public void HideAllyPanel()
    {
        MonsterPanel.SetActive(false);
    }
}
