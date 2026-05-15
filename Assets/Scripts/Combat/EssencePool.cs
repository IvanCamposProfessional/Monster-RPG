using System.Collections.Generic;
using UnityEngine;

//Pool de Essence de un bando durante el combate
public class EssencePool
{
    //Diccionario que almacena las Essence en la pool, contiene el tipo y la cantidad
    private Dictionary<MonsterType, int> _pool = new Dictionary<MonsterType, int>();

    //Añade Essence de un tipo a la pool
    public void Add(MonsterType type, int amount)
    {
        //Si la pool no contiene el tipo de Essence que vamos a agregar crea la entrada del diccionario con cantidad 0 para evitar nulls
        if (!_pool.ContainsKey(type)) _pool[type] = 0;
        //Añadimos la cantidad a agregar a la entrada del tipo de Essence en el diccionario
        _pool[type] += amount;
    }

    //Comprueba si la pool puede pagar el coste indicado
    public bool CanAfford(List<EssenceAmount> cost)
    {
        //Si el coste es null o 0 devuelve true (se puede pagar)
        if (cost == null || cost.Count == 0) return true;

        //Creamos un bucle que recorra las EssenceAmount del Cost
        foreach (var entry in cost)
        {
            //Si no tenemos ninguna entrada en la pool del tipo de coste devuelve false (no se puede pagar)
            if (!_pool.TryGetValue(entry.Type, out int current)) return false;

            //Si tenemos la entrada pero no la cantidad devuelve false tambien (no sep uede pagar)
            if (current < entry.Amount) return false;
        }

        //Si supera todas estas comprobaciones devuelve true (si que se puede pagar)
        return true;
    }

    //Gasta la Essence indicada de la pool
    public void Spend(List<EssenceAmount> cost)
    {
        //Comprobacion de seguridad
        if (cost == null) return;

        //Creamos un bucle que recorra las EssenceAmount del Cost
        foreach (var entry in cost)
        {
            //Si la pool contiene el tipo de Essence
            if (_pool.ContainsKey(entry.Type))
            {
                //Restamos la Quantity del Cost
                _pool[entry.Type] -= entry.Amount;
            }
        }
    }

    //Devuelve la cantidad de Essence disponible de un tipo concreto, si no existe devuelve 0
    public int Get(MonsterType type)=>
        _pool.TryGetValue(type, out int val) ? val : 0;

    //Devuelve una copia del diccionario completo para la UI 
    public Dictionary<MonsterType, int> GetAll()
    {
        //Creamos el diccionario a devolver
        var result = new Dictionary<MonsterType, int>();

        //Creamos un bucle que recorre la pool de Essence
        foreach(var pair in _pool)
        {
            //Si hay mas de 0 de ese tipo de Essence
            if (pair.Value > 0)
                //Guardamos el Value en el tipo de Essence del diciconario que vamos a devolver
                result[pair.Key] = pair.Value;
        }

        //Devolvemos el diccionario
        return result;
    }

    //Resetea la pool a 0
    public void Reset() => _pool.Clear();
}
