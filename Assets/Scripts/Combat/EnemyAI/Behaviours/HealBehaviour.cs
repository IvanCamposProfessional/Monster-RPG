using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "HealBehaviour", menuName = "AI/HealBehaviour")]
public class HealBehaviour : AIBehaviour
{
    //Variable que define en que porcentaje de vida se cura el Enemy
    [Range(0f, 1f)]
    float hpThreshold = 0.3f;

    public override bool CanExecute(MonsterUnit enemy, List<MonsterUnit> allyTargets)
    {
        //Comprobamos si el monster tiene menos HP del umbral
        float hpPercent = (float)enemy.monster.currentHP / enemy.monster.maxHP;
        //Si la unit tiene mas porcentage de vida que el threshold no se puede ejecutar el behaviour(devolvemos false)
        if(hpPercent >= hpThreshold) return false;

        //Comprobamos si tiene algun move con HealEffect y deevuelve true en caso de que asi sea
        return enemy.monster.learnedMoves.Any(m => MoveHasEffect<HealEffect>(m));
    }

    public override AIDecision Execute(MonsterUnit enemy, List<MonsterUnit> allyTargets)
    {
        //Buscamos el primer move con HealEffect
        MoveData healMove = enemy.monster.learnedMoves.First(m => MoveHasEffect<HealEffect>(m));

        //El target es el propio enemy ya que se cura a si mismo
        return new AIDecision(healMove, new List<MonsterUnit>{ enemy });
    }
}
