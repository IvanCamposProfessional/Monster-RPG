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
        //Ponemos el Level del Monster Save Data a 1
        save.level = 1;
        //HP y BP iniciales son el valor maximo (mismo calculo que Monster.CalculateMaxHP)
        save.currentHP = data.BaseHP + 1 * 5;
        save.currentBP = data.BaseBP + 1 * 5;

        //Añadimos los moves que se aprenden en nivel 1
        //Recorremos los Lerneable Moves dentro del Monster Data
        foreach(LerneableMove learneableMove in data.LerneableMoves)
        {
            //Si el nivel al que se aprende el move es 1 y Lerneable Move contiene el ataque (seguridad)
            if(learneableMove.LevelLearned <= 1 && learneableMove.Move != null)
            {
                //Guardamos el Move en el Monster Save Data
                save.learnedMoveIDs.Add(learneableMove.Move.MoveID);
            }
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
        //Guardamos en el Monster Save Data el Level, la HP y la BP del Monster
        save.level = monster.level;
        save.currentHP = monster.currentHP;
        save.currentBP = monster.currentBP;
        //Guardamos los IDs de los Moves aprendidos
        save.learnedMoveIDs = monster.learnedMoves.Where(m => m != null && !string.IsNullOrEmpty(m.MoveID)).Select(m => m.MoveID).ToList();
        //Devolvemos el Monster Save Data que hemos creado
        return save;
    }

    // ─────────────────────────────────────────
    // DESERIALIZAR (MonsterSaveData → Monster runtime)
    // ─────────────────────────────────────────

    //Reconstruye un Monster en runtime desde su MonsterSaveData usando las bases de datos
    public static Monster Deserialize(MonsterSaveData save, MonsterDatabase monsterDatabase, MoveDatabase moveDatabase)
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
        Monster monster = new Monster(data, save.level, save.currentHP, save.currentBP);

        //Resolvemos y añadimos los moves aprendidos desde la base de datos
        //Recorremos los Learned Moves IDs en el Monster Save Data
        foreach(string moveID in save.learnedMoveIDs)
        {
            //Creamos el move que queremos añadir al monster buscandolo en la base de datos de Moves
            MoveData move = moveDatabase.GetMoveByID(moveID);

            //Comprobacion de seguridad
            if (move != null)
            {
                monster.learnedMoves.Add(move);
            }
            else
            {
                Debug.LogWarning("MonsterSerializer: MoveData no encontrada para ID: " + moveID);
            }
        }

        //Devolvemos el Monster que hemos creado
        return monster;
    }
}
