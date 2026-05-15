using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase abstracta base del efecto del Move, se hereda de ScriptableObject para poder crear los efectos como Assets y asignarlos en el inspector directamente
public abstract class MoveEffect : ScriptableObject
{
    //Creamos una coroutine abstracta para luego al crear el efecto poder hacerle override
    public abstract IEnumerator Execute(MonsterUnit user, List<MonsterUnit> targets, MoveData move);
}
