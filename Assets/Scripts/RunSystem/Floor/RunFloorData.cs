using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//Estado en runtime de un nodo individual
public class RunNodeData
{
    public string nodeId;
    //Posicion en grid, viene de FloorLayoutData
    public Vector2Int gridPosition;
    //IDs de los nodos conectados hacia adelante
    public List<string> connectedNodeIds;
    //Tipo de evento asignado en runtime por el RunManager
    public NodeType nodeType;
    //Variable para saber si el jugador ya ha vistiado este nodo
    public bool isVisited;
    //Variable para saber si el jugador puede clickar este nodo desde su posicion actual
    public bool isReachable;

    //Constructor
    public RunNodeData(string nodeId, Vector2Int gridPosition, List<string> connectedNodeIds, NodeType nodeType)
    {
        this.nodeId = nodeId;
        this.gridPosition = gridPosition;
        this.connectedNodeIds = new List<string>(connectedNodeIds);
        this.nodeType = nodeType;
        isVisited = false;
        isReachable = false;
    }
}

//Estado en runtime de un piso completo
public class RunFloorData
{
    public List<RunNodeData> nodes;

    //Constructor
    public RunFloorData(List<RunNodeData> nodes)
    {
        this.nodes = nodes;
    }

    //Devuelve un nodo por su id o null si no existe
    public RunNodeData GetNode(string nodeId)
    {
        return nodes.Find(n => n.nodeId == nodeId);
    }

    //Devuelve el nodo de tipo Camp (punto de entrada)
    public RunNodeData GetStartNode()
    {
        return nodes.Find(n => n.nodeType == NodeType.Camp);
    }
}
