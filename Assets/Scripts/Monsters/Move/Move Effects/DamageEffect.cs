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

    //Resultado completo del calculo de daño, para que Execute pueda construir el mensaje de log sin recalcular nada
    private struct DamageResult
    {
        public int damage;
        public bool isCritical;
        //typeMultiplier: 0 = inmune, <1 = resistencia, >1 = debilidad, =1 = neutral
        public float typeMultiplier;
    }

    //Hacemos override de Execute funcion que hereda de Move Effect
    public override IEnumerator Execute(MonsterUnit user, List<MonsterUnit> targets, MoveData move)
    {
        //Lista de fragmentos de log, uno por target, para poder agrupar por coma si hay mas de uno (AOE)
        List<string> logFragments = new List<string>();

        //Por cada target del Move
        foreach(var target in targets)
        {
            //Lanzamos la ejecucion para calcular el daño
            DamageResult result = CalculateDamage(user.monster, target.monster, move);
            //El target actual del move recibe el daño
            target.monster.TakeDamage(result.damage);
            //Construimos el fragmento de log de este target, combinando tipo especial (resistencia/debilidad) + linea de daño en un unico string
            logFragments.Add(BuildLogFragment(target.monster, move, result));
            //Esperamos medio segundo para que de la sensacion de aplicarse el efect
            yield return new WaitForSeconds(0.5f);
        }

        //Emitimos el log: un fragmento si hay un solo target, agrupados por coma si hay varios (AOE)
        CombatLogHelper.RaiseGrouped(logFragments, CombatLogType.Damage);
    }

    //Construye el texto final a mostrar en el Combat Feedback Panel para un target concreto
    private string BuildLogFragment(Monster target, MoveData move, DamageResult result)
    {
        string typeLine = "";

        //Inmune (0)
        if (result.typeMultiplier <= 0f)
        {
            return target.data.MonsterName + " es inmune a " + move.DamageType + ".";
        }
        //Resistente (0.5)
        else if (result.typeMultiplier < 1f)
        {
            typeLine = target.data.MonsterName + " es resistente a " + move.DamageType + ".\n";
        }
        //Debil (x2)
        else if (result.typeMultiplier > 1f)
        {
            typeLine = target.data.MonsterName + " es débil a " + move.DamageType + ".\n";
        }

        //Si es critico
        string critPrefix = result.isCritical ? "¡Golpe crítico! " : "";
        //El daño calculado
        string damageLine = critPrefix + target.data.MonsterName + " recibe " + result.damage + " de daño";

        //Devolvemos el log del tipo + el log del daño
        return typeLine + damageLine;
    }

    //Funcion para calcular el daño, necesita recibir el monster que ataca, el que recibe el daño y que move se ejecuta
    private DamageResult CalculateDamage(Monster attacker, Monster defender, MoveData move)
    {
        // Elegimos ataque y defensa segun la categoria del move
        float attack  = move.Category == MoveCategory.Physical ? attacker.currentAttack : attacker.currentSpecialAttack;
        float defense = move.Category == MoveCategory.Physical ? defender.currentDefense : defender.currentSpecialDefense;

        //Formula base para el daño (Attack * Power / Defense)
        float baseDamage = (attack * move.Power / defense);

        //Multiplicador de tipo (tabla de tipos), se va a la clase de la tabla de tipos y recorre el diccionario para saber el multiplicador correspondiente
        float typeMultiplier = TypeChart.GetMultiplier(move.DamageType, defender.data.Type);

        //Si el tipo es inmune (multiplicador 0), el daño es 0 sin excepcion
        if (typeMultiplier <= 0f) return new DamageResult { damage = 0, isCritical = false, typeMultiplier = typeMultiplier };

        //STAB: bonus si el tipo del move coincide con el tipo del atacante, si el tipo del move coincide con el del attacker devuelve stab multiplier, si no devuelve 1
        float stab = move.DamageType == attacker.data.Type ? stabMultiplier : 1f;
        
        //Generamos un numero random y si es menor que critChance guardamos critMultiplier en la variable, si no guardamos 1
        bool isCritical = Random.value < critChance;
        float crit = isCritical ? critMultiplier : 1f;

        //Variacion aleatoria entre 0.85 y 1f
        float variance = Random.Range(0.85f, 1f);

        //Calculamos el Multiplier (tipo * STAB * Crititico * Variacion)
        float Multiplier = typeMultiplier * stab * crit * variance;

        //Daño final redondeado, minimo 1
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * Multiplier));

        return new DamageResult { damage = finalDamage, isCritical = isCritical, typeMultiplier = typeMultiplier };
    }
}
