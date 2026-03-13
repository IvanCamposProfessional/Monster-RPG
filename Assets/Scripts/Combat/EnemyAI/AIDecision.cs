using System.Collections.Generic;
using UnityEngine;

//Clase que devuelve la decision del Enemy, que move usar y que targets
public class AIDecision
{
    public MoveData move;
    public List<MonsterUnit> targets;

    //Creamos el constructor
    public AIDecision(MoveData move, List<MonsterUnit> targets)
    {
        this.move = move;
        this.targets = targets;
    }
}
