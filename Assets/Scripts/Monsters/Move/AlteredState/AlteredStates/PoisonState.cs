using UnityEngine;

//Creamos el altered state de Poison que hereda de AlteredState y definimos el Asset Menu para crearlo
[CreateAssetMenu(fileName = "PoisonState", menuName = "AlteredStates/Poison")]
public class PoisonState : AlteredState
{
    //Indicamos en Awake que es poison, el timing y si es stackable
    private void Awake()
    {
        stateId = "poison";
        stateName = "Poison";
        timing = ModifierTiming.OnTurnStart;
        stackable = true;
    }

    //Creamos la instance del Altered State
    public override AlteredStateInstance CreateInstance(int intensity, int duration)
    {
        return new PoisonInstance(this, intensity, duration);
    }
}

//Creamos la instancia de poison que hereda de Altered State Instance
public class PoisonInstance : AlteredStateInstance
{
    //Creamos el constructor de la instancia
    public PoisonInstance(AlteredState data, int intensity, int duration) : base(data, intensity, duration){}

    public override void OnApply(Monster monster)
    {
        // Sincronizamos duracion con intensidad ya que en el veneno son iguales
        duration = intensity;
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
        // El daño es igual a la intensidad actual
        monster.TakeDamage(intensity);

        // Reducimos tanto intensidad como duracion juntas
        intensity--;
        duration--;

        //Lanzamos el mensaje de el tick del estado alterado
        CombatLogHelper.Raise("El veneno inflige " + intensity + " de daño a " + monster.data.MonsterName + " Turnos restantes: " + duration, CombatLogType.Status);

        return duration <= 0;
    }
}