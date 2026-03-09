using UnityEngine;

//Clase abstracta para poder heredar de ella y crear Modifiers
public abstract class Modifier
{
    public string Id;
    public ModifierType modifierType;
    //Indica los turnos restantes
    public int duration;
    //Variable para definir si se puede stackear
    public bool stackable;
    public ModifierTiming timing;

    //Se llama cuando se aplica el Modifier al Monster y se le pasa el monster al que se le ha aplicado
    public abstract void OnApply(Monster monster);

    //Se llama cuando expira o se elimina y se le pasa el monster que estaba afectado
    public abstract void OnRemove(Monster monster);

    //Se llama cada turno segun el Timing seleccionado (inicio o fin de turno) y se le pasa el monster que está afectado
    //Devuelve true o false si el modifier debe eliminarse tras el tick
    public abstract bool OnTick(Monster monster);
}
