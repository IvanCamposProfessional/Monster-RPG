using UnityEngine;

//// Singleton local de la HubScene. Coordina los subsistemas del HUB y expone el flag de bloqueo de input que todos los sistemas deben respetar.
public class HubManager : MonoBehaviour
{
    [SerializeField] private HubRoom _startRoom;

    public static HubManager Instance { get; private set; }

    // true durante transiciones, fades, diálogos o cualquier acción que bloquee al jugador
    public bool IsInputBlocked { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
{
    //Posicionamos la cámara en la habitación inicial al arrancar
    HubCameraManager.Instance.SnapToRoom(_startRoom);
}

    // ─────────────────────────────────────────
    // CONTROL DE INPUT
    // ─────────────────────────────────────────

    public void BlockInput()  => IsInputBlocked = true;
    public void UnblockInput() => IsInputBlocked = false;
}
