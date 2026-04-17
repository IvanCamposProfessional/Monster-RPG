using System.Collections.Generic;
using UnityEngine;

//Genera los pisos de la run combinando un FloorLayoutData con los pesos del RunTypeData
//Expone RunType y CurrentLayout para que otros sistemas puedan leer el tema y el visual del piso actual
public class RunManager : MonoBehaviour
{
    public static RunManager Instance {  get; private set; }

    [SerializeField] private RunTypeData runType;
    //Creamos la variable publica de Run Type para que el resto de sistemas puedan leer RunManager.Instance.RunType
    public RunTypeData RunType => runType;

    //Layout activo del piso actual
    public FloorLayoutData CurrentLayout { get; private set; }

    //Indice del piso actual (0 = primer piso)
    public int CurrentFloorIndex{ get; private set; }
    //Datos del piso actualmente activos
    public RunFloorData CurrentFloor { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ─────────────────────────────────────────
    // GENERACION
    // ─────────────────────────────────────────

    //Funcion para empezar la run a nivel logico
    public void StartRun()
    {
        //Indicamos que el piso es el 1
        CurrentFloorIndex = 0;
        //Generamos el piso pasandole el piso actual
        GenerateFloor(CurrentFloorIndex);
    }

    //Funcion para avanzar de piso en la run a nivel logico
    public void AdvanceFloor()
    {
        //Sumamos 1 al piso actual
        CurrentFloorIndex++;
        //Generamos el piso pasandole el piso actual
        GenerateFloor(CurrentFloorIndex);
    }

    //Funcion privada para generar el piso correspondiente de forma logica
    private void GenerateFloor(int floorIndex)
    {
        //Comprobacion de seguridad
        if (runType == null)
        {
            Debug.LogWarning("RunManager: no hay RunTypeData asignado");
            return;
        }

        //Paso 1: elegir layout aleatorio de la pool del piso actual
        //Llamamos a la funcion GetRandomLayoutForFloor dentro de RunTypeData
        FloorLayoutData layout = runType.GetRandomLayoutForFloor(floorIndex);
        //Comprobacion de seguridad
        if (layout == null) return;
        //Guaradmos el Current Layout con el que hemos generado aleatoriamente
        CurrentLayout = layout;

        //Paso 2: Obtener pesos de este piso
        //Creamos una lista de pesos y llamamos a GetWeitghtsForFloor dentro de RunTypeData
        List<NodeTypeWeight> weights = runType.GetWeightsForFloor(floorIndex);

        //Paso 3: Construir los RunNodeData
        //Creamos una lista de node data
        List<RunNodeData> nodeList = new List<RunNodeData>();

        //Recorremos los Node Layout Entrys dentro de nodes
        foreach(NodeLayoutEntry entry in layout.nodes)
        {
            //Inicializamos el tipo de nodo y lo guardamos
            NodeType assignedType = AssignNodeType(entry.nodeRole, weights);

            //Inicializamos el Node Data actual
            RunNodeData nodeData = new RunNodeData(
                entry.nodeId,
                entry.gridPosition,
                entry.connectedNodesIds,
                assignedType
            );

            //Añadirmos el node inicializado a la lista de nodes
            nodeList.Add(nodeData);
        }

        //Paso 4: Marcar como Reachable solo el nodo Start
        //Creamos el RunFloorData pasandole la Node List que hemos creado
        RunFloorData floorData = new RunFloorData(nodeList);
        //Guardamos el Start Node
        RunNodeData startNode = floorData.GetStartNode();

         //Guardamos el piso generado como Current Floor
        CurrentFloor = floorData;

        //Comprobacion de seguridad
        if(startNode != null)
        {
            //Marcamos que el Start Node Is Visible
            startNode.isVisited = true;
            OnNodeVisited(startNode.nodeId);
        }
        else
        {
            Debug.LogWarning("RunManager: el layout " + layout.layoutId + " no tiene nodo Start");
        }

        Debug.Log("RunManager: piso " + floorIndex + " generado | tipo: " + runType.runTypeName + " (" + runType.themeType + ") | layout: " + layout.layoutId);

        //Paso 5: Entregar al RunMapManager
        if (RunMapManager.Instance != null)
        {
            //Llamamos al RunMapManager y creamos el mapa con el piso actual y el layout actual
            RunMapManager.Instance.BuildMap(CurrentFloor, CurrentLayout);
        }
        else
        {
            Debug.LogWarning("RunManager: no hay RunMapManager en escena");
        }
    }

    // ─────────────────────────────────────────
    // PROGRESO DEL JUGADOR
    // ─────────────────────────────────────────

    public void OnNodeVisited(string nodeId)
    {
        //Comprobacion de seguridad
        if (CurrentFloor == null) return;

        //Guardamos el nodo visitado
        RunNodeData visited = CurrentFloor.GetNode(nodeId);
        //Comprobacion de seguridad
        if (visited == null) return;

        //Desactivamos el isReachable de TODOS los nodos antes de asignar los nuevos, asi los nodos paralelos que no se visitaron quedan bloqueados
        foreach (RunNodeData node in CurrentFloor.nodes)
        {
            if (!node.isVisited)
                node.isReachable = false;
        }
        
        //Marcamos que se ha visitado y que ya no es alcanzable
        visited.isVisited = true;

        //Creamos un bucle que recorre los nodos alcanzables al actual
        foreach(string connectedId in visited.connectedNodeIds)
        {
            //Guardamos el nodo conectado con el actual
            RunNodeData connected = CurrentFloor.GetNode(connectedId);

            //Si el nodo conectado no se ha visitado
            if(connected != null && !connected.isVisited)
            {
                //Guardamos que el nodo conectado es alcanzable
                connected.isReachable = true;
            }
        }

        //Refrescamos los nodos
        if (RunMapManager.Instance != null)
            RunMapManager.Instance.RefreshNodes();
    }

    // ─────────────────────────────────────────
    // PRIVADOS
    // ─────────────────────────────────────────

    private NodeType AssignNodeType(NodeRole role, List<NodeTypeWeight> weights)
    {
        //Creamos un switch para asignar el tipo de nodo dependiendo del rol que le pasamos a la funcion
        switch (role)
        {
            case NodeRole.Start: return NodeType.Camp;
            case NodeRole.Boss: return NodeType.Boss;
            default: return RollWeightedType(weights);
        }
    }

    //Funcion para asignar el tipo de nodo segun el peso
    private NodeType RollWeightedType(List<NodeTypeWeight> weights)
    {
        //Comprobacion de seguridad
        if (weights == null || weights.Count == 0)
        {
            Debug.LogWarning("RunManager: no hay pesos definidos, usando Battle por defecto");
            return NodeType.Battle;
        }

        //Creamos la variable para guardar el peso
        float totalWeight = 0f;

        //Creamos un bucle que recorra los pesos
        foreach (NodeTypeWeight w in weights)
        {
            //Guardamos la suma total de los pesos
            totalWeight += w.weight;
        }

        //Creamos un numero random entre 0 y el peso total
        float roll = Random.Range(0f, totalWeight);
        //Creamos una variable para guardar el peso acumulado
        float accumulated = 0f;

        //Creamos un bucle que recorra los pesos
        foreach (NodeTypeWeight w in weights)
        {
            //Guardamos el peso actual en el peso acumulado
            accumulated += w.weight;
            //Si el peso random es menor o igual al peso acumulado devolvemos el tipo del nodo de ese peso
            if (roll <= accumulated)
                return w.nodeType;
        }

        //Devolvemos el tipo de nodo mas pesado
        return weights[weights.Count - 1].nodeType;
    }
}
