using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    public MonsterData data;

    public int currentHP { get; set; }
    public int maxHP { get; private set; }
    public int currentBP;
    public int maxBP;
    public int level;
    public int currentSpeed { get; set; }
    //Los ataques que el monstruo actualmente sabe
    public List<MoveData> learnedMoves;

    public Monster(MonsterData data, int level, int currentHP, int currentBP){
        this.data = data;
        this.level = level;
        this.currentHP = currentHP;
        this.currentBP = currentBP;
        currentSpeed = data.BaseSpeed;
        maxHP = CalculateMaxHP();
        maxBP = CalculateMaxBP();
        //Inicializamos la lista de los Learned Moves
        learnedMoves = new List<MoveData>();
    }

    public bool IsAlive => currentHP > 0;

    int CalculateMaxHP(){
        return data.BaseHP + level * 5;
    }

    int CalculateMaxBP(){
        return data.BaseBP + level * 5;
    }

    //Funcion para que el Monster reciba daño, utilizamos Mathf.Max para que la vida nunca baje de 0
    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
    }

    //Funcion para que el Monster reciba curacion, utilizamos Mathf.Min para que la vida nunca suba del maximoq
    public void Heal(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }
}
