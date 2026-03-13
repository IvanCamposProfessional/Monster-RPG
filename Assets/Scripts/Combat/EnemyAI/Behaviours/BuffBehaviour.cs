using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffBehaviour", menuName = "AI/BuffBehaviour")]
public class BuffBehaviour : AIBehaviour
{
    //Variable en la que definimos el threshold para que se lance este behaviour
    [Range(0f, 1f)]
    public float hpThreshold = 0.7f;

    public override bool CanExecute(MonsterUnit enemy, List<MonsterUnit> allyTargets)
    {
        //Comprobamos si tiene mas del HP del umbral
        float hpPercent = (float)enemy.monster.currentHP / enemy.monster.maxHP;
        //Si el procentaje es menor que el umbral devolvemos false ya que se tiene que ejecutar cuando tenga mas del 70% de vida
        if(hpPercent < hpThreshold) return false;

        //Buscamos un move con ApllyModifierEffect de tipo Buff
        MoveData buffMove = enemy.monster.learnedMoves.FirstOrDefault(m =>
        {
            //Si no tiene el effect de Apply Modifier devuelve false
            if(!MoveHasEffect<ApplyModifierEffect>(m)) return false;
            //Guardamos el effect para compararlo
            ApplyModifierEffect effect = m.Effects.OfType<ApplyModifierEffect>().First();
            //Devuelve true o false segun si es buff o no
            return effect.modifierType == ModifierType.Buff;
        });

        //Si no se ha guardado ningun MoveData quiere decir que no tiene ningun ataque que pueda dar buff y devuelve false
        if(buffMove == null) return false;

        //Comprobamos si ya tiene el buff activo
        ApplyModifierEffect buffEffect = buffMove.Effects.OfType<ApplyModifierEffect>().First();
        //Comprueba si en la lista de stat modifiers del monster contiene alguno con el mismo id que el id del buff effect guardado lo que significa que el monster ya lo tiene aplicado
        bool alreadyBuffed = enemy.monster.statModifiers.Any(s => s.modifierId == buffEffect.statModifier.modifierId);
        //Si ya tiene el buff activo y no es stackeable devuelve false
        if(alreadyBuffed && !buffEffect.statModifier.stackable) return false;

        //Si pasa de todas las comprobaciones anteriores sin devolver false quiere decir que si que se puede ejecutar este behaviour y devuelve true
        return true;
    }

    public override AIDecision Execute(MonsterUnit enemy, List<MonsterUnit> allyTargets)
    {
        //Guarda el primer movimiento que contenga un ApplyModifierEffect de tipo Buff
        MoveData buffMove = enemy.monster.learnedMoves.First(m =>
        {
            //Comoprobacion de seguridad aunque si llega a Execute ya debe haber comprobado que tenga un Move que lo pueda aplicar
            if(!MoveHasEffect<ApplyModifierEffect>(m)) return false;
            //Guarda el efecto del Move que es Apply Modifier
            ApplyModifierEffect effect = m.Effects.OfType<ApplyModifierEffect>().First();
            //Devuelve true o false segun si es buff o no
            return effect.modifierType == ModifierType.Buff;
        });

        //Devuelve el Move tipo Buff y el target que es el propio Enemy
        return new AIDecision(buffMove, new List<MonsterUnit> { enemy });
    }
}
