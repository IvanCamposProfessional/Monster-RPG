using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Asset menu para poder crear el scriptable object
[CreateAssetMenu(fileName = "Monster Data", menuName = "Scriptable Objects/Monster Data")]
public class MonsterData : ScriptableObject
{
    //Data base del Monster
    public string MonsterID;
    public string MonsterName;
    public string MonsterDescription;
    public MonsterType Type;
    public Sprite MonsterIcon;
    public Sprite MonsterSprite;
    public int BaseHP;
    public int BaseBP;
    public int BaseLevel;
    public int MaxLevel;
    public int BaseAttack;
    public int BaseDefense;
    public int BaseSpecialAttack;
    public int BaseSpecialDefense;
    public int BaseSpeed;

    public List<LerneableMove> LerneableMoves;

    public int timesDefeatedForLevel2;
    public int timesDefeatedForLevel3;
    public int timesSummonedForLevel2;
}
