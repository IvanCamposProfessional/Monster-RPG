using System.Collections.Generic;
using UnityEngine;

public enum EssenceRuneCardState { Neutral, Equipable, Equipped, OtherMonster, Incompatible }

//Class pura que gestiona equipar y desequipar Essence Runes en Monsters
public class EssenceRuneSystem
{
    private EssenceRuneDatabase runeDatabase;
    private PlayerData playerData;

    public const int MAX_RUNE_SLOTS = MonsterSaveData.MAX_RUNE_SLOTS;

    //Creamos el constructor
    public EssenceRuneSystem(EssenceRuneDatabase runeDatabase, PlayerData playerData)
    {
        this.runeDatabase = runeDatabase;
        this.playerData = playerData;
    }

    // ─────────────────────────────────────────
    // CONSULTAS
    // ─────────────────────────────────────────

    public EssenceRuneCardState GetRuneCardState(string runeID, MonsterSaveData selectedMonster)
    {
        //Sin Monster seleccionado → neutro
        if (selectedMonster == null) return EssenceRuneCardState.Neutral;

        MonsterSaveData owner = FindRuneOwner(runeID);

        //Ya equipada en este Monster → azul
        if (owner != null && owner == selectedMonster) return EssenceRuneCardState.Equipped;

        //Ya equipada en otro Monster → naranja
        if (owner != null && owner != selectedMonster) return EssenceRuneCardState.OtherMonster;

        //Equipable en el Monster seleccionado → verde
        if (CanEquip(runeID, selectedMonster, out _)) return EssenceRuneCardState.Equipable;

        //No compatible con ningún Monster del equipo → rojo
        bool compatibleWithAny = false;
        foreach (MonsterSaveData monster in GetAllMonsters())
        {
            if (CanEquip(runeID, monster, out _)) { compatibleWithAny = true; break; }
        }
        if (!compatibleWithAny) return EssenceRuneCardState.Incompatible;

        //No equipable aquí pero sí en otro → neutro
        return EssenceRuneCardState.Neutral;

    }

    //Devuelve true si la Rune puede equiparse en el Monster indicado
    public bool CanEquip(string runeID, MonsterSaveData monster, out string reason)
    {
        //Guardamos la Essence Rune
        EssenceRune rune = runeDatabase.GetRuneByID(runeID);

        //Si no se encuentra la rune
        if (rune == null)
        {
            //Indica que la reason es que no se ha encontrado y devuelve false
            reason = "Rune no encontrada.";
            return false;
        }

        //Comprobacion 1: el tipo principal de la Rune debe coincidir con el tipo del Monster
        //Comprobacion de seguridad
        if(rune.MoveData == null || rune.MoveData.EssenceAmountToUse == null || rune.MoveData.EssenceAmountToUse.Count == 0)
        {
            reason = "La Rune no tiene Move Data valida.";
            return false;
        }

        //Guardamos el main type de la rune leyendo el MoveData
        MonsterType runeMainType = rune.MainType;

        //Si no coincide el Main Type de la rune con el type del monster
        if (runeMainType != monster.monsterType)
        {
            //Se indica la reason y devuelve false
            reason = "El tipo principal de la Rune no coincide con el tipo del Monster.";
            return false;
        }

        //Comprobacion 2: el Monster debe tener algun slot libre
        bool hasEmptySlot = monster.equippedRuneIDs.Exists(id => string.IsNullOrEmpty(id));
        if (!hasEmptySlot)
        {
            reason = "El Monster ya tiene " + MAX_RUNE_SLOTS + " Runes equipadas.";
            return false;
        }

        //Comprobacion 3: la Rune no puede estar equipada en otro Monster
        //Guardamos el Monster que es Owner de la rune
        MonsterSaveData ownerMonster = FindRuneOwner(runeID);
        //Si la rune la posee ya un monster y es distinto del monster al que la equipamos
        if (ownerMonster != null && ownerMonster != monster)
        {
            reason = "Esta Rune ya está equipada en otro Monster.";
            return false;
        }

        //Si consigue pasar todas las comprobaciones devuelve true
        reason = string.Empty;
        return true;
    }

    //Devuelve true si la Rune ya esta desbloqueada por el jugador
    public bool isUnlocked(string runeID)
    {
        return playerData.unlockedRuneIDs != null && playerData.unlockedRuneIDs.Contains(runeID);
    }

    //Devuelve el monster que tiene equipada esta Rune
    public MonsterSaveData FindRuneOwner(string runeID)
    {
        //Creamos un bucle que recorre todos los Monsters del Player
        foreach (MonsterSaveData monster in GetAllMonsters())
        {
            //Comprobacion para checkear si la rune la tiene equipped el monster actuals
            if(monster.equippedRuneIDs != null && monster.equippedRuneIDs.Contains(runeID)){
                return monster;
            }
        }

        //Si ningun monster tiene equipada la rune devuelve null
        return null;
    }

    //Devuelve las Runes desbloqueadas ordenadas por tipo principal y luego por numero de tipos
    public List<EssenceRune> GetUnlockedRunesSorted()
    {
        //Creamos una lista de runes
        List<EssenceRune> runes = new List<EssenceRune>();
 
        //Si las unlocked runes del player data es null devolvemos la lista vacia
        if (playerData.unlockedRuneIDs == null) return runes;

        //Creamos un bucle que recorre las unlocked runes del player
        foreach (string runeID in playerData.unlockedRuneIDs)
        {
            //Guardamos la rune actual
            EssenceRune rune = runeDatabase.GetRuneByID(runeID);

            //Añadimos la rune a la lista de runes
            if (rune != null) runes.Add(rune);
        }

        //Ordenamos por tipo principal y luego por numero de tipos del Move
        runes.Sort((a, b) =>
        {
            MonsterType typeA = a.MainType;
            MonsterType typeB = b.MainType;

            int typeCompare = typeA.CompareTo(typeB);
            if (typeCompare != 0) return typeCompare;

            int countA = a.MoveData != null ? a.MoveData.EssenceAmountToUse.Count : 0;
            int countB = b.MoveData != null ? b.MoveData.EssenceAmountToUse.Count : 0;
            return countA.CompareTo(countB);
        });

        //Devolvemos las runes ordenadas
        return runes;
    }

    // ─────────────────────────────────────────
    // EQUIPAR / DESEQUIPAR
    // ─────────────────────────────────────────

    //Equipa una Rune en un Monster, si la RUne ya estaba equipada en otro Monster la desequipa primero, devuelve true si la operacion fue exitosa
    public bool EquipRune(string runeID, MonsterSaveData targetMonster, int slotIndex, out string reason)
    {
        //Comprobacion de seguridad del slot index
        if (slotIndex < 0 || slotIndex >= MAX_RUNE_SLOTS)
        {
            reason = "Slot index invalido.";
            return false;
        }
        
        //Si estaba equipada en otro Monster la desequipamos primero
        MonsterSaveData currentOwner = FindRuneOwner(runeID);
        //Guardamos el indice original por si hay que restaurar
        int originalSlot = currentOwner != null ? currentOwner.equippedRuneIDs.IndexOf(runeID) : -1;

        if (currentOwner != null)
            UnequipRune(runeID, currentOwner);

        //Si no se puede equipar
        if (!CanEquip(runeID, targetMonster, out reason))
        {
            // Si falla, reequipamos en el owner original
            if (currentOwner != null  && originalSlot >= 0)
                currentOwner.equippedRuneIDs[originalSlot] = runeID;

            Debug.LogWarning("RuneSystem: no se puede equipar la Rune " + runeID + " — " + reason);
            return false;
        }

        //Si el target monster no tiene ya la rune la equipada se la equipamos
        if (!targetMonster.equippedRuneIDs.Contains(runeID))
            targetMonster.equippedRuneIDs[slotIndex] = runeID;
            reason = string.Empty;

        return true;
    }

    //Desequipa una Rune del Monster que la tiene equipada, devuelve true si la operacion fue exitosa
    public bool UnequipRune(string runeID, MonsterSaveData monster)
    {
        int index = monster.equippedRuneIDs.IndexOf(runeID);
        if (index < 0)
        {
            Debug.LogWarning("RuneSystem: la Rune " + runeID + " no esta equipada en este Monster");
            return false;
        }

        if (monster.equippedRuneIDs == null || !monster.equippedRuneIDs.Contains(runeID))
        {
            Debug.LogWarning("RuneSystem: la Rune " + runeID + " no esta equipada en este Monster");
            return false;
        }
 
        monster.equippedRuneIDs[index] = null;
        Debug.Log("RuneSystem: Rune " + runeID + " desequipada de " + monster.monsterID);
        return true;
    }

    // ─────────────────────────────────────────
    // UTILIDADES
    // ─────────────────────────────────────────

    //Devuelve todos los Monsters del jugador (party + reserve)
    private List<MonsterSaveData> GetAllMonsters()
    {
        //Creamos una lista de todos los monsters
        List<MonsterSaveData> all = new List<MonsterSaveData>();

        //Guardamos los mosnters de la party y al reserve del player en la lista y la devolvemos
        if (playerData.activeParty != null) all.AddRange(playerData.activeParty);
        if (playerData.reserve != null) all.AddRange(playerData.reserve);

        return all;
    }
}
