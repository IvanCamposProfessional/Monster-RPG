using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum DelayType
{
    Fixed,
    Percentage
}

[CreateAssetMenu(fileName = "DelayEffect", menuName = "Effects/Delay")]
public class DelayEffect : MoveEffect
{
    public DelayType delayType;

    //Se usa cuando delayType es Fixed
    public float fixedDelay = 50f;

     //Se usa cuando delayType es Percentage, entre 0 y 1
     [Range(0f, 1f)]
     public float percentageDelay = 0.5f;

    public override IEnumerator Execute(MonsterUnit user, List<MonsterUnit> targets, MoveData move)
    {
        //Por cada target del move
        foreach(var target in targets)
        {
            //Si el Delay Type es fixed coge el valor puro que hemos definido al crearlo y si no hace el valor de Percentage y lo calcula con el progreso actual
            float delay = delayType == DelayType.Fixed ? fixedDelay : target.timelineProgress * percentageDelay;

            //Reseteamos el delay al timelineProgress, minimo 0
            target.timelineProgress = Mathf.Max(0f, target.timelineProgress - delay);

            //Notificamos que la timeline necesita refrescar sus iconos
            GameEvents.RaiseTimelineNeedsRefresh();

            //Lanzamos la Action de mensaje de feedback
            CombatLogHelper.Raise(target.monster.data.MonsterName + " es retrasado " + Mathf.RoundToInt(delay) + " puntos en el timeline", CombatLogType.Timeline);

            yield return new WaitForSeconds(0.3f);
        }
    }
}
