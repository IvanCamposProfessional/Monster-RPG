using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Scriptable object abstracto base para definir el comportamiento del Enemy
public abstract class AIBehaviour : ScriptableObject
{
    //Evalua si el behaviour puede ejecutarse
    //Recibe el Enemy que actua y los allies vivos contra los que puede atacar
    public abstract bool CanExecute(MonsterUnit enemy, List<MonsterUnit> allyTargets);

    //Devuelve la decision: move elegido y targets
    public abstract AIDecision Execute(MonsterUnit enemy, List<MonsterUnit> allyTargets);

    //Funcion auxiliar para comprobar si un move tiene un efecto de un tipo concreto
    protected bool MoveHasEffect<T>(MoveData move) where T : MoveEffect
    {
        return move.Effects.Any(e => e is T);
    }
}
