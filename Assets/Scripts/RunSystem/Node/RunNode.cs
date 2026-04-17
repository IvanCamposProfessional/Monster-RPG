using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

//Componente que va en el prefab de cada nodo de run
public class RunNode : MonoBehaviour, IPointerClickHandler
{
    //Datos del nodo que representa este prefab
    public RunNodeData NodeData { get; private set; }

    [SerializeField] private SpriteRenderer nodeSprite;

    [Header("Sprites por tipo")]
    [SerializeField] private Sprite campSprite;
    [SerializeField] private Sprite battleSprite;
    [SerializeField] private Sprite eliteSprite;
    [SerializeField] private Sprite shopSprite;
    [SerializeField] private Sprite eventSprite;
    [SerializeField] private Sprite bossSprite;

    [Header("Colores de estado")]
    //Color cuando el nodo es clickable
    [SerializeField] private Color reachableColor = Color.white;
    //Color cuando el nodo no es clickable aun
    [SerializeField] private Color lockedColor    = new Color(0.35f, 0.35f, 0.35f, 1f);
    //Color cuando el nodo ya fue visitado
    [SerializeField] private Color visitedColor   = new Color(0.6f, 0.6f, 0.6f, 0.6f);

    [Header("Nombres de escena")]
    [SerializeField] private string combatSceneName = "CombatScene";

    // ─────────────────────────────────────────
    // SETUP
    // ─────────────────────────────────────────

    public void Setup(RunNodeData data, float nodeSize)
    {
        NodeData = data;
        ApplySprite();
        ApplySpriteSize(nodeSize);
        ApplyColliderSize(nodeSize);
        RefreshVisual();
    }

    

    //Refresca el color segun el estado actual del nodo
    public void RefreshVisual()
    {
        //Comprobacion de seguridad
        if (nodeSprite == null) return;

        if (NodeData.isVisited)
            nodeSprite.color = visitedColor;
        else if (NodeData.isReachable)
            nodeSprite.color = reachableColor;
        else
            nodeSprite.color = lockedColor;
    }

    // ─────────────────────────────────────────
    // INPUT
    // ─────────────────────────────────────────

    public void OnPointerClick(PointerEventData eventData)
    {
        //Solo procesamos el click si el nodo es alcanzable
        if (!NodeData.isReachable) return;

        // Notificamos al RunManager que el jugador ha visitado este nodo
        RunManager.Instance.OnNodeVisited(NodeData.nodeId);

        //Cargamos la escena correspondiente segun el tipo de nodo
        HandleNodeEvent();
    }

    // ─────────────────────────────────────────
    // PRIVADOS
    // ─────────────────────────────────────────

    private void ApplySprite()
    {
        //Comprobacion de seguridad
        if (nodeSprite == null) return;

        nodeSprite.sprite = NodeData.nodeType switch
        {
            NodeType.Camp   => campSprite,
            NodeType.Battle => battleSprite,
            NodeType.Elite  => eliteSprite,
            NodeType.Shop   => shopSprite,
            NodeType.Event  => eventSprite,
            NodeType.Boss   => bossSprite,
            _               => battleSprite
        };
    }

    //Escala el SpriteRenderer para que el sprite ocupe nodeSize unidades
    private void ApplySpriteSize(float nodeSize)
    {
        //Comprobacion de seguridad
        if (nodeSprite == null || nodeSprite.sprite == null) return;

        //Escalamos el sprite a 1,1,1
        nodeSprite.transform.localScale = Vector3.one;

        //Guardamos el tamaño actual del sprite
        Vector2 spriteSize = nodeSprite.sprite.bounds.size;

        //Escala acumulada del padre
        Vector3 parentScale = nodeSprite.transform.parent != null ? nodeSprite.transform.parent.lossyScale : Vector3.one;

        //Compensamos la escala del padre para que el resultado en mundo sea nodeSize
        float scaleX = nodeSize / (spriteSize.x * parentScale.x);
        float scaleY = nodeSize / (spriteSize.y * parentScale.y);
        float scale  = Mathf.Min(scaleX, scaleY);;

        //Aplicamos la escala calculada al sprite
        nodeSprite.transform.localScale = new Vector3(scale, scale, 1f);
    }

    private void ApplyColliderSize(float nodeSize)
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null) return;
        //Radio = mitad del nodeSize, compensando la escala del transform raiz
        col.radius = (nodeSize * 0.6f) / transform.lossyScale.x;
    }

    private void HandleNodeEvent()
    {
        switch (NodeData.nodeType)
        {
            case NodeType.Battle:
            case NodeType.Elite:
            case NodeType.Boss:
                LoadCombat();
                break;
            case NodeType.Camp:
                Debug.Log("RunNode: nodo Camp — pendiente de implementar");
                break;
 
            case NodeType.Shop:
                Debug.Log("RunNode: nodo Shop — pendiente de implementar");
                break;
 
            case NodeType.Event:
                Debug.Log("RunNode: nodo Event — pendiente de implementar");
                break;
        }
    }

    private void LoadCombat()
    {
        SceneManager.LoadScene(combatSceneName);
    }
}
