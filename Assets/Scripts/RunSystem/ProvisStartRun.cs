using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ProvisStartRun : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene("RunScene");
    }
}
