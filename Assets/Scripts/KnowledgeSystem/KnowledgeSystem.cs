using UnityEngine;

public class KnowledgeSystem
{
    //Variable para almacenar el Save Data
    private KnowledgeSaveData data;
    //Variable para almacenar la base de datos de Monsters
    private MonsterDatabase monsterDatabase;

    //Creamos el constructor
    public KnowledgeSystem(KnowledgeSaveData data, MonsterDatabase monsterDatabase)
    {
        this.data = data;
        this.monsterDatabase = monsterDatabase;
    }

    // ─────────────────────────────────────────
    // WORLD KNOWLEDGE
    // ─────────────────────────────────────────

    //Creamos una funcion para añadir conocimiento del mundo al cual le pasamos la flag que queremos añadir
    public void AddWorldKnowledge(string flag)
    {
        //Si el Save Data no contiene la flag que le pasamos
        if (!data.worldFlags.Contains(flag))
        {
            //Añadimos la flag
            data.worldFlags.Add(flag);
            Debug.Log("WorldKnowledge desbloqueada: " + flag);
        }
    }

    //Funcion para asaber si la data contiene una flag
    public bool HasWorldKnowledge(string flag)
    {
        return data.worldFlags.Contains(flag);
    }

    // ─────────────────────────────────────────
    // NPC KNOWLEDGE
    // ─────────────────────────────────────────

    //Creamos una funcion para añadir conocimiento del NPC al cual le pasamos la flag que queremos añadir
    public void AddNPCKnowledge(string flag)
    {
        //Si el Save Data no contiene la flag que le pasamos
        if (!data.npcFlags.Contains(flag))
        {
            //Añadimos la flag
            data.npcFlags.Add(flag);
            Debug.Log("NPCKnowledge desbloqueada: " + flag);
        }
    }

    //Funcion para asaber si la data contiene una flag
    public bool HasNPCKnowledge(string flag)
    {
        return data.npcFlags.Contains(flag);
    }

    // ─────────────────────────────────────────
    // MONSTER KNOWLEDGE
    // ─────────────────────────────────────────
    
    //Funcion que se lanza cuando se descubre un monster (ya sea si has oido hablar de el) por primera vez creando la entrada en nivel 0
    public void DiscoverMonster(string monsterID)
    {
        //Si ya existe la entry del monsterID que le pasamos salimos de la funcion
        if (GetEntry(monsterID) != null) return;

        data.monsterEntries.Add(new MonsterKnowledgeEntry(monsterID));
        Debug.Log("Monster registrado en el sistema: " + monsterID);
    }

    //Registra que el jugador ha combatido con el Monster por primera vez
    //Sube el nivel de conocimiento de monster a 1 si no lo habia encontrado antes
    public void RegisterMonsterEncountered(string monsterID)
    {
        EnsureEntry(monsterID);
        //Guardas la entrada del Knowledge Save Data del monster
        MonsterKnowledgeEntry entry = GetEntry(monsterID);

        //Si no te habias encontrado con este monstruo
        if (!entry.encountered)
        {
            //Registra que te has encontrado con el monster
            entry.encountered = true;
            //Sube el Knowledge Level a 1
            entry.knowledgeLevel = 1;
            Debug.Log("Monster " + monsterID + " encontrado por primera vez, nivel de conocimiento 1");
        }
    }

    //Registra una derrota y comprueba si el Knowledge Level sube de nivel
    public void RegisterMonsterDefeated(string monsterID)
    {
        EnsureEntry(monsterID);
        //Guardas la entrada del Knowledge Save Data del monster
        MonsterKnowledgeEntry entry = GetEntry(monsterID);

        //Si no lo habiasd encontrado lo marcamos como encontrado
        if (!entry.encountered)
        {
            RegisterMonsterEncountered(monsterID);
        }

        //Guardamos que lo has derrotado
        entry.timesDefeated++;
        Debug.Log("Monster " + monsterID + " derrotado " + entry.timesDefeated + " veces");

        //Checkeamos si el Knowledge Level sube de nivel
        CheckKnowledgeLevelUp(monsterID);
    }

    // Registra una invocacion y comprueba si sube de nivel
    public void RegisterMonsterSummoned(string monsterID)
    {
        EnsureEntry(monsterID);
        //Guardas la entrada del Knowledge Save Data del monster
        MonsterKnowledgeEntry entry = GetEntry(monsterID);

        //Guardamos que lo has invocado
        entry.timesSummoned++;
        Debug.Log("Monster " + monsterID + " invocado " + entry.timesSummoned + " veces");

        //Checkeamos si el Knowledge Level sube de nivel
        CheckKnowledgeLevelUp(monsterID);
    }

    //Registra que el monster ha subido al nivel maximo
    public void RegisterMonsterMaxleveled(string monsterID)
    {
        EnsureEntry(monsterID);
        //Guardas la entrada del Knowledge Save Data del monster
        MonsterKnowledgeEntry entry = GetEntry(monsterID);

        //Si el monster no se habia maxeado de level antes
        if(!entry.maxLeveled)
        {
            
            entry.maxLeveled = true;
            Debug.Log("Monster " + monsterID + " ha alcanzado el nivel maximo");

            //Forzamos la comprobacion para que suba a nivel 3 inmediatamente
            CheckKnowledgeLevelUp(monsterID);
            
        }
    }

    //Funcion para traer la ifnromacion de un Monster Knowledge
    public MonsterKnowledgeEntry GetMonsterKnowledge(string monsterID)
    {
        EnsureEntry(monsterID);
        return GetEntry(monsterID);
    }

    //Devuelve el nivel de conocimeinto de un monster SIN crear una entrada si no existe
    //Devuelve 0 si el monster es completamente desconocido
    public int GetKnowledgeLevel(string monsterID)
    {
        MonsterKnowledgeEntry entry = GetEntry(monsterID);
        //Devuelve el knowledge level de la entry, si es null devuelve 0
        return entry?.knowledgeLevel ?? 0;
    }

    //Funcion para saber si el player tiene un monster knowledge
    public bool HasMonsterKnowledge(string monsterID)
    {
        return GetEntry(monsterID) != null;
    }

    // ─────────────────────────────────────────
    // PRIVADOS
    // ─────────────────────────────────────────

    //Funcion que devuelve una monster entry buscando en data.monsterEntries el ID que le pasamos
    private MonsterKnowledgeEntry GetEntry(string monsterID)
    {
        return data.monsterEntries.Find(e => e.monsterID == monsterID);
    }

    //Funcion de seguridad para asegurarnos que existe una entrada en el sistema para un monster antes de operar con el (hacerlo antes de GetEntry para que no devuelva null)
    private void EnsureEntry(string monsterID)
    {
        //Si la entry no existe
        if(GetEntry(monsterID) == null)
        {
            //Creamos la Entry del monster a nivel 0
            DiscoverMonster(monsterID);
        }
    }

    //Funcion para checkear el Level UP del Knowledge del monster
    private void CheckKnowledgeLevelUp(string monsterID)
    {
        //Guardamos la entry del monster
        MonsterKnowledgeEntry entry = GetEntry(monsterID);
        //Guardamos la monster data del monster de la entry
        MonsterData monsterData = monsterDatabase.GetMonsterByID(monsterID);

        //Checkeo de seguridad para asegurarnos que la monster data existe
        if(monsterData == null) return;

        // Comprobamos subida a nivel 2
        // Solo puede subir a nivel 2 si ya esta en nivel 1 (encountered)
        if(entry.knowledgeLevel == 1)
        {
            //Variable para saber si has derrotado las suficientes veces al monster
            //Guarda true si las veces que has derrotado al monster de la entry es mayor o igual a las veces definidas en la monster data que hay que derrotarlo
            bool defeatedEnough = entry.timesDefeated >= monsterData.timesDefeatedForLevel2;
            //Variable para saber si has invocado las suficientes veces al monster
            //Guarda true si las veces que has invocado al monster de la entry es mayor o igual a las veces definidas en la monster data que hay que invocarlo
            bool summonedEnough = entry.timesSummoned >= monsterData.timesSummonedForLevel2;

            //Si defeatedEnouth y summonedEnough son true
            if(defeatedEnough || summonedEnough)
            {
                //Subes el knowledgeLevel de la Entry a 2
                entry.knowledgeLevel = 2;
                Debug.Log("Monster " + monsterID + " nivel de conocimiento 2");
            }
        }

        //Comprobamos la subida a nivel 3
        if(entry.knowledgeLevel == 2)
        {
            //Variable para saber si has derrotado las suficientes veces al monster
            //Guarda true si las veces que has derrotado al monster de la entry es mayor o igual a las veces definidas en la monster data que hay que derrotarlo
            bool defeatedEnough = entry.timesDefeated >= monsterData.timesDefeatedForLevel3;

            //Si defeatedEnough y maxLeveled
            if(defeatedEnough || entry.maxLeveled)
            {
                //Subes el knowledgeLevel de la Entry a 3
                entry.knowledgeLevel = 3;
                Debug.Log("Monster " + monsterID + " nivel de conocimiento 3");
            }
        }
    }
}
