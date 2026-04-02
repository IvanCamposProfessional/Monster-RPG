using TMPro;
using UnityEngine;

//Script que va en cada boton del slot y gestiona su apariencia segun si esta vacio u ocupado
public class SlotButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI slotNameText;
    [SerializeField] private TextMeshProUGUI playTimeText;

    //Variable para saber el indice del slot
    private int slotIndex;
    //Variable para guardar el Menu Manager
    private MainMenuManager menuManager;

    //Funcion para setear la info del slot
    public void Setup(SlotInfo info, MainMenuManager manager)
    {
        //Guardamos el slot index segun el slot que se pasa a la funcion
        slotIndex = info.slot;
        //Guardamos el Menu Manager
        menuManager = manager;

        //Si el slot esta vacio
        if (info.isEmpty)
        {
            //Indicamos que el slot esta vacio
            slotNameText.text = "Slot " + (info.slot + 1) + "\nFree";
            //Ponemos el texto del playtime vacio
            playTimeText.text = "";
        }
        //Si el slot contiene guardado
        else
        {
            //Indicamos el nombre del player
            slotNameText.text = info.playerName;
            //Indicamos el play time
            playTimeText.text = info.GetFormattedPlayTime();
        }
    }

    //Seteamos la funcion que se va a lanzar al pulsar el boton
    public void OnClicked()
    {
        //Lanzamos el OnSlotClicked del Menu Manager
        menuManager.OnSlotClicked(slotIndex);
    }
}
