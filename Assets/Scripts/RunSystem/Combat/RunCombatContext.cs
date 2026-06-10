using UnityEngine;

//Clase estatica que transporta el contexto de combate entre la RunScene y el CombatScene
//RunNode escribe aqui antes de cargar la CombatScene
//CombatManager lee aqui al inicializarse

public static class RunCombatContext
{
    //Tipo de run activo, determina la pool de enemies
    public static MonsterType ThemeType { get; private set; }
    //Indice del piso actual, determina la dificultad del encuentro
    public static int FloorIndex { get; private set; }
    //Tipo de nodo que originó el combate (Battle, Elite, Boss)
    public static NodeType NodeType { get; private set; }
    //ID del nodo origen para notificar al RunManager al volver
    public static string NodeId { get; private set; }
    //True si hay contexto activo pendiente de ser leido
    public static bool IsSet { get; private set; }
    //Resultado del combate: true = victoria, false = derrota
    public static bool BattleWon { get; private set; }
    //True si hay resultado pendiente de ser procesado por RunManager
    public static bool HasResult { get; private set; }
    // Posicion Y de la camara al salir de RunScene, para restaurarla al volver
    public static float CameraY { get; private set; }

    //Escribe el contextro antes de cargar la CombatScene
    public static void Set(MonsterType themeType, int floorIndex, NodeType nodeType, string nodeId, float cameraY)
    {
        Debug.Log("RunCombatContext.Set llamado — nodeType: " + nodeType + " | nodeId: " + nodeId);

        ThemeType  = themeType;
        FloorIndex = floorIndex;
        NodeType   = nodeType;
        NodeId     = nodeId;
        IsSet      = true;
        CameraY = cameraY;
    }

    //Escribe el resultado del combate antes de volver a RunScene
    public static void SetResult(bool battleWon)
    {
        BattleWon = battleWon;
        HasResult = true;
    }

    //Limpia el contexto una vez el CombatManager lo ha leido
    public static void Clear()
    {
        IsSet = false;
    }

    //Limpia el resultado una vez RunManager lo ha procesado
    public static void ClearResult()
    {
        HasResult = false;
    }
}
