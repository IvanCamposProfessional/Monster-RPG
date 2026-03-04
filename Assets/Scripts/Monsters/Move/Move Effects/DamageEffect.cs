using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Effects/Damage")]
//Esta clase de effecto hereda de MoveEffect
public class DamageEffect : MoveEffect
{
    //Variable para definir la cantidad de daño que recibe el target
    public int damageAmount;

    //Hacemos override de Execute funcion que hereda de Move Effect
    public override IEnumerator Execute(MonsterUnit user, List<MonsterUnit> targets)
    {
        //Por cada target del Move
        foreach(var target in targets)
        {
            //El target monster recibe la cantidad de daño
            target.monster.TakeDamage(damageAmount);
            //Esperamos medio segundo para que de la sensacion de aplicarse el efect
            yield return new WaitForSeconds(0.5f);
        }
    }
}
