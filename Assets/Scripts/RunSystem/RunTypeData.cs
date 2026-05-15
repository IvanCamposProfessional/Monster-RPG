using System;
using System.Collections.Generic;
using UnityEngine;

//Par de Node Type + peso para el sistema de pesos
[Serializable]
public class NodeTypeWeight
{
    public NodeType nodeType;
    //Peso relativo
    [Min(0)] public float weight;
}

//Override de pesos para un piso concreto, si existe un override para el FloorIndex actual sustituye al Default
[Serializable]
public class FloorWeightOverride
{
    //Indica el piso al que aplica este override (0 = piso 1)
    public int floorIndex;
    public List<NodeTypeWeight> eventWeights;
}

//Pool de layouts para un piso concreto
[Serializable]
public class FloorLayoutPool
{
    //Indice del piso al que pertenece esta pool (0 = piso 1)
    public int floorIndex;
    //Layouts disponibles para este piso
    public List<FloorLayoutData> layouts;
}

//Scriptable object que define un tipo de run completo
[CreateAssetMenu(fileName = "RunTypeData", menuName = "Run/Run Type")]
public class RunTypeData : ScriptableObject
{
    [Header("Identidad")]
    public string runTypeId;
    public string runTypeName;

    [Header("Tema de monsters")]
    public MonsterType themeType;

    [Header("Layouts por piso")]
    public List<FloorLayoutPool> layoutsByFloor;

    [Header("Peso de eventos")]
    //Pesos de eventos en la run
    public List<NodeTypeWeight> defaultEventWeights;
    //OPCIONAL - Override de pesos por piso, se puede dejar vacio
    public List<FloorWeightOverride> perFloorWeightOverrides;

    //─────────────────────────────────────────
    //CONSULTAS
    //─────────────────────────────────────────

    //Devuelve un layout aleatorio de los disponibles
    public FloorLayoutData GetRandomLayoutForFloor(int floorIndex)
    {
        //Comprobacion de seguridad
        if(layoutsByFloor == null)
        {
            Debug.LogWarning("RunTypeData: layoutsByFloor no esta configurado en " + runTypeId);
            return null;
        }

        //Guardamos la pool de layouts del piso en el que nos encontramos
        FloorLayoutPool pool = layoutsByFloor.Find(p => p.floorIndex == floorIndex);

        //Comprobaciones de seguridad
        if (pool == null)
        {
            Debug.LogWarning("RunTypeData: no hay pool de layouts para el piso " + floorIndex + " en " + runTypeId);
            return null;
        }

        if (pool.layouts == null || pool.layouts.Count == 0)
        {
            Debug.LogWarning("RunTypeData: la pool del piso " + floorIndex + " esta vacia en " + runTypeId);
            return null;
        }

        //Devolvemos un layout aleatorio dentro de la pool de layouts
        return pool.layouts[UnityEngine.Random.Range(0, pool.layouts.Count)];
    }

    //Devuelve los pesos activos para el piso indicado, si existe override para ese indice de piso lo usa si no usa default
    public List<NodeTypeWeight> GetWeightsForFloor(int floorIndex)
    {
        //Si existe override de los pesos
        if (perFloorWeightOverrides != null)
        {
            //Busca y guarda el override correspondiente al piso en el que nos encontramos
            FloorWeightOverride match = perFloorWeightOverrides.Find(o => o.floorIndex == floorIndex);

            //Comprobacion de seguridad
            if (match != null && match.eventWeights != null && match.eventWeights.Count > 0)
                //Devuelve los pesos de eventos del piso en el override
                return match.eventWeights;
        }

        //Si no existe override para los pesos devuelve Default Event Weights
        return defaultEventWeights;
    }
}
