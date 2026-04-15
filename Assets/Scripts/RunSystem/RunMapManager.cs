using System.Collections.Generic;
using UnityEngine;

//Singleton de la escena de Run.
//Recibe el RunFloorData y el FloorLayoutData del RunManager, instancia los prefabs de nodos y aplica el fondo del layout
public class RunMapManager : MonoBehaviour
{
    public static RunMapManager Instance { get; private set; }

    [Header("Prefabs y contenedor")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private Transform nodesContainer;
    [SerializeField] private GameObject connectionLinePrefab;

     [Header("Fondo de escena")]
    //SpriteRenderer del objeto de fondo en la RunScene
    //Se asigna en el inspector — el sprite lo pone el RunMapManager en runtime
    [SerializeField] private SpriteRenderer backgroundRenderer;

    [Header("Layout")]
    [SerializeField] private float gridSpacing = 2f;

    //Diccionario que guarda la ID del nodo y el nodo activo
    private Dictionary<string, RunNode> activeNodes = new Dictionary<string, RunNode>();
    //Lista de GameObjects que guarda los prefabs de connection lines
    private List<GameObject> activeConnections = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ─────────────────────────────────────────
    // CONSTRUCCION DEL MAPA
    // ─────────────────────────────────────────

    //Recibe el piso generado y el layout elegido
    public void BuildMap(RunFloorData floorData, FloorLayoutData layout)
    {
        ClearMap();
        ApplyBackground(layout);

        //Creamos un bucle que recorra los nodes del floor data
        foreach(RunNodeData nodeData in floorData.nodes)
        {
            //Guardamos la posicion del nodo
            Vector3 worldPos = GridToWorld(nodeData.gridPosition);

            //Instanciamos el prefab en la posicion que hemos guardado antes
            GameObject obj = Instantiate(nodePrefab, worldPos, Quaternion.identity, nodesContainer);
            obj.name = "Node_" + nodeData.nodeId + "_" + nodeData.nodeType;

            //Guardamos el Run Node en runtime del prefab y le hacemos Setup
            RunNode runNode = obj.GetComponent<RunNode>();
            runNode.Setup(nodeData);

            //Guardamos el nodo en el diccionario
            activeNodes[nodeData.nodeId] = runNode;
        }

        //Construimos las lineas de conexiones de la escena
        if (connectionLinePrefab != null)
            BuildConnections(floorData);

        Debug.Log("RunMapManager: mapa construido con layout " + layout.layoutId);
    }

    //Refresca el estado visual de todos los nodos activos
    public void RefreshNodes()
    {
        foreach (RunNode node in activeNodes.Values)
            node.RefreshVisual();
    }

    // ─────────────────────────────────────────
    // PRIVADOS
    // ─────────────────────────────────────────

    //Aplica el sprite del fondo de la escena
    private void ApplyBackground(FloorLayoutData layout)
    {
        //Comprobacion de seguridad
        if (backgroundRenderer == null)
        {
            Debug.LogWarning("RunMapManager: no hay backgroundRenderer asignado");
            return;
        }
 
        if (layout.backgroundSprite != null)
            backgroundRenderer.sprite = layout.backgroundSprite;
    }

    private void ClearMap()
    {
        //Destruimos los nodos en escena y limpiamos el diccionario
        foreach (RunNode node in activeNodes.Values)
            if (node != null) Destroy(node.gameObject);
        activeNodes.Clear();
 
        //Destruimos las line connections de la escena y limpiamos la lista
        foreach (GameObject line in activeConnections)
            if (line != null) Destroy(line);
        activeConnections.Clear();
    }

    private void BuildConnections(RunFloorData floorData)
    {
        //Creamos el Hash para guardar las conexiones ya dibujadas
        HashSet<string> drawn = new HashSet<string>();

        //Creamos un bucle que recorre los nodes en el floor
        foreach (RunNodeData nodeData in floorData.nodes)
        {
            //Creamos un bucle que recorra los connected nodes
            foreach (string connectedId in nodeData.connectedNodeIds)
            {
                //CompareOrdinal devuelve un número negativo si nodeId va antes que connectedId alfabéticamente
                //En ese caso la clave es "A_B", si no es "B_A" — siempre el mismo orden para el mismo par
                string key = string.CompareOrdinal(nodeData.nodeId, connectedId) < 0 ? nodeData.nodeId + "_" + connectedId : connectedId + "_" + nodeData.nodeId;

                //Si ya ha sido dibujado continue a la siguiente iteracion
                if (drawn.Contains(key)) continue;
                //Si no ha saltado a la siguiente iteracion añade el dibujo al Hash
                drawn.Add(key);
                if (!activeNodes.TryGetValue(nodeData.nodeId, out RunNode fromNode)) continue;
                if (!activeNodes.TryGetValue(connectedId, out RunNode toNode)) continue;

                //Dibujamos las lineas
                DrawConnection(fromNode.transform.position, toNode.transform.position);
            }
        }
    }

    private void DrawConnection(Vector3 from, Vector3 to)
    {
        //Instanciamos el prefab de la linea
        GameObject lineObj = Instantiate(connectionLinePrefab, nodesContainer);
        //Añadimos la linea a la lista de conexiones
        activeConnections.Add(lineObj);

        //Configuramos la posicion de la linea
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, from);
            lr.SetPosition(1, to);
        }
    }

    //Funcion para convertir la posicion en grid a la posicion del mundo
    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * gridSpacing, gridPos.y * gridSpacing, 0f);
    }
}
