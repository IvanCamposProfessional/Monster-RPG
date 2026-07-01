using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Gestiona el intercambio de habitaciones y el teletransporte del jugador.
public class HubTransitionManager : MonoBehaviour
{
    public static HubTransitionManager Instance { get; private set; }

    [Header("Fade")]
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDuration = 0.3f;
    [SerializeField] private bool _fadeEnabled = true;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        SetAlpha(0f);
    }

    // ─────────────────────────────────────────
    // TRANSICIÓN
    // ─────────────────────────────────────────

    public void TransitionToRoom(string destinationRoomId, Vector2Int spawnTile, HubDoor door)
    {
        StartCoroutine(TransitionCoroutine(destinationRoomId, spawnTile, door));
    }

    private IEnumerator TransitionCoroutine(string destinationRoomId, Vector2Int spawnTile, HubDoor door)
    {
        HubRoom destination = FindRoomById(destinationRoomId);
        if (destination == null)
        {
            Debug.LogWarning("HubTransitionManager: no se encontró la habitación " + destinationRoomId);
            HubManager.Instance.UnblockInput();
            yield break;
        }

        HubRoom origin = FindActiveRoom();

        //1 — Fade Out
        yield return StartCoroutine(FadeOut());

        //2 — Activar destino
        destination.Activate();

        //3 — Teletransportar player al spawnTile y reinicializar grilla
        HubPlayerController.Instance.TeleportToTile(spawnTile, destination, door.EntryDirection);

        //4 — Cámara snappea instantáneamente al player en la nueva habitación
        HubCameraManager.Instance.SnapToTarget();

        //5 — Desactivar origen
        if (origin != null && origin != destination)
            origin.Deactivate();

        //6 — Fade In
        yield return StartCoroutine(FadeIn());

        //7 — Devolver control
        HubManager.Instance.UnblockInput();
    }

    // ─────────────────────────────────────────
    // FADE
    // ─────────────────────────────────────────

    private IEnumerator FadeOut()
    {
        if (!_fadeEnabled) yield break;
        yield return StartCoroutine(Fade(0f, 1f));
    }

    private IEnumerator FadeIn()
    {
        if (!_fadeEnabled) yield break;
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        SetAlpha(from);

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, elapsed / _fadeDuration));
            yield return null;
        }

        SetAlpha(to);
    }

    private void SetAlpha(float alpha)
    {
        if (_fadeImage == null) return;
        Color c = _fadeImage.color;
        c.a = alpha;
        _fadeImage.color = c;
    }

    // ─────────────────────────────────────────
    // UTILIDADES
    // ─────────────────────────────────────────

    private HubRoom FindRoomById(string roomId)
    {
        foreach (HubRoom room in FindObjectsByType<HubRoom>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (room.RoomId == roomId) return room;

        return null;
    }

    private HubRoom FindActiveRoom()
    {
        foreach (HubRoom room in FindObjectsByType<HubRoom>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (room.gameObject.activeSelf) return room;

        return null;
    }
}
