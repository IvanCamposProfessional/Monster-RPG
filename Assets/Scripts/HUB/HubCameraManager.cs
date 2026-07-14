using UnityEngine;

//Gestiona la cámara del HUB. Sigue al player suavemente dentro de la habitación activa.
public class HubCameraManager : MonoBehaviour
{
    public static HubCameraManager Instance { get; private set; }
    [SerializeField] private Camera _hubCamera;
    [SerializeField] private Transform _target; //el Transform del Player
    [SerializeField] private float _smoothSpeed = 5f;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        //Seguimos al player manteniendo la Z de la cámara
        Vector3 targetPos = new Vector3(_target.position.x, _target.position.y, _hubCamera.transform.position.z);
        _hubCamera.transform.position = Vector3.Lerp(_hubCamera.transform.position, targetPos, _smoothSpeed * Time.deltaTime);
    }

    // ─────────────────────────────────────────
    // PÚBLICO
    // ─────────────────────────────────────────

    //Teletransporte instantáneo — se usa al cambiar de habitación para evitar que la cámara atraviese el negro
    public void SnapToTarget()
    {
        if (_target == null) return;
        _hubCamera.transform.position = new Vector3(_target.position.x, _target.position.y, _hubCamera.transform.position.z);
    }
}
