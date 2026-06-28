using UnityEngine;

//MonoBehaviour que va en el GameObject raíz de cada habitación, gestiona su activación y expone sus datos al resto de sistemas.
public class HubRoom : MonoBehaviour
{
    [SerializeField] private HubRoomData _data;

    public HubRoomData Data => _data;

    public string RoomId => _data != null ? _data.roomId : string.Empty;
    public Vector3 CameraPosition => _data != null ? _data.cameraPosition : Vector3.zero;

    // ─────────────────────────────────────────
    // ACTIVACIÓN
    // ─────────────────────────────────────────

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
