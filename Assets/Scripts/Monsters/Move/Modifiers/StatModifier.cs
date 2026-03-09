using System.Xml;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class StatModifier : Modifier
{
    public StatType statAffected;
    //Creamos value como float ya que podemos guardar valor fijo o porcentual
    public float value;
    //Variable para saber si el value es porcentual, True = porcentaje y False = valor fijo
    public bool isPercentage;

    //Guardamos el valor original del stat a modificar para restaurarlo al expirar
    private int originalValue;

    //Creamos el constructor
    public StatModifier(string id, ModifierType type, StatType stat, float value, bool isPercentage, int duration, bool stackable)
    {
        //Definimos las variables de Modifier al crear el stat modifier
        Id = id;
        modifierType = type;
        statAffected = stat;
        //El value e is Percentage del StatModifier es igual al value que le pasamos al crearlo
        this.value = value;
        this.isPercentage = isPercentage;
        //Terminamos de crear la duration, stackable y timing con los valores que le pasamos al crearlo
        this.duration = duration;
        this.stackable = stackable;
        timing = ModifierTiming.OnTurnEnd; //Los buffs debuffs son pasivos y no hacen un efecto en tick por turno
    }

    public override void OnApply(Monster monster)
    {
        switch (statAffected)
        {
            case StatType.Attack:
                //Guardamos el valor original del Attack
                originalValue = monster.currentAttack;
                //Calculamos el nuevo value modificado del Attack
                monster.currentAttack = CalculateNewValue(monster.currentAttack);
                break;
            case StatType.Defense:
                //Guardamos el valor original del Defense
                originalValue = monster.currentDefense;
                //Calculamos el nuevo value modificado del Speed
                monster.currentDefense = CalculateNewValue(monster.currentDefense);
                break;
            case StatType.Speed:
                //Guardamos el valor original del Speed
                originalValue = monster.currentSpeed;
                //Calculamos el nuevo value modificado del Speed
                monster.currentSpeed = CalculateNewValue(monster.currentSpeed);
                break;
        }
    }

    public override void OnRemove(Monster monster)
    {
        //Recalculamos restaurando el valor original
        switch (statAffected)
        {
             case StatType.Attack:
                monster.currentAttack = originalValue;
                break;
            case StatType.Defense:
                monster.currentDefense = originalValue;
                break;
            case StatType.Speed:
                monster.currentSpeed = originalValue;
                break;
        }
    }

    //Los StatModifier no hacen nada por turno, solo reducen su duracion
    public override bool OnTick(Monster monster)
    {
        duration--;
        //Devuelve true si la duration es <= 0
        return duration <= 0;
    }

    //Funcion para calcular y devolver el New Value de un stat pasandole un value
    private int CalculateNewValue(int actualValue)
    {
        //Si la modificacion es porcentual
        if (isPercentage)
        {
            //Calculamos la modificacion porcentual del value
            return Mathf.RoundToInt(actualValue * (1f + value));
        }
        else
        {
            //Devolvemos el actual value + el value de la modificacion
            return actualValue + Mathf.RoundToInt(value);
        }
    }
}
