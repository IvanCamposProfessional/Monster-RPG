using UnityEngine;

//Gestiona la cámara ortográfica del HUB, se teletransporta instantáneamente al CameraPosition de la habitación activa.
public class HubCameraManager : MonoBehaviour
{
    public static HubCameraManager Instance { get; private set; }
    [SerializeField] private Camera _hubCamera;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ─────────────────────────────────────────
    // PÚBLICO
    // ─────────────────────────────────────────

    //Teletransporta la cámara al punto definido en el HubRoomData
    public void SnapToRoom(HubRoom room)
    {
        if (room == null) return;
        _hubCamera.transform.position = room.CameraPosition;
    }
}
