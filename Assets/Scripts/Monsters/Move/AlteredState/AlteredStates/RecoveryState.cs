using UnityEngine;

//Creamos el altered state de Recovery que hereda de AlteredState y definimos el Asset Menu para crearlo
[CreateAssetMenu(fileName = "RecoveryState", menuName = "AlteredStates/Recovery")]
public class RecoveryState : AlteredState
{
    //Indicamos en Awake que es recovery, el timing y si es stackable
    private void Awake()
    {
        stateId = "recovery";
        stateName = "Recovery";
        timing = ModifierTiming.OnTurnEnd;
        stackable = true;
    }

    //Creamos la instance del Altered State
    public override AlteredStateInstance CreateInstance(int intensity, int duration)
    {
        return new RecoveryInstance(this, intensity, duration);
    }
}

//Creamos la instancia de poison que hereda de Altered State Instance
public class RecoveryInstance : AlteredStateInstance
{
    //Creamos el constructor de la instancia
    public RecoveryInstance(AlteredState data, int intensity, int duration) : base(data, intensity, duration) {}

    public override void OnApply(Monster monster)
    {
        //Lanzamos el mensaje de aplicar estado alterado de Feedback en combate
        CombatLogHelper.Raise(monster.data.MonsterName + " queda " + stateNameAdjective + " durante " + duration, CombatLogType.Status );
    }

    public override void OnRemove(Monster monster)
    {
        //Lanzamos el mensaje de que se ha quitado el estado alterado
        CombatLogHelper.Raise(monster.data.MonsterName + " ya no está " + stateNameAdjective, CombatLogType.Status);
    }

    public override bool OnTick(Monster monster)
    {
        //La curación es igual a la intensidad actual
        monster.Heal(intensity);
        //Lanzamos el mensaje de el tick del estado alterado
        CombatLogHelper.Raise(monster.data.MonsterName + " se cura " + intensity + " Turnos restantes: " + duration, CombatLogType.Status);

        //Reducimos intensidad
        duration--;

        //Cuando la intensidad llega a 0 recovery expira
        return duration <= 0;
    }
}