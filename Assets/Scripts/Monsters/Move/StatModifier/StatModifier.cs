using System.Xml;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "StatModifier", menuName = "Modifiers/StatModifier")]
public class StatModifier : ScriptableObject
{
    public string modifierId;
    public string modifierName;
    public Sprite icon;
    public ModifierType modifierType;
    public StatType statAffected;
    //Creamos value como float ya que podemos guardar valor fijo o porcentual
    public float value;
    //Variable para saber si el value es porcentual, True = porcentaje y False = valor fijo
    public bool isPercentage;
    public int duration;
    public bool stackable;

    //Crea una instancia runtime del modifier
    public StatModifierInstance CreateInstance()
    {
        return new StatModifierInstance(this);
    }
}
