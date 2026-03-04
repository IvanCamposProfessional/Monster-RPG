using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CombatMenu : MonoBehaviour
{
    //Variable para guardar el Combat Manager
    public CombatManager combatManager;
    //Variable para guardar el Prefab del boton de Move
    public GameObject moveButtonPrefab;
    //Panel Combat Menu
    public Transform buttonContainer;
    public GameObject movesButton;

    //Variable para saber la current unit del turno
    private MonsterUnit currentUnit;

    //Lista para guardar los botones que se instancian en el Combat Menu
    public List<GameObject> currentButtons = new List<GameObject>();

    //Funcion para que el Menu sepa la Unit de la cual es el turno y se llama desde el Battle Manager
    public void SetCurrentUnit(MonsterUnit unit)
    {
        currentUnit = unit;
    }

    //Funcion para definir lo que ocurre cuando se muestre el menu, tenemos que pasarle una Monster Unit para saber los moves que tiene
    public void ShowMenu()
    {
        //Por seguridad
        if(currentUnit == null)
        {
            return;
        }

        //Limpiar botones anteriores
        foreach (var button in currentButtons)
        {
            Destroy(button);
        }

        //Limpiar la lista de los botones anteriores
        currentButtons.Clear();

        //Desactivamos el boton de moves
        movesButton.SetActive(false);

        //Instancias un boton por cada ataque que tiene aprendido la Monster Unit
        foreach(var move in currentUnit.monster.learnedMoves)
        {
            //Instanciamos un prefab button en el Combat Menu
            GameObject moveBtn = Instantiate(moveButtonPrefab, buttonContainer);
            //Cambiamos el texto del boton al nombre del Move
            moveBtn.GetComponentInChildren<TMP_Text>().text = move.MoveName;

            //Añadimos un listener al boton
            moveBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                //Decimos que move ha elegido el player
                combatManager.chosenMove = move;
                //Indicamos que el Player ya ha elegido accion
                combatManager.moveChosen = true;
            });

            //Guardamos el boton en la lista de los botones activos
            currentButtons.Add(moveBtn);
        }

        //Mostramos el panel
        gameObject.SetActive(true);
    }

    //Funcion para ocultar el Combat Menu
    public void HideMenu()
    {
        //Ocultamos el panel
        gameObject.SetActive(false);
    }
}
