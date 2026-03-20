using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterUnit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Monster monster;
    private SpriteRenderer sr;
    //public int Speed;
    //Variable para saber el progreso de la unidad dentro de la timeline de turnos
    public float timelineProgress;
    //Variable para saber si la MonsterUnit es aliada
    public bool IsAlly { get; private set; }

    public bool IsAlive => monster.IsAlive;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    //Funcion para hacer setup del monster en el prefab en la cual le pasaremos el monster que tiene que ser
    public void Setup(Monster monster)
    {
        //El prefab guarda que monster es segun el que le pasamos
        this.monster = monster;

        //Speed = monster.currentSpeed;
        
        //Cambiamos el sprite al correspondiente
        sr.sprite = monster.data.MonsterSprite;
        // Ajustamos el tamaño del sprite al tamaño del GameObject
        Vector3 objSize = transform.localScale; // Tamaño del GameObject en unidades locales
        Vector3 spriteSize = sr.sprite.bounds.size; // Tamaño del sprite en unidades

        // Escalamos el GameObject para que el sprite coincida con el tamaño del objeto
        transform.localScale = new Vector3(
            objSize.x / spriteSize.x,
            objSize.y / spriteSize.y,
            1f
        );

        //Añadimos el Polygon Collider una ve hecho el escalado del GameObject y del Sprite para que se ajuste bien
        gameObject.AddComponent<PolygonCollider2D>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CombatUIManager.UIManager.ShowAllyPanel(monster);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CombatUIManager.UIManager.HideAllyPanel();
    }

    //Funcion para indicar si es aliado o enemigo
    public void SetSide(bool isAlly)
    {
        IsAlly = isAlly;
    }

    //Creamos la funcion para saber si la Monster Unit ha sido clickada para seleccionarla como target y se lo pasamos al Combat Manager con la funcion On Unit Clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        CombatManager.Instance.OnUnitClicked(this);
    }
}
