using System;
using System.Collections.Generic;
using UnityEngine;

//Par tipo de monster + icono
[Serializable]
public class TypeIconEntry
{
    public MonsterType type;
    public Sprite icon;
}

[CreateAssetMenu(fileName = "TypeIconDatabase", menuName = "Database/Type Icon Database")]
public class TypeIconDatabase : ScriptableObject
{
    public List<TypeIconEntry> entries;

    //Devuelve el sprite del tipo indicado, null si no esta registrado
    public Sprite GetIconByType(MonsterType type)
    {
        return entries.Find(e => e.type == type)?.icon;
    }
}
