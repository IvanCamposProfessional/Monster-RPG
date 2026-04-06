using System.IO;
using JetBrains.Annotations;
using UnityEngine;

public class SaveSystem
{
    //Variable para guardar la ruta de guardado donde irá el JSON
    private static string SavePath => Application.persistentDataPath + "/SaveData/";
    //Variable para definir la cantidad maxima de slots de guardado
    private const int MAX_SLOTS = 3;

    //Devuelve la ruta del archivo de un slot
    private static string GetSlotPath(int slot)
    {
        return SavePath + "slot" + slot + ".json";
    }

    //Guarda el PlayerData en el slot indeicado
    public static void Save(PlayerData data, int slot)
    {
        //Creamos la carpeta si no existe
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }

        //Creamos el archivo JSON con la data que le pasamos del Player
        string json = JsonUtility.ToJson(data, true);
        //Escribimos el JSON en el Path del Slot
        File.WriteAllText(GetSlotPath(slot), json);
        Debug.Log("Partida guardada en slot " + slot);
    }

    //Carga el PlayerData en el Slot indicado
    public static PlayerData Load(int slot)
    {
        string path = GetSlotPath(slot);

        //Si el JSON de guardado no existe hacemos devolvemos nulo
        if (!File.Exists(path))
        {
            Debug.Log("No hay partida guardada en slot " + slot);
            return null;
        }

        //Guardamos el JSON del path del slot que le pasamos y lo devolvemos
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<PlayerData>(json);
    }

    //Funcion que devuelve true si el slot tiene partida guardada
    public static bool SlotExists(int slot)
    {
        return File.Exists(GetSlotPath(slot));
    }

    //Elimina la partida de un slot
    public static void DeleteSlot(int slot)
    {
        //Guardas el Path del JSON
        string path = GetSlotPath(slot);
        //Si el archivo JSON existe
        if (File.Exists(path))
        {
            //Lo elimina
            File.Delete(path);
            Debug.Log("Slot " + slot + " eliminado");
        }
    }

    //Devuelve un resumen de todos los slots para la pantalla de seleccion
    public static SlotInfo[] GetAllSlotsInfo()
    {
        //Creamos un array de SlotInfo con el tamaño maximo de Slots
        SlotInfo[] slots = new SlotInfo[MAX_SLOTS];

        //Recorremos el array de slots info
        for(int i = 0; i < MAX_SLOTS; i++)
        {
            //Si hay un Slot creado
            if (SlotExists(i))
            {
                //Creamos una variable Player Data y cargamos el JSON
                PlayerData data = Load(i);
                //Inicializamos la Slot Info con la info del JSON en data
                slots[i] = new SlotInfo
                {
                    slot = i,
                    isEmpty = false,
                    playerName = data.playerName,
                    playTime = data.playTime
                };
            }
            //Si no hay ningun archivo JSON guardado en el Slot
            else
            {
                //Inicializamos la Slot Info vacia
                slots[i] = new SlotInfo
                {
                    slot = i,
                    isEmpty = true
                };
            }
        }

        //Una vez inicializada devolvemos la lista de slots
        return slots;
    }
}
