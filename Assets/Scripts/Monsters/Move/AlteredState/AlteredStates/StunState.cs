using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "StunState", menuName = "AlteredStates/Stun")]
public class StunState : AlteredState
{
    public void Awake()
    {
        stateId = "stun";
        stateName = "Stun";
        timing = ModifierTiming.OnTurnStart;
        stackable = true;
    }

    public override AlteredStateInstance CreateInstance(int intensity, int duration)
    {
        return new StunInstance(this, intensity, duration);
    }
}

public class StunInstance : AlteredStateInstance
{
    //Creamos el consturctor
    public StunInstance(AlteredState data, int intensity, int duration) : base(data, intensity, duration){}

    public override void OnApply(Monster monster)
    {
        //Lanzamos el mensaje de aplicar estado alterado de Feedback en combate
        CombatLogHelper.Raise(monster.data.MonsterName + " queda " + stateNameAdjective + " durante " + duration, CombatLogType.Status );
    }

    public override void OnRemove(Monster monster)
    {
        // Al expirar nos aseguramos de limpiar el flag
        monster.actionBlocked = false;
        //Lanzamos el mensaje de que se ha quitado el estado alterado
        CombatLogHelper.Raise(monster.data.MonsterName + " ya no está " + stateNameAdjective, CombatLogType.Status);
    }

    public override bool OnTick(Monster monster)
    {
        // Activamos el flag para bloquear el turno
        monster.actionBlocked = true;

        duration--;

        //Lanzamos el mensaje de el tick del estado alterado
        CombatLogHelper.Raise(stateNameAdjective + monster.data.MonsterName + " Turnos restantes: " + duration, CombatLogType.Status);

        // Si expira limpiamos el flag
        if (duration <= 0)
        {
            monster.actionBlocked = false;
            return true;
        }

        return false;
    }
}
