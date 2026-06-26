using UnityEngine;
using UnityEngine.UI;

public class TimelineIcon : MonoBehaviour
{
    public MonsterUnit unit;
    [SerializeField] private Image IconImage;
    [SerializeField] private Image Highlight;
    [SerializeField] private Image backgroundImage;

    public float HighlightWidth => Highlight.rectTransform.rect.width;
    private RectTransform iconRt;

    private void Awake()
    {
        //Al despertar el script guardamos el rect transform del objeto
        iconRt = GetComponent<RectTransform>();
        //Deshabilitamos la imagen de Highlight al despertar
        SetHighlight(false);
    }

    //Creamos una funcion Setap en la que le decimos que MonsterUnit es y cambiamos de imagen el icon al icono de la Unit y asignamos el color del Background
    public void SetupTimelineIcon(MonsterUnit monsterUnit)
    {
        unit = monsterUnit;
        IconImage.sprite = unit.monster.data.MonsterIcon; 
        backgroundImage.color = unit.IsAlly ? Color.green : Color.red;
    }

    //Funcion para setear la position en la timeline del icon
    public void SetPosition(float x, float y)
    {
        iconRt.anchoredPosition = new Vector2(x, y);
    }

    //Creamos una funcion para activar el resaltado del icono
    public void SetHighlight(bool active)
    {
        Highlight.enabled = active;
    }
}
