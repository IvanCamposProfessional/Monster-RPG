using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Creamos el Asset Menu
[CreateAssetMenu(fileName = "RemoveModifierEffect", menuName = "Effects/RemoveModifier")]
//Creamos el efecto de Remove Modifier que hereda de Move Effect
public class RemoveModifierEffect : MoveEffect
{
    public ModifierType modifierType;
    //El id del modifier que queremos eliminar
    public string modifierId;

    //Ejecutamos el effect
    public override IEnumerator Execute(MonsterUnit user, List<MonsterUnit> targets)
    {
        //Por cada target del Move
        foreach (var target in targets)
        {
            //Si el Modifier es Altered State
            if (modifierType == ModifierType.AlteredState)
                //Eliminamos el Altered State
                target.monster.RemoveAlteredState(modifierId);
            //Si es stat modifier
            else
                //Eliminamos el stat modifier
                target.monster.RemoveStatModifier(modifierId);

            yield return new WaitForSeconds(0.3f);
        }
    }
}
