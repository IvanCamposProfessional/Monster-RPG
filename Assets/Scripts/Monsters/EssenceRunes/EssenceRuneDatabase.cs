using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Base de datos de todas las EssenceRunes del juego, provee metodos de filtado para el sistema de Rewards
[CreateAssetMenu(fileName = "EssenceRuneDatabase", menuName = "Database/Essence Rune Database")]
public class EssenceRuneDatabase : ScriptableObject
{
    public List<EssenceRune> allRunes;

    //Devuelve una Rune por ID, null si no existe
    public EssenceRune GetRuneByID(string id)
    {
        return allRunes.Find(r => r.RuneID == id);
    }

    //Devuelve las Runes elegibles para un reward dando rareza, no debloqueada por el juigador y tipo principal
    public List<EssenceRune> GetElegibleRunes(List<RarityType> allowedRarities, List<string> unlockedRuneIDs, List<MonsterType> teamTypes)
    {
        return allRunes.Where(rune =>
        {
            //Comprobacion de seguridad: la Rune debe tener MoveData valida
            if (rune.MoveData == null) return false;

            //Filtro 1: rareza permitida por el nodo
            if(!allowedRarities.Contains(rune.Rarity)) return false;

            //Filtro 2: no desbloqueada todavia
            if(unlockedRuneIDs.Contains(rune.RuneID)) return false;

            //Guardamos el tipo principal del MoveData cogiendo la primera posicion de EssenceToUse
            MonsterType primaryType = rune.MainType;
            //Si MainType no pudo resolverse, descartamos la Rune
            if (primaryType == default) return false; 

            return teamTypes.Contains(primaryType);

        }).ToList();
    }
}
