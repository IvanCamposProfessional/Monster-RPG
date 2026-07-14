using UnityEngine;

//ScriptableObject que define los datos estáticos de una habitación de la mansión
[CreateAssetMenu(fileName = "HubRoomData", menuName = "Hub/Room Data")]
public class HubRoomData : ScriptableObject
{
    [Header("Identidad")]
    public string roomId;

    [Header("Grid")]
    //Dimensiones de la grilla de esta habitación
    public int gridWidth;
    public int gridHeight;
    //Posición mundial de la casilla (0,0) de la grilla (esquina inferior izquierda)
    public Vector2 gridOrigin;
    //Coordenadas de tiles bloqueados (obstáculos, paredes interiores)
    public Vector2Int[] blockedTiles;
}
