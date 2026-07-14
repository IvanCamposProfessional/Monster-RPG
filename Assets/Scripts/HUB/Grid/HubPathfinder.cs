using System.Collections.Generic;
using UnityEngine;

//A* sobre HubGrid. Devuelve la lista de posiciones mundiales del camino, o null si no existe ruta.
public class HubPathfinder
{
    private HubGrid _grid;

    //Direcciones: 4 direcciones cardinales (sin diagonales)
    private static readonly Vector2Int[] _directions = {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    public HubPathfinder(HubGrid grid)
    {
        _grid = grid;
    }

    // ─────────────────────────────────────────
    // PATHFINDING
    // ─────────────────────────────────────────

    //Devuelve la lista de centros de tile en coordenadas mundo desde start hasta end, devuelve null si no hay ruta válida.
    public List<Vector2> FindPath(Vector2 startWorld, Vector2 endWorld)
    {
        Vector2Int start = _grid.WorldToGrid(startWorld);
        Vector2Int end   = _grid.WorldToGrid(endWorld);

        //Comprobaciones de seguridad
        if (!_grid.IsWalkable(start.x, start.y)) return null;
        if (!_grid.IsWalkable(end.x, end.y))     return null;

        //Nodos abiertos y cerrados
        List<AStarNode> open   = new List<AStarNode>();
        HashSet<Vector2Int> closed = new HashSet<Vector2Int>();

        AStarNode startNode = new AStarNode(start, null, 0, Heuristic(start, end));
        open.Add(startNode);

        while (open.Count > 0)
        {
            // Seleccionamos el nodo con menor F
            AStarNode current = GetLowestF(open);

            if (current.Position == end)
                return BuildPath(current);

            open.Remove(current);
            closed.Add(current.Position);

            //Exploramos vecinos
            foreach (Vector2Int dir in _directions)
            {
                Vector2Int neighborPos = current.Position + dir;

                if (closed.Contains(neighborPos)) continue;
                if (!_grid.IsWalkable(neighborPos.x, neighborPos.y)) continue;

                float g = current.G + 1f;
                float h = Heuristic(neighborPos, end);

                AStarNode existing = open.Find(n => n.Position == neighborPos);
                if (existing == null)
                {
                    open.Add(new AStarNode(neighborPos, current, g, h));
                }
                else if (g < existing.G)
                {
                    existing.G      = g;
                    existing.Parent = current;
                }
            }
        }

        //No hay ruta
        return null;
    }

    // ─────────────────────────────────────────
    // PRIVADOS
    // ─────────────────────────────────────────

    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        //Manhattan distance para grilla sin diagonales
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private AStarNode GetLowestF(List<AStarNode> list)
    {
        AStarNode lowest = list[0];
        foreach (AStarNode node in list)
            if (node.F < lowest.F) lowest = node;
        return lowest;
    }

    private List<Vector2> BuildPath(AStarNode endNode)
    {
        List<Vector2> path = new List<Vector2>();
        AStarNode current  = endNode;

        while (current != null)
        {
            path.Add(_grid.GridToWorld(current.Position.x, current.Position.y));
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }

    // ─────────────────────────────────────────
    // NODO INTERNO
    // ─────────────────────────────────────────

    private class AStarNode
    {
        public Vector2Int Position;
        public AStarNode  Parent;
        public float      G; //coste real desde el inicio hasta este tile. Cada paso cuesta 1.
        public float      H; //estimación de la distancia hasta el destino (la heurística). Usamos distancia
        public float      F => G + H; //prioridad total. El algoritmo siempre explora primero el tile con F más bajo.

        public AStarNode(Vector2Int position, AStarNode parent, float g, float h)
        {
            Position = position;
            Parent   = parent;
            G        = g;
            H        = h;
        }
    }
}
