using UnityEngine;

// Construye y expone el mapa de tiles caminables de una habitación, se inicializa con los datos del HubRoomData al activar la habitación.
public class HubGrid
{
    private bool[,] _walkable;
    private int _width;
    private int _height;
    private Vector2 _origin;

    // ─────────────────────────────────────────
    // INICIALIZACIÓN
    // ─────────────────────────────────────────

    public HubGrid(HubRoomData data)
    {
        _width  = data.gridWidth;
        _height = data.gridHeight;
        _origin = data.gridOrigin;

        //Todos los tiles son caminables por defecto
        _walkable = new bool[_width, _height];
        for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
                _walkable[x, y] = true;

        // Marcamos los tiles bloqueados
        if (data.blockedTiles != null)
            foreach (Vector2Int tile in data.blockedTiles)
                if (IsInBounds(tile.x, tile.y))
                    _walkable[tile.x, tile.y] = false;
    }

    // ─────────────────────────────────────────
    // CONSULTAS
    // ─────────────────────────────────────────

    public bool IsWalkable(int x, int y)
    {
        if (!IsInBounds(x, y)) return false;
        return _walkable[x, y];
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < _width && y >= 0 && y < _height;
    }

    // ─────────────────────────────────────────
    // CONVERSIÓN MUNDO ↔ GRID
    // ─────────────────────────────────────────

    //Convierte posición mundial a coordenadas de grilla
    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x - _origin.x);
        int y = Mathf.FloorToInt(worldPos.y - _origin.y);
        return new Vector2Int(x, y);
    }

    //Convierte coordenadas de grilla a centro del tile en mundo
    public Vector2 GridToWorld(int x, int y)
    {
        return new Vector2(_origin.x + x + 0.5f, _origin.y + y + 0.5f);
    }
}
