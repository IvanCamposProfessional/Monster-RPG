using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Gestiona el movimiento Point & Click del jugador sobre la grilla del HUB, lee el click del ratón, calcula la ruta con A* y mueve el personaje tile a tile.
[RequireComponent(typeof(SpriteRenderer))]
public class HubPlayerController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float _moveSpeed = 5f;

    //Habitación activa — se asigna al iniciar y al cambiar de habitación
    private HubRoom _currentRoom;
    private HubGrid _grid;
    private HubPathfinder _pathfinder;

    private bool _isMoving = false;

    private List<Vector2> _debugPath;

    public HubGrid CurrentGrid => _grid;

    //Devuelve el tile actual del player en coordenadas de grilla
    public Vector2Int CurrentTile => _grid.WorldToGrid(transform.position);

    public static HubPlayerController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ─────────────────────────────────────────
    // INICIALIZACIÓN
    // ─────────────────────────────────────────

    private void Start()
    {
        // Buscamos la habitación activa en el RoomsContainer
        InitializeRoom(FindActiveRoom());
    }

    //Inicializa la grilla y el pathfinder con los datos de la habitación recibida
    public void InitializeRoom(HubRoom room)
    {
        if (room == null)
        {
            Debug.LogWarning("HubPlayerController: no se encontró habitación activa.");
            return;
        }

        _currentRoom = room;
        _grid = new HubGrid(room.Data);
        _pathfinder = new HubPathfinder(_grid);

        //Snappeamos al centro del tile más cercano a la posición inicial
        Vector2Int startTile = _grid.WorldToGrid(transform.position);
        transform.position = _grid.GridToWorld(startTile.x, startTile.y);
    }

    //Busca el primer HubRoom activo en la escena
    private HubRoom FindActiveRoom()
    {
        foreach (HubRoom room in FindObjectsByType<HubRoom>(FindObjectsSortMode.None))
            if (room.gameObject.activeSelf) return room;

        return null;
    }

    // ─────────────────────────────────────────
    // INPUT
    // ─────────────────────────────────────────

    private void Update()
    {
        //Ignoramos el input si está bloqueado o el personaje ya se está moviendo
        if (HubManager.Instance.IsInputBlocked) return;
        if (_isMoving) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            HandleClick();
    }

    private void HandleClick()
    {
        //Si el click cayó sobre un objeto de UI o un interactuable, lo ignoramos
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        //Convertimos la posición del click en pantalla a posición en mundo
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        //Comprobamos si el click cayó sobre una puerta
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null && hit.GetComponent<HubDoor>() != null)
            return;

        //Calculamos la ruta desde la posición actual del jugador hasta el click
        List<Vector2> path = _pathfinder.FindPath(transform.position, worldPos);
        if (path == null || path.Count == 0) return;

        _debugPath = path; // guardamos para debug
        StartCoroutine(MoveAlongPath(path));
    }

    // ─────────────────────────────────────────
    // MOVIMIENTO
    // ─────────────────────────────────────────

    private IEnumerator MoveAlongPath(List<Vector2> path, System.Action onComplete = null)
    {
        _isMoving = true;
        HubManager.Instance.BlockInput();

        foreach (Vector2 target in path)
        {
            while ((Vector2)transform.position != target)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    target,
                    _moveSpeed * Time.deltaTime
                );
                yield return null;
            }
        }

        HubManager.Instance.UnblockInput();
        _isMoving = false;

        onComplete?.Invoke();
    }

    // ─────────────────────────────────────────
    // WALK
    // ─────────────────────────────────────────

    // Camina al tile destino y ejecuta el callback al llegar
    public void WalkToTile(Vector2Int targetTile, System.Action onArrival)
    {
        if (_isMoving) return;

        Vector2 target = _grid.GridToWorld(targetTile.x, targetTile.y);
        List<Vector2> path = _pathfinder.FindPath(transform.position, target);

        if (path == null || path.Count == 0) return;

        _debugPath = path;
        StartCoroutine(MoveAlongPath(path, onArrival));
    }

    // ─────────────────────────────────────────
    // PUERTAS
    // ─────────────────────────────────────────

    public void TriggerDoorTransition(HubDoor door, Vector2Int spawnTile)
    {
        if (!door.IsUnlocked())
        {
            Debug.Log("Puerta bloqueada: " + door.LockedMessage);
            _isMoving = false;
            return;
        }

        HubTransitionManager.Instance.TransitionToRoom(door.DestinationRoomId, spawnTile, door);
    }

    //Teletransporta al player al tile indicado y reinicializa la grilla con la nueva habitación
    public void TeleportToTile(Vector2Int tile, HubRoom room, DoorDirection facingDirection)
    {
        //Reinicializamos la grilla con la habitación destino
        InitializeRoom(room);

        //Forzamos el tile exacto de spawn
        transform.position = _grid.GridToWorld(tile.x, tile.y);

        //TODO: orientar el sprite del player según facingDirection
    }

    // ─────────────────────────────────────────
    // DEBUG — GIZMOS
    // ─────────────────────────────────────────

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        HubRoom room = Application.isPlaying ? _currentRoom : FindActiveRoom();
        if (room == null || room.Data == null) return;

        HubRoomData data = room.Data;

        // Dibujamos la grilla
        for (int x = 0; x < data.gridWidth; x++)
        {
            for (int y = 0; y < data.gridHeight; y++)
            {
                bool blocked = Application.isPlaying
                    ? !_grid.IsWalkable(x, y)
                    : System.Array.Exists(data.blockedTiles, t => t.x == x && t.y == y);

                Gizmos.color = blocked
                    ? new Color(1f, 0f, 0f, 0.5f)
                    : new Color(0f, 1f, 0f, 0.2f);

                Vector2 center = new Vector2(
                    data.gridOrigin.x + x + 0.5f,
                    data.gridOrigin.y + y + 0.5f
                );

                Gizmos.DrawCube(new Vector3(center.x, center.y, 0f), Vector3.one * 0.9f);
            }
        }

        // Dibujamos la ruta calculada
        if (_debugPath == null || _debugPath.Count < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < _debugPath.Count - 1; i++)
            Gizmos.DrawLine(
                new Vector3(_debugPath[i].x, _debugPath[i].y, 0f),
                new Vector3(_debugPath[i + 1].x, _debugPath[i + 1].y, 0f)
            );

        // Marcamos el destino final
        Gizmos.color = Color.cyan;
        Vector2 last = _debugPath[_debugPath.Count - 1];
        Gizmos.DrawSphere(new Vector3(last.x, last.y, 0f), 0.3f);
    }
    #endif
}
