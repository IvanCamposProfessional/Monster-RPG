using UnityEngine;

public class StatModifierInstance
{
    //Referencia al stat modifier
    public StatModifier data;
    public int remainingDuration;
    public int originalValue;
    public float originalEvasionValue;

    //Guardamos el id de data
    public string modifierId => data.modifierId;
    //Guardamos el icon de data
    public Sprite icon => data.icon;
    //Guardamos el name de data
    public string modifierName => data.modifierName;

    //Creamos el constructor
    public StatModifierInstance(StatModifier data)
    {
        this.data = data;
        remainingDuration = data.duration;
    }

    public void OnApply(Monster monster)
    {
        switch (data.statAffected)
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
             case StatType.SpecialAttack:
                //Guardamos el valor original del SpecialAttack
                originalValue = monster.currentSpecialAttack;
                //Calculamos el nuevo value modificado del SpecialAttack
                monster.currentSpecialAttack = CalculateNewValue(monster.currentSpecialAttack);
                break;
            case StatType.SpecialDefense:
                //Guardamos el valor original del SpecialDefense
                originalValue = monster.currentSpecialDefense;
                //Calculamos el nuevo value modificado del SpecialDefense
                monster.currentSpecialDefense = CalculateNewValue(monster.currentSpecialDefense);
                break;
            case StatType.Speed:
                //Guardamos el valor original del Speed
                originalValue = monster.currentSpeed;
                //Calculamos el nuevo value modificado del Speed
                monster.currentSpeed = CalculateNewValue(monster.currentSpeed);
                break;
            case StatType.Evasion:
                originalEvasionValue = monster.currentEvasion;
                monster.currentEvasion = CalculateNewEvasionValue(monster.currentEvasion);
                break;
        }
    }

    public void OnRemove(Monster monster)
    {
        //Recalculamos restaurando el valor original
        switch (data.statAffected)
        {
            case StatType.Attack:
                monster.currentAttack = originalValue;
                break;
            case StatType.Defense:
                monster.currentDefense = originalValue;
                break;
            case StatType.SpecialAttack:
                monster.currentSpecialAttack = originalValue;
                break;
            case StatType.SpecialDefense:
                monster.currentSpecialDefense = originalValue;
                break;
            case StatType.Speed:
                monster.currentSpeed = originalValue;
                break;
            case StatType.Evasion:
                monster.currentEvasion = originalEvasionValue;
                break;
        }
    }

    //Los StatModifier no hacen nada por turno, solo reducen su duracion
    public bool OnTick(Monster monster)
    {
        remainingDuration--;
        //Devuelve true si la duration es <= 0
        return remainingDuration <= 0;
    }

    //Funcion para calcular y devolver el New Value de un stat pasandole un value
    private int CalculateNewValue(int actualValue)
    {
        //Si la modificacion es porcentual
        if (data.isPercentage)
        {
            //Calculamos la modificacion porcentual del value
            return Mathf.RoundToInt(actualValue * (1f + data.value));
        }
        else
        {
            //Devolvemos el actual value + el value de la modificacion
            return actualValue + Mathf.RoundToInt(data.value);
        }
    }

    //Funcion para calcular y devolver el New Value de Evasion, sin redondear porque Evasion es float
    private float CalculateNewEvasionValue(float actualValue)
    {
        if (data.isPercentage)
            return actualValue * (1f + data.value);
        else
            return actualValue + data.value;
    }
}
