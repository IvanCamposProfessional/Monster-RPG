using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Boton de filtro por tipo en la columna Filters
public class EssenceRuneFilterButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private Toggle toggle;

    private MonsterType monsterType;
    private Action<MonsterType, bool> onToggled;
 
    public void Setup(MonsterType type, Action<MonsterType, bool> toggledCallback)
    {
        //Hacemos Setup del MonsterType, el Toggle Callback y el Type Text
        monsterType = type;
        onToggled = toggledCallback;
        typeText.text = type.ToString();
 
        //Configuramos el Toggle para desactivarlo y añadirle el Listener On Toggle Changed
        toggle.isOn = false;
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }
 
    //Invocamos On Toggled
    private void OnToggleChanged(bool isOn)
    {
        onToggled?.Invoke(monsterType, isOn);
    }
}
