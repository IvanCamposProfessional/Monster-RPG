using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    public MonsterData data;

    public int currentHP { get; set; }
    public int maxHP { get; set; }
    public int currentBP;
    public int maxBP;
    public int currentAttack { get; set; }
    public int currentDefense { get; set; }
    public int currentSpecialAttack { get; set; }
    public int currentSpecialDefense { get; set; }
    public int level;
    public int currentSpeed { get; set; }
    //Los ataques que el monstruo actualmente sabe
    public List<MoveData> learnedMoves;
    //Lista de stat modifiers activos
    public List<StatModifierInstance> statModifiers = new List<StatModifierInstance>();
    //Lista de Altered States activos
    public List<AlteredStateInstance> alteredStates = new List<AlteredStateInstance>(); 

    //IA del enemy, se queda a null si es ally
    public EnemyAI enemyAI;

    //Flag que el StuntInstance activa para bloquear el turno
    public bool actionBlocked = false;

    public Monster(MonsterData data, int level, int currentHP, int currentBP){
        this.data = data;
        this.level = level;
        this.currentHP = currentHP;
        this.currentBP = currentBP;
        currentSpeed = data.BaseSpeed;
        currentAttack = data.BaseAttack;
        currentDefense = data.BaseDefense;
        currentSpecialAttack = data.BaseSpecialAttack;
        currentSpecialDefense = data.BaseSpecialDefense;
        maxHP = CalculateMaxHP();
        maxBP = CalculateMaxBP();
        //Inicializamos la lista de los Learned Moves
        learnedMoves = new List<MoveData>();
    }

    public bool IsAlive => currentHP > 0;

    int CalculateMaxHP(){
        return data.BaseHP + level * 5;
    }

    int CalculateMaxBP(){
        return data.BaseBP + level * 5;
    }

    //Funcion para que el Monster reciba daño, utilizamos Mathf.Max para que la vida nunca baje de 0
    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
    }

    //Funcion para que el Monster reciba curacion, utilizamos Mathf.Min para que la vida nunca suba del maximoq
    public void Heal(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }

    //Aplica un stat modifier
    public void AddStatModifier(StatModifier modifierData)
    {
        //Si no  es stackable y ya existe uno con el mismo ID lo eliminamos para que despues se vuelva a añadir y refrescarlo
        if (!modifierData.stackable)
        {
            //Buscamos si la lista de stat modifiers existe el Stat Modifier que intentamos añadir
            StatModifierInstance existing = statModifiers.Find(m => m.modifierId == modifierData.modifierId);
            //Si el stat modifier ya se encontraba en la lista (el existing que hemos creado ha almacenado un stat modifier al buscarlo en la lista)
            if(existing != null)
            {
                existing.OnRemove(this);
                statModifiers.Remove(existing);
            }
        }

        //Creamos la instancia del StatModifier
        StatModifierInstance instance = modifierData.CreateInstance();
        //Añadimos el Stat Modifier a la lista y lo aplicamos al monster
        statModifiers.Add(instance);
        instance.OnApply(this);
    }

    //Aplica un altered state
    public void AddAlteredState(AlteredState stateData, int intensity, int duration)
    {
        //Si no  es stackable y ya existe uno con el mismo ID lo eliminamos para que despues se vuelva a añadir y refrescarlo
        if (!stateData.stackable)
        {
            //Buscamos si la lista de altered state instances existe el altered state que intentamos añadir
            AlteredStateInstance existing = alteredStates.Find(s => s.stateId == stateData.stateId);
            //Si el altered state ya se encontraba en la lista (el existing que hemos creado ha almacenado un altered state al buscarlo en la lista)
            if(existing != null)
            {
                existing.OnRemove(this);
                alteredStates.Remove(existing);
            }
        }

        //Creamos la instalncia del Altered State, la añadimos a la lista y lo aplicamos al monster
        AlteredStateInstance instance = stateData.CreateInstance(intensity, duration);
        alteredStates.Add(instance);
        instance.OnApply(this);
    }

    //Elimina un stat modifier por ID
    public void RemoveStatModifier(string modifierId)
    {
        //Buscamos el stat modifier por id
        StatModifierInstance modifier = statModifiers.Find(m => m.modifierId == modifierId);
        //Si no esta el stat modifier (es null) salimos de la funcion
        if (modifier == null) return;

        //Eliminamos el stat modifier
        modifier.OnRemove(this);
        statModifiers.Remove(modifier);
    }

    //Elimina un AlteredState por ID
    public void RemoveAlteredState(string stateId)
    {
        //Buscamos el altered state por id
        AlteredStateInstance state = alteredStates.Find(s => s.stateId == stateId);
        //Si no esta el altered state (es null) salimos de la funcion
        if (state == null) return;

        //Eliminamos el altered state
        state.OnRemove(this);
        alteredStates.Remove(state);
    }

    //Procesa todos los stat modifiers y altered states segun el timing
    //Se llama desde el CombatManager al inicio y al fin de cada turno
    public void ProcessModifiers(ModifierTiming timing)
    {
        // Procesamos StatModifiers con el timing correspondiente
        //Iteramos al reves para poder eliminar sin romper el indice de la lista
        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            //Los stats modifiers siempre tickean en Turn End
            if (timing != ModifierTiming.OnTurnEnd) continue;

            //Lanzamos el tick del stat modifier y guardamos si expira este turno
            bool expired = statModifiers[i].OnTick(this);
            //Si expira este turno lo eliminamos
            if (expired)
            {
                statModifiers[i].OnRemove(this);
                statModifiers.RemoveAt(i);
            }
        }

        // Procesamos Altered States con el timing correspondiente
        //Iteramos al reves para poder eliminar sin romper el indice de la lista
        for (int i = alteredStates.Count - 1; i >= 0; i--)
        {
            //Si el timing del altered state a procesar es distinto al timing que le pasamos a la funcion pasampos a la siguiente iteracion del bucle
            if (alteredStates[i].timing != timing) continue;

            //Lanzamos el tick del altered state y guardamos si expira este turno
            bool expired = alteredStates[i].OnTick(this);
            //Si expira este turno lo eliminamos
            if (expired)
            {
                alteredStates[i].OnRemove(this);
                alteredStates.RemoveAt(i);
            }
        }
    }
}
