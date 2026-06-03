using System;
using System.Collections.Generic;

[Serializable]
public class MonsterSaveData
{
    public string monsterID;
    public MonsterType monsterType;
    public int level;
    public int currentHP;
    public int maxHP;

    //Variables de gestion de la party y la reserva que deben persistir entre partidas
    public bool isLocked;
    public bool isFavorite;

    //ID de los Basic Moves aprendidos
    public List<string> learnedBasicMoveIDs = new List<string>();
    //ID de los Essence Moves aprendidos
    public List<string> learnedEssenceMoveIDs = new List<string>();

    //Variable para saber en que slot se encontraba el Monster
    public int slotIndex;
}
