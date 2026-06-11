using System.Linq;
using UnityEngine;

//Clase estatica que centraliza toda la conversion entre Monster (runtime) y MonsterSaveData
public static class MonsterSerializer
{
    // ─────────────────────────────────────────
    // CREACION (para invocacion de nuevos monsters)
    // ─────────────────────────────────────────
    
    //Crea un MonsterSaveData nuevo desde un MonsterData con valores de inicio
    //Aprende automaticamente los Moves disponibles en nivel 1
    public static MonsterSaveData CreateNew(MonsterData data)
    {
        //Creamos un nuevo Monster Save Data
        MonsterSaveData save = new MonsterSaveData();
        //Guardamos la ID del Monster Save Data con la del Monster Data que le pasamos
        save.monsterID = data.MonsterID;
         //Guardamos el tipo del Monster para que sistemas puros puedan acceder sin consultar la base de datos
        save.monsterType = data.Type;
        //Ponemos el Level del Monster Save Data a 1
        save.level = 1;

        //HP iniciales son el valor maximo
        save.currentHP = data.BaseHP + 1 * 5;
        save.maxHP = data.BaseHP + 1 * 5;

        //Añadimos los Basic Moves que se aprenden en nivel 1 via LerneableMove
        foreach(LerneableMove lerneableMove in data.LerneableMoves)
        {
            if (lerneableMove.LevelLearned > 1 || lerneableMove.Move == null) continue;
            save.learnedBasicMoveIDs.Add(lerneableMove.Move.MoveID);
        }

        //Devolvemos el Monster Save Data que hemos creado
        return save;
    }

    // ─────────────────────────────────────────
    // SERIALIZAR (Monster runtime → MonsterSaveData)
    // ─────────────────────────────────────────

    //Convierte un Monster en runtime a su representacion serializable para guardar
    public static MonsterSaveData Serialize(Monster monster)
    {
        //Creamos el Monster Save Data que queremos guardar
        MonsterSaveData save = new MonsterSaveData();
        //Guardamos la ID del Monster Save Data con la del Monster que le pasamos
        save.monsterID = monster.data.MonsterID;
        //Guardamos el tipo del Monster para que sistemas puros puedan acceder sin consultar la base de datos
        save.monsterType = monster.data.Type;
        //Guardamos en el Monster Save Data el Level, la HP y la BP del Monster
        save.level = monster.level;
        save.currentHP = monster.currentHP;
        save.maxHP = monster.maxHP;

        //Guardamos los IDs de los Basic Moves aprendidos
        save.learnedBasicMoveIDs = monster.learnedBasicMoves.Where(m => m != null && !string.IsNullOrEmpty(m.MoveID)).Select(m => m.MoveID).ToList();

        //Devolvemos el Monster Save Data que hemos creado
        return save;
    }

    // ─────────────────────────────────────────
    // DESERIALIZAR (MonsterSaveData → Monster runtime)
    // ─────────────────────────────────────────

    //Reconstruye un Monster en runtime desde su MonsterSaveData usando las bases de datos
    public static Monster Deserialize(MonsterSaveData save, MonsterDatabase monsterDatabase, MoveDatabase moveDatabase, EssenceRuneDatabase essenceRuneDatabase)
    {
        //Buscamos la MonsterData en la base de datos
        MonsterData data = monsterDatabase.GetMonsterByID(save.monsterID);

        //Comprobacion de nulo
        if (data == null)
        {
            Debug.LogWarning("MonsterSerializer: MonsterData no encontrada para ID: " + save.monsterID);
            return null;
        }

        //Creamos el Monster con los valores guardados
        Monster monster = new Monster(data, save.level, save.currentHP);

        //Resolvemos y añadimos los Basic Moves desde la base de datos
        foreach (string moveID in save.learnedBasicMoveIDs)
        {
            MoveData move = moveDatabase.GetMoveByID(moveID);
            
            if(move != null)
                monster.learnedBasicMoves.Add(move);
            else
                Debug.LogWarning("MonsterSerializer: MoveData no encontrada para ID: " + moveID);
        }

        //Resolvemos y añadimos los Essence Moves desde la base de datos
        foreach(string runeID in save.equippedRuneIDs)
        {
            if (string.IsNullOrEmpty(runeID)) continue;
            EssenceRune rune = essenceRuneDatabase.GetRuneByID(runeID);
            if (rune != null && rune.MoveData != null)
                monster.learnedEssenceMoves.Add(rune.MoveData);
            else
                Debug.LogWarning("MonsterSerializer: Rune no encontrada para ID: " + runeID);
        }

        //Devolvemos el Monster que hemos creado
        return monster;
    }
}
