using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveData", menuName = "Scriptable Objects/MoveData")]
public class MoveData : ScriptableObject
{
    public string MoveName;
    public string MoveDescription;
    public MonsterType MoveType;
    public MoveCategory Category;
    public int Power;
    //Variable target type para poder definir que tipo de target utiliza (single, multi, etc)
    public TargetType TargetType;
    //Lista de efectos del move en orden de ejecucion, se crea una lista porque un Move puede realizar mas de un efecto (ej: daño al enemigo y curar al ally)
    public List<MoveEffect> Effects;
    //Variable para almacenar el numero de tarjets en caso de que el Target Type sea Multiple
    public int TargetCount;
}
