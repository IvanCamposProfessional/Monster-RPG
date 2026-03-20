using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAI", menuName = "AI/EnemyAI")]
public class EnemyAI : ScriptableObject
{
    //Creamos una lista para los behaviours del enemy
    public List<AIBehaviour> behaviours;

    //Creamos una funcion para crear la decision a la cual le pasamos la unidad enemiga y los targets aliados
    public AIDecision MakeDecision(MonsterUnit enemy, List<MonsterUnit> allyTargets)
    {
        //Bucle que recorre los behaviours
        foreach(var behaviour in behaviours)
        {
            //Si el behaviour resuelve que se puede ejecutar
            if(behaviour.CanExecute(enemy, allyTargets))
            {
                return behaviour.Execute(enemy, allyTargets);
            }
        }

        //Si ninguno puede resolver devuelve null
        return null;
    }
}
