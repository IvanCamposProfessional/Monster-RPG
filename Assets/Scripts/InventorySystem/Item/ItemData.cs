using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string ItemID;
    public string ItemName;
    [TextArea] public string ItemDescription;
    public ItemCategory Category;
    public Sprite ItemSprite;
}
