using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Creamos el Asset Menu
[CreateAssetMenu(fileName = "ApplyModifierEffect", menuName = "Effects/ApplyModifier")]
//Creamos el efecto de Aplly Modifier que hereda de Move Effect
public class ApplyModifierEffect : MoveEffect
{
    [Header("Tipo de Modifier")]
    public ModifierType modifierType;

    [Header("Configuracion StatModifier (Buff / Debuff)")]
    public StatModifier statModifier;

    [Header("Configuracion AlteredState")]
    public AlteredState alteredState;
    public int intensity;
    public int alteredStateDuration;

    //Ejecutamos el effect
    public override IEnumerator Execute(MonsterUnit user, List<MonsterUnit> targets, MoveData move)
    {
        //Lista de fragmentos de log, uno por target, para agrupar por coma si hay mas de uno
        List<string> logFragments = new List<string>();
        //Tipo de log a usar al emitir, depende de si es AlteredState o StatModifier
        CombatLogType logType = modifierType == ModifierType.AlteredState ? CombatLogType.Status : CombatLogType.Stat;

        //Por cada target del Move
        foreach(var target in targets)
        {
            //Si el Modifier es Altered State
            if(modifierType == ModifierType.AlteredState)
            {
                //Añadimos el Altered State al monster
                target.monster.AddAlteredState(alteredState, intensity, alteredStateDuration);

                //Acumulamos el fragmento de log de este target, usando la forma adjetiva del estado
                logFragments.Add(target.monster.data.MonsterName + " queda " + alteredState.stateNameAdjective);
            }
            //Si es stat modifier
            else
            {
                //Añadimos el stat modifier al monster
                target.monster.AddStatModifier(statModifier);

                //El signo de statModifier.value determina si es aumento o reduccion del stat
                string direction = statModifier.value >= 0 ? "aumenta" : "reduce";
                //Si es porcentual mostramos el valor como porcentaje (+20%), si es fijo como entero con signo (+2)
                string signedValue = statModifier.isPercentage ? (statModifier.value >= 0 ? "+" : "") + Mathf.RoundToInt(statModifier.value * 100f) + "%" : (statModifier.value >= 0 ? "+" : "") + Mathf.RoundToInt(statModifier.value);

                //Acumulamos el fragmento de log de este target
                logFragments.Add(target.monster.data.MonsterName + " " + direction + " su " + statModifier.statAffected + " (" + signedValue + ")");
            }

            yield return null;
        }

        //Emitimos el log: un fragmento si hay un solo target, agrupados por coma si hay varios
        CombatLogHelper.RaiseGrouped(logFragments, logType);
    }
}
