using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Effects/Heal")]
//Esta clase de effecto hereda de MoveEffect
public class HealEffect : MoveEffect
{
    //Variable para definir lo que se cura el target
    public int healAmount;

    //Hacemos override de Execute funcion que hereda de Move Effect
    public override IEnumerator Execute(MonsterUnit user, List<MonsterUnit> targets)
    {
        //Por cada target del Move
        foreach(var target in targets)
        {
            //El target monster recibe la cantidad de curacion
            target.monster.Heal(healAmount);
            //Esperamos medio segundo para que de la sensacion de aplicarse el efect
            yield return new WaitForSeconds(0.5f);
        }
    }
}
