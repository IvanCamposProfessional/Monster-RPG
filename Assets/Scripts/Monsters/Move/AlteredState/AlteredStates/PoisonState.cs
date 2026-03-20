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
        Debug.Log(monster.data.MonsterName + " ha sido envenenado con intensidad " + intensity);
    }

    public override void OnRemove(Monster monster)
    {
        Debug.Log(monster.data.MonsterName + " ya no está envenenado");
    }

    public override bool OnTick(Monster monster)
    {
        // El daño es igual a la intensidad actual
        monster.TakeDamage(intensity);
        Debug.Log(monster.data.MonsterName + " recibe " + intensity + " de daño por veneno");

        // Reducimos tanto intensidad como duracion juntas
        intensity--;
        duration--;

        return duration <= 0;
    }
}