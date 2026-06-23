using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Effects/Heal")]
//Esta clase de effecto hereda de MoveEffect
public class HealEffect : MoveEffect
{
    //Hacemos override de Execute funcion que hereda de Move Effect
    public override IEnumerator Execute(MonsterUnit user, List<MonsterUnit> targets, MoveData move)
    {
        //Lista de fragmentos de log, uno por target, para agrupar por coma si hay mas de uno
        List<string> logFragments = new List<string>();

        //Por cada target del Move
        foreach(var target in targets)
        {
            //El target monster recibe la cantidad de curacion
            int healedAmount = target.monster.Heal(move.Power);

            //Acumulamos el fragmento de log de este target con la cantidad real curada
            logFragments.Add(target.monster.data.MonsterName + " recupera " + healedAmount + " de HP");

            //Esperamos medio segundo para que de la sensacion de aplicarse el efect
            yield return new WaitForSeconds(0.5f);
        }

        //Emitimos el log: un fragmento si hay un solo target, agrupados por coma si hay varios
        CombatLogHelper.RaiseGrouped(logFragments, CombatLogType.Heal);
    }
}
