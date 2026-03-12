using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

//Creamos el Asset Menu
[CreateAssetMenu(fileName = "ApplyModifierEffect", menuName = "Effects/ApplyModifier")]
//Creamos el efecto de Aplly Modifier que hereda de Move Effect
public class ApllyModifierEffect : MoveEffect
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
        //Por cada target del Move
        foreach(var target in targets)
        {
            //Si el Modifier es Altered State
            if(modifierType == ModifierType.AlteredState)
            {
                //Añadimos el Altered State al monster
                target.monster.AddAlteredState(alteredState, intensity, alteredStateDuration);
            }
            //Si es stat modifier
            else
            {
                //Añadimos el stat modifier al monster
                target.monster.AddStatModifier(statModifier);
            }

            yield return null;
        }
    }
}
