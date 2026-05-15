using System;
using System.Collections.Generic;
using UnityEngine;

//Entrada individual de un nodo en el layout, no tiene tipo de evento solo geometria y rol
[Serializable]
public class NodeLayoutEntry
{
    //Identificador unico del nodo dentro de este layout
    public string nodeId;
    //Posicion en grid (ej: columna 2, fila 3)
    public Vector2Int gridPosition;
    //IDs de los nodos a los que conecta este nodo
    public List<string> connectedNodesIds;
    //Rol del nodo
    public NodeRole nodeRole;
}

//ScriptableObject que define la geometria de un piso, solo sabe posiciones y conexiones de nodos
[CreateAssetMenu(fileName = "FloorLayoutData", menuName = "Run/Floor Layout")]
public class FloorLayoutData : ScriptableObject
{
    [Header("Identidad")]
    public string layoutId;

    [Header("Visual del mapa")]
    public Sprite backgroundSprite;

    [Header("Nodos")]
    public List<NodeLayoutEntry> nodes;

    //Devuelve la entrada de un nodo por su ID, null si no existe
    public NodeLayoutEntry GetNode(string nodeId)
    {
        return nodes.Find(n => n.nodeId == nodeId);
    }

    //Devuelve el nodo con el rol start
    public NodeLayoutEntry GetStartNode()
    {
        return nodes.Find(n => n.nodeRole == NodeRole.Start);
    }
}
