using UnityEngine;

//Creamos la clase como ScriptableObject para crear los altered states desde el inspector
public abstract class AlteredState : ScriptableObject
{
    public string stateId;
    public string stateName;
    public ModifierTiming timing;
    public bool stackable;
    public Sprite icon;

    // Crea una instancia runtime del estado con la intensidad/duracion que le pasemos
    // Si dos monsters tienen X altered state cada uno tiene su propia intensidad y duracion independiente
    public abstract AlteredStateInstance CreateInstance(int intensity, int duration);
}
