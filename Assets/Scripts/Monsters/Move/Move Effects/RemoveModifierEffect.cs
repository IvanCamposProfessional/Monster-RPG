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
    //Referencia al AlteredState asset, solo se usa cuando modifierType es AlteredState
    public AlteredState alteredStateRef;

    //Ejecutamos el effect
    public override IEnumerator Execute(MonsterUnit user, List<MonsterUnit> targets, MoveData move)
    {
        //Lista de fragmentos de log, uno por target, para agrupar si hay varios
        List<string> logFragments = new List<string>();

        //Por cada target del Move
        foreach (var target in targets)
        {
            bool removed;

            //Si el Modifier es Altered State
            if (modifierType == ModifierType.AlteredState)
                //Eliminamos el Altered State
                removed = target.monster.RemoveAlteredState(modifierId);
            //Si es stat modifier
            else
                //Eliminamos el stat modifier
                removed = target.monster.RemoveStatModifier(modifierId);

            //Solo logueamos si realmente se elimino algo, si el modifier no existia no tiene sentido informar al jugador
            if (removed)
            {
                string effectName = (modifierType == ModifierType.AlteredState && alteredStateRef != null) ? alteredStateRef.stateNameAdjective : modifierId;
                logFragments.Add(target.monster.data.MonsterName + " ya no está " + effectName);
            }

            yield return new WaitForSeconds(0.3f);
        }

        CombatLogHelper.RaiseGrouped(logFragments, CombatLogType.Status);
    }
}
