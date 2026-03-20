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
        Debug.Log(monster.data.MonsterName + " ha sido stunneado por " + duration + " turnos");
    }

    public override void OnRemove(Monster monster)
    {
        // Al expirar nos aseguramos de limpiar el flag
        monster.actionBlocked = false;
        Debug.Log(monster.data.MonsterName + " ya no está stunneado");
    }

    public override bool OnTick(Monster monster)
    {
        // Activamos el flag para bloquear el turno
        monster.actionBlocked = true;

        duration--;

        // Si expira limpiamos el flag
        if (duration <= 0)
        {
            monster.actionBlocked = false;
            return true;
        }

        return false;
    }
}
