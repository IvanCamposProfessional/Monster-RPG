using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterStateIcon : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private TextMeshProUGUI valueText;

    //Para altered states mostramos la intensidad
    public void SetupAlteredState(AlteredStateInstance state)
    {
        iconImage.sprite = state.icon;
        valueText.text = state.intensity.ToString();
    }

    //Para los stat modifiers mostramos los turnos restantes
    public void SetupStatModifier(StatModifierInstance modifier)
    {
        iconImage.sprite = modifier.icon;
        valueText.text = modifier.remainingDuration.ToString();
    }
}
