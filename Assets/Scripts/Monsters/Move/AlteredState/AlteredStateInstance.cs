using UnityEngine;

//La instancia de Altered State no se crea como Sciptable Object ya que la crearemos por codigo
public abstract class AlteredStateInstance
{
    //Referencia al ScriptableObject original
    public AlteredState stateData;
    public int intensity;
    public int duration;
    //Creamos el timing de la instancia y lo definimos con el timing del altered state referenciado
    public ModifierTiming timing => stateData.timing;
    //Creamos el id de la instancia y lo definimos con el id del altered state referenciado
    public string stateId => stateData.stateId;
    //Creamos el icon y guardamos el icon de data
    public Sprite icon => stateData.icon;
    //Creamos el name del state y guardamos el de la data
    public string stateName => stateData.stateName;

    //Creamos el constructor
    protected AlteredStateInstance(AlteredState data, int intensity, int duration)
    {
        stateData = data;
        this.intensity = intensity;
        this.duration = duration;
    }

    //Creamos las funciones OnApply, OnRemove y OnTick a las que le pasaremos el monster al que afecta
    public abstract void OnApply(Monster monster);
    public abstract void OnRemove(Monster monster);
    //Devuelve true si debe eliminarse en este tick
    public abstract bool OnTick(Monster monster);
}
