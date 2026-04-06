using UnityEngine;

//Clase simple para mostrar el resumen de cada slot en la UI
public class SlotInfo
{
    public int slot;
    public bool isEmpty;
    public string playerName;
    public float playTime;

    //Convierte los segundos a formato legible
    public string GetFormattedPlayTime()
    {
        int hours = Mathf.FloorToInt(playTime / 3600f);
        int minutes = Mathf.FloorToInt((playTime % 3600f) / 60f); 
        return string.Format("{0:00}:{1:00}", hours, minutes);
    }
}
