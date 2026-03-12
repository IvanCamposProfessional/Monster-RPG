using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Effects/Damage")]
//Esta clase de effecto hereda de MoveEffect
public class DamageEffect : MoveEffect
{
    //Probabilidad de critico de 0 a 1 en el que 0.1 significa el 10%, es una constante
    private const float critChance = 0.1f;
    //Multiplicador de critico, tambien es una constante
    private const float critMultiplier = 1.5f;
    //STAB bonus constante
    private const float stabMultiplier = 1.5f;

    //Hacemos override de Execute funcion que hereda de Move Effect
    public override IEnumerator Execute(MonsterUnit user, List<MonsterUnit> targets, MoveData move)
    {
        //Por cada target del Move
        foreach(var target in targets)
        {
            //Lanzamos la ejecucion para calcular el daño
            int damage = CalculateDamage(user.monster, target.monster, move);
            //El target actual del move recibe el daño
            target.monster.TakeDamage(damage);
            Debug.Log(user.monster.data.MonsterName + " hace " + damage + " de daño a " + target.monster.data.MonsterName);
            //Esperamos medio segundo para que de la sensacion de aplicarse el efect
            yield return new WaitForSeconds(0.5f);
        }
    }

    //Funcion para calcular el daño, necesita recibir el monster que ataca, el que recibe el daño y que move se ejecuta
    private int CalculateDamage(Monster attacker, Monster defender, MoveData move)
    {
        // Elegimos ataque y defensa segun la categoria del move
        float attack  = move.Category == MoveCategory.Physical ? attacker.currentAttack : attacker.currentSpecialAttack;
        float defense = move.Category == MoveCategory.Physical ? defender.currentDefense : defender.currentSpecialDefense;

        //Formula base para el daño (Attack * Power / Defense)
        float baseDamage = (attack * move.Power / defense);

        //Multiplicador de tipo (tabla de tipos), se va a la clase de la tabla de tipos y recorre el diccionario para saber el multiplicador correspondiente
        float typeMultiplier = TypeChart.GetMultiplier(move.MoveType, defender.data.Type);

        //STAB: bonus si el tipo del move coincide con el tipo del atacante, si el tipo del move coincide con el del attacker devuelve stab multiplier, si no devuelve 1
        float stab = move.MoveType == attacker.data.Type ? stabMultiplier : 1f;
        
        //Generamos un numero random y si es menor que critChance guardamos critMultiplier en la variable, si no guardamos 1
        float crit = Random.value < critChance ? critMultiplier : 1f;
        if (crit > 1f) Debug.Log("¡Golpe crítico!");

        //Variacion aleatoria entre 0.85 y 1f
        float variance = Random.Range(0.85f, 1f);

        //Calculamos el Multiplier (tipo * STAB * Crititico * Variacion)
        float Multiplier = typeMultiplier * stab * crit * variance;

        //Daño final redondeado, minimo 0
        return Mathf.Max(0, Mathf.RoundToInt(baseDamage * Multiplier));
    }
}
