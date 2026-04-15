using UnityEngine;
using UnityEngine.EventSystems;

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

    // ─────────────────────────────────────────
    // SETUP
    // ─────────────────────────────────────────

    public void Setup(RunNodeData data)
    {
        NodeData = data;
        ApplySprite();
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
}
