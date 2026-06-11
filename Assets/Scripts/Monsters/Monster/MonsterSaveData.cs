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

    public const int MAX_RUNE_SLOTS = 5;
    //IDs de las Essence Runes equipadas en este Monster (maximo 5)
    public List<string> equippedRuneIDs = new List<string>(new string[MAX_RUNE_SLOTS]);

    //Variable para saber en que slot se encontraba el Monster
    public int slotIndex;
}
