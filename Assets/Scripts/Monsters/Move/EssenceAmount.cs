using System;
using UnityEngine;

//Representa una unidad de Essence, se usa tanto para definir el coste de un Essence Move como para definir la generacion de un Basic Move
[Serializable]
public struct EssenceAmount
{
    public MonsterType Type;
    public int Amount;
}
