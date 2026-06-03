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

            //Filtro 3: el primer tipo del Move debe estar presente en el equipo activo
            //Guarddamos la EssenceAmount del MoveData de la rune
            List<EssenceAmount> essenceToUse = rune.MoveData.EssenceAmountToUse;
            //Comprobacion de seguridad
            if (essenceToUse == null || essenceToUse.Count == 0) return false;

            //Guardamos el tipo principal del MoveData cogiendo la primera posicion de EssenceToUse
            MonsterType primaryType = essenceToUse[0].Type;
            return teamTypes.Contains(primaryType);

        }).ToList();
    }
}
