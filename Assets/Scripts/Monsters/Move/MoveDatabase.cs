using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveDatabase", menuName = "Database/Move Database")]
public class MoveDatabase : ScriptableObject
{
    public List<MoveData> allMoves;

    //Funcion que devuelve el Move con el ID que le pasamos
    public MoveData GetMoveByID(string id)
    {
        return allMoves.Find(m => m.MoveID == id);
    }
}
