using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackBehaviour", menuName = "AI/AttackBehaviour")]
public class AttackBehaviour : AIBehaviour
{
    public override bool CanExecute(MonsterUnit enemy, List<MonsterUnit> allyTargets)
    {
        //Devuelve un target aleatorio que esté vivo y un move que tenga el damage effect
        return allyTargets.Any(u => u.IsAlive) && enemy.monster.learnedMoves.Any(m => MoveHasEffect<DamageEffect>(m));
    }

    public override AIDecision Execute(MonsterUnit enemy, List<MonsterUnit> allyTargets)
    {
        //Creamos una lista con los aliados vivos
        List<MonsterUnit> livingAllies = allyTargets.Where(u => u.IsAlive).ToList();
        //Creamos una lista con los moves que tienen efecto de daño
        List<MoveData> damageMoves = enemy.monster.learnedMoves.Where(m => MoveHasEffect<DamageEffect>(m)).ToList();

         // Paso 1: buscar combinacion move con STAB + ventaja x2 contra el ally con menos HP
         //Recorremos la lista de monster unit aliados vivos y los ordenamos por current HP
         foreach(var ally in livingAllies.OrderBy(u => u.monster.currentHP))
        {
            //Guardamos el primer move que detecte que tiene stab (el tipo del movimiento == al tipo del monster) y guarda primero el que tenga x2 recorriendo el TypeChart y buscando el tipo del ally monster target
            MoveData stabAdvantageMove = damageMoves.Where(m => m.MoveType == enemy.monster.data.Type).FirstOrDefault(m => TypeChart.GetMultiplier(m.MoveType, ally.monster.data.Type) == 2f);
            //Si ha guardado algun movimiento que tenga stab
            if (stabAdvantageMove != null)
            {
                //Devuelve el move que va a ejecutar y a que monster unit
                return new AIDecision(stabAdvantageMove, new List<MonsterUnit> { ally });
            }
        }

        // Paso 2: cualquier move con x2 contra el ally con menos HP aunque no haya STA
        //Recorremos la lista de monster unit aliados vivos y los ordenamos por current HP
         foreach(var ally in livingAllies.OrderBy(u => u.monster.currentHP))
        {
            //Guardamos el que tenga x2 recorriendo el TypeChart y buscando el tipo del ally monster target
            MoveData advantageMove = damageMoves.FirstOrDefault(m => TypeChart.GetMultiplier(m.MoveType, ally.monster.data.Type) == 2f);
            //Si ha guardado algun movimiento que sea x2
            if (advantageMove != null)
            {
                //Devuelve el move que va a ejecutar y a que monster unit
                return new AIDecision(advantageMove, new List<MonsterUnit> { ally });
            }
        }

        // Paso 3: sin ventaja de tipo, ataca al ally con menos HP con cualquier move de daño
        MonsterUnit weakestAlly = livingAllies.OrderBy(u => u.monster.currentHP).First();
        MoveData anyDamageMove = damageMoves.First();

        return new AIDecision(anyDamageMove, new List<MonsterUnit> { weakestAlly });
    }
}
